using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMover : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _turnSpeed = 14f;

    [Header("Gravity")]
    [SerializeField] private float _gravity = -20f;
    [SerializeField] private float _groundedStick = -2f;
    [SerializeField] private LayerMask _groundLayers = ~0;
    [SerializeField] private float _groundCheckOffset = 0.1f;

    private CharacterController _characterController;
    private IMoveVectorProvider _moveProvider;

    private float _verticalSpeed;

    public Vector3 Velocity => _characterController.velocity;
    public bool IsGrounded { get; private set; }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _moveProvider = GetComponent<IMoveVectorProvider>();
    }

    private void Update()
    {
        ApplyGravity();

        Vector3 horizontal = _moveProvider != null
            ? _moveProvider.GetDesiredHorizontalVelocity()
            : Vector3.zero;

        RotateIfNeeded(horizontal);

        Vector3 velocity = new Vector3(horizontal.x, _verticalSpeed, horizontal.z);
        _characterController.Move(velocity * Time.deltaTime);
    }

    private void RotateIfNeeded(Vector3 horizontal)
    {
        if (_moveProvider == null || !_moveProvider.ShouldRotate)
            return;

        Vector3 lookDir = _moveProvider.LookDirection;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude < 0.0001f)
            return;

        Quaternion desired = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desired, _turnSpeed * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        Vector3 origin = transform.position + _characterController.center;
        float radius = Mathf.Max(0.01f, _characterController.radius - 0.02f);
        float castDistance = ( _characterController.height * 0.5f ) + _groundCheckOffset;

        IsGrounded = Physics.SphereCast(
            origin,
            radius,
            Vector3.down,
            out _,
            castDistance,
            _groundLayers,
            QueryTriggerInteraction.Ignore);

        if (IsGrounded && _verticalSpeed < 0f)
            _verticalSpeed = _groundedStick;
        else
            _verticalSpeed += _gravity * Time.deltaTime;
    }
}