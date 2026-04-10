using UnityEngine;

public class ChaseTargetSteering : MonoBehaviour, IMoveVectorProvider
{
    [Header("Chase")]
    [SerializeField] private float _moveSpeed = 3.5f;
    [SerializeField] private float _stopDistance = 1.2f;

    private Transform _target;
    private float _stopDistanceSqr;

    public bool ShouldRotate => true;

    public Vector3 LookDirection
    {
        get
        {
            if (_target == null)
                return transform.forward;

            Vector3 to = _target.position - transform.position;
            to.y = 0f;
            return to.sqrMagnitude > 0.0001f ? to : transform.forward;
        }
    }

    public float MoveIntensity { get; private set; }

    public void SetTarget(Transform target) => _target = target;

    private void Awake()
    {
        _stopDistanceSqr = _stopDistance * _stopDistance;
    }

    public Vector3 GetDesiredHorizontalVelocity()
    {
        if (_target == null)
        {
            MoveIntensity = 0f;
            return Vector3.zero;
        }

        Vector3 to = _target.position - transform.position;
        to.y = 0f;

        if (to.sqrMagnitude <= _stopDistanceSqr)
        {
            MoveIntensity = 0f;
            return Vector3.zero;
        }

        Vector3 dir = to.normalized;
        MoveIntensity = 1f;
        return dir * _moveSpeed;
    }
}