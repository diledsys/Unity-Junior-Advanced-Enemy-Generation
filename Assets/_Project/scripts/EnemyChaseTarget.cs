using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyChaseTarget : MonoBehaviour
{
    [Header("Chase")]
    [SerializeField] private float _moveSpeed = 3.5f;
    [SerializeField] private float _turnSpeed = 14f;
    [SerializeField] private float _stopDistance = 1.2f;

    [Header("Gravity")]
    [SerializeField] private float _gravity = -20f;
    [SerializeField] private float _groundedStick = -2f;
    [SerializeField] private LayerMask _groundLayers = ~0;
    [SerializeField] private float _groundCheckOffset = 0.1f;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [SerializeField] private string _speedParam = "Speed";
    [SerializeField] private string _motionSpeedParam = "MotionSpeed";

    private CharacterController _characterController;
    private Transform _target;

    private float _verticalVelocity;

    public void SetTarget(Transform target) => _target = target;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        ApplyGravity();

        if (_target == null)
        {
            _animator?.SetFloat(_speedParam, 0f);
            _animator?.SetFloat(_motionSpeedParam, 0f);

            _characterController.Move(Vector3.up * ( _verticalVelocity * Time.deltaTime ));
            return;
        }

        Vector3 transformPosition = _target.position - transform.position;
        transformPosition.y = 0f;

        float dist = transformPosition.magnitude;

        if (dist <= _stopDistance)
        {
            _animator?.SetFloat(_speedParam, 0f);
            _animator?.SetFloat(_motionSpeedParam, 0f);
            _characterController.Move(Vector3.up * ( _verticalVelocity * Time.deltaTime ));
            return;
        }

        Vector3 dir = transformPosition.normalized;

        Quaternion desired = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desired, _turnSpeed * Time.deltaTime);

        Vector3 horizontalVel = dir * _moveSpeed;
        Vector3 velocity = new Vector3(horizontalVel.x, _verticalVelocity, horizontalVel.z);

        _characterController.Move(velocity * Time.deltaTime);

        float animSpeed = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z).magnitude;
        _animator?.SetFloat(_speedParam, animSpeed);
        _animator?.SetFloat(_motionSpeedParam, 1f);
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
            _verticalVelocity = _groundedStick;
        else
            _verticalVelocity += _gravity * Time.deltaTime;
    }
}