using UnityEngine;

[RequireComponent(typeof(CharacterMover))]
[RequireComponent(typeof(CharacterAnimatorDriver))]
public class CharacterAnimationSync : MonoBehaviour
{
    [SerializeField] private CharacterMover _motor;
    [SerializeField] private MonoBehaviour _providerBehaviour;

    private CharacterAnimatorDriver _animatorDriver;
    private IMoveVectorProvider _provider;

    private void Awake()
    {
        if (_motor == null)
            _motor = GetComponent<CharacterMover>();

        _animatorDriver = GetComponent<CharacterAnimatorDriver>();
        _provider = _providerBehaviour as IMoveVectorProvider;

        if (_provider == null)
            _provider = GetComponent<IMoveVectorProvider>();
    }

    private void Update()
    {
        Vector3 velocity = _motor.Velocity;
        float horizontalSpeed = new Vector3(velocity.x, 0f, velocity.z).magnitude;
        float moveIntensity = _provider != null ? Mathf.Clamp01(_provider.MoveIntensity) : 0f;

        _animatorDriver.SetLocomotion(horizontalSpeed, moveIntensity);
    }
}