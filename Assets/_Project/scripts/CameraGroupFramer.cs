using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraGroupFramer : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private List<Transform> targets = new();

    [Header("Framing")]
    [SerializeField] private float _padding = 2.0f;
    [SerializeField] private float _minDistance = 8f;
    [SerializeField] private float _maxDistance = 60f;

    [Tooltip("Сдвиг точки фокуса вверх/вниз в мировых метрах. + вверх, - вниз.")]
    [SerializeField] private float _focusYOffset = 1.5f;

    [Tooltip("Насколько сильно смещать группу вниз в экране (0..0.5). 0.15 обычно красиво.")]
    [Range(0f, 0.5f)]
    [SerializeField] private float _screenBiasDown = 0.15f;

    [Header("Smoothing")]
    [SerializeField] private float _positionSmoothTime = 0.25f;
    [SerializeField] private float _distanceSmoothTime = 0.25f;
    [SerializeField] private float _rotationSmoothTime = 0.18f;

    [Header("Camera Orbit / Angle")]
    [SerializeField] private Vector3 _rigOffset = new Vector3(0, 0, 0); 
    [SerializeField] private float _pitchDegrees = 35f;
    [SerializeField] private float _yawDegrees = 0f;

    private Camera _cam;

    private Vector3 _posVel;
    private float _distVel;

    private float _currentDistance;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
        _currentDistance = Vector3.Distance(transform.position, Vector3.zero);
    }

    private void LateUpdate()
    {
        if (targets == null || targets.Count == 0)
            return;

        Bounds bounds = CalculateBounds(targets);
        Vector3 focus = bounds.center + Vector3.up * _focusYOffset;

        focus += Vector3.up * ( bounds.extents.y * _screenBiasDown );

        float desiredDistance = CalculateDistanceToFit(bounds);
        desiredDistance = Mathf.Clamp(desiredDistance, _minDistance, _maxDistance);

        _currentDistance = Mathf.SmoothDamp(_currentDistance, desiredDistance, ref _distVel, _distanceSmoothTime);

        Quaternion desiredRot = Quaternion.Euler(_pitchDegrees, _yawDegrees, 0f);
        Quaternion smoothedRot = Quaternion.Slerp(transform.rotation, desiredRot, 1f - Mathf.Exp(-_rotationSmoothTime * Time.deltaTime));

        Vector3 desiredPos = focus + ( smoothedRot * ( Vector3.back * _currentDistance ) ) + _rigOffset;
        Vector3 smoothedPos = Vector3.SmoothDamp(transform.position, desiredPos, ref _posVel, _positionSmoothTime);

        transform.SetPositionAndRotation(smoothedPos, smoothedRot);
    }

    private Bounds CalculateBounds(List<Transform> list)
    {
        Vector3 first = list[0].position;
        Bounds b = new Bounds(first, Vector3.zero);

        for (int i = 0; i < list.Count; i++)
        {
            Transform t = list[i];
            if (t == null)
                continue;
            b.Encapsulate(t.position);
        }

        b.Expand(_padding * 2f);
        return b;
    }

    private float CalculateDistanceToFit(Bounds b)
    {
       
        float fovRad = _cam.fieldOfView * Mathf.Deg2Rad;
        float halfFovTan = Mathf.Tan(fovRad * 0.5f);

        float halfHeight = b.extents.y;
        float halfWidth = b.extents.x;

        float halfWidthAsHeight = halfWidth / Mathf.Max(_cam.aspect, 0.0001f);

        float neededByHeight = halfHeight / Mathf.Max(halfFovTan, 0.0001f);
        float neededByWidth = halfWidthAsHeight / Mathf.Max(halfFovTan, 0.0001f);

        return Mathf.Max(neededByHeight, neededByWidth);
    }

    public void SetTargets(List<Transform> newTargets) => targets = newTargets;

    public void AddTarget(Transform t)
    {
        if (t != null && !targets.Contains(t))
            targets.Add(t);
    }

    public void RemoveTarget(Transform t)
    {
        if (t != null)
            targets.Remove(t);
    }
}