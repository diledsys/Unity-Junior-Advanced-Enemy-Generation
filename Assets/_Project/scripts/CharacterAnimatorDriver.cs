using UnityEngine;

public class CharacterAnimatorDriver : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterMotorCC _motor;
    [SerializeField] private MonoBehaviour _providerBehaviour; // сюда перетащи компонент-провайдер

    [Header("Params")]
    [SerializeField] private string _speedParam = "Speed";
    [SerializeField] private string _motionSpeedParam = "MotionSpeed";

    private IMoveVectorProvider _provider;

    private void Awake()
    {
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();

        if (_motor == null)
            _motor = GetComponent<CharacterMotorCC>();

        _provider = _providerBehaviour as IMoveVectorProvider;

        if (_provider == null)
            _provider = GetComponent<IMoveVectorProvider>();
    }

    private void Update()
    {
        if (_animator == null || _motor == null)
            return;

        Vector3 v = _motor.Velocity;
        float horizontalSpeed = new Vector3(v.x, 0f, v.z).magnitude;

        float intensity01 = _provider != null ? Mathf.Clamp01(_provider.MoveIntensity) : 0f;

        _animator.SetFloat(_speedParam, horizontalSpeed);
        _animator.SetFloat(_motionSpeedParam, intensity01);
    }
}