using UnityEngine;

public class WaypointMoveProvider : MonoBehaviour, IMoveVectorProvider
{
    [Header("Path")]
    [SerializeField] private Transform _pathRoot;

    [Header("Move")]
    [SerializeField] private float _moveSpeed = 2.5f;
    [SerializeField] private float _arriveDistance = 0.2f;

    private int _index;
    private float _arriveDistanceSqr;

    public bool ShouldRotate => true;
    public float MoveIntensity01 { get; private set; }

    public Vector3 LookDirection
    {
        get
        {
            Transform wp = GetWaypoint(_index);
            if (wp == null)
                return transform.forward;

            Vector3 to = wp.position - transform.position;
            to.y = 0f;
            return to.sqrMagnitude > 0.0001f ? to : transform.forward;
        }
    }

    private void Awake()
    {
        _arriveDistanceSqr = _arriveDistance * _arriveDistance;
    }

    public Vector3 GetDesiredHorizontalVelocity()
    {
        int count = GetWaypointCount();
        if (_pathRoot == null || count == 0)
        {
            MoveIntensity01 = 0f;
            return Vector3.zero;
        }

        _index %= count;

        Transform wp = GetWaypoint(_index);
        if (wp == null)
        {
            MoveIntensity01 = 0f;
            return Vector3.zero;
        }

        Vector3 to = wp.position - transform.position;
        to.y = 0f;

        if (to.sqrMagnitude <= _arriveDistanceSqr)
        {
            AdvanceIndex(count);
            MoveIntensity01 = 0f;
            return Vector3.zero;
        }

        Vector3 dir = to.normalized;
        MoveIntensity01 = 1f;
        return dir * _moveSpeed;
    }

    private void AdvanceIndex(int count)
    {
        _index++;
        _index %= count;
    }

    private int GetWaypointCount() => _pathRoot != null ? _pathRoot.childCount : 0;
    private Transform GetWaypoint(int i) => _pathRoot != null && i >= 0 && i < _pathRoot.childCount
        ? _pathRoot.GetChild(i)
        : null;
}