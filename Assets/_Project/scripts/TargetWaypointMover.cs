using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TargetWaypointMover : MonoBehaviour
{
    [Header("Path")]
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private bool _loop = true;

    [Header("Move")]
    [SerializeField] private float _moveSpeed = 2.5f;
    [SerializeField] private float _turnSpeed = 12f;
    [SerializeField] private float _arriveDistance = 0.2f;

    [Header("Gravity")]
    [SerializeField] private float _gravity = -20f;
    [SerializeField] private float _groundedStick = -2f;
    [SerializeField] private LayerMask _groundLayers = ~0;
    [SerializeField] private float _groundCheckOffset = 0.1f;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [SerializeField] private string _speedParam = "Speed";

    private CharacterController _characterController;
    private int _index;
    private int _dir = 1;

    private float _verticalVelocity;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (_waypoints == null || _waypoints.Length == 0)
            return;

        ApplyGravity();

        Vector3 targetPos = _waypoints[_index].position;
        Vector3 position = targetPos - transform.position;
        position.y = 0f;

        float dist = position.magnitude;
        if (dist <= _arriveDistance)
        {
            AdvanceIndex();
            _animator?.SetFloat(_speedParam, 0f);

            _characterController.Move(Vector3.up * ( _verticalVelocity * Time.deltaTime ));
            return;
        }

        Vector3 dir = position.normalized;

        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion desired = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, desired, _turnSpeed * Time.deltaTime);
        }

        Vector3 horizontal = dir * _moveSpeed;
        Vector3 motion = new Vector3(horizontal.x, _verticalVelocity, horizontal.z);

        _characterController.Move(motion * Time.deltaTime);

        _animator?.SetFloat(_speedParam, _moveSpeed);
    }

    private void ApplyGravity()
    {
        Vector3 origin = transform.position + _characterController.center;
        float sphereRadius = Mathf.Max(0.01f, _characterController.radius - 0.02f);
        float castDistance = ( _characterController.height * 0.5f ) + _groundCheckOffset;

        bool grounded = Physics.SphereCast(
            origin,
            sphereRadius,
            Vector3.down,
            out _,
            castDistance,
            _groundLayers,
            QueryTriggerInteraction.Ignore);

        if (grounded && _verticalVelocity < 0f)
        {
            _verticalVelocity = _groundedStick;
        }
        else
        {
            _verticalVelocity += _gravity * Time.deltaTime;
        }
    }

    private void AdvanceIndex()
    {
        if (_index >= _waypoints.Length)
            _index = _loop ? 0 : _waypoints.Length - 1;

        _index += _dir;
        if (_index >= _waypoints.Length)
        {
            _dir = -1;
            _index = _waypoints.Length - 2;
        }
        else if (_index < 0)
        {
            _dir = 1;
            _index = 1;
        }
    }
}