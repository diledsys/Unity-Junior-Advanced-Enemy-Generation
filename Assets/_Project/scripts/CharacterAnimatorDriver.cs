using UnityEngine;

public class CharacterAnimatorDriver : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    [Header("Params")]
    [SerializeField] private string _speedParam = "Speed";
    [SerializeField] private string _motionSpeedParam = "MotionSpeed";

    private void Awake()
    {
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();
    }

    public void SetSpeed(float speed)
    {
        if (_animator == null)
            return;

        _animator.SetFloat(_speedParam, speed);
    }

    public void SetMotionSpeed(float motionSpeed)
    {
        if (_animator == null)
            return;

        _animator.SetFloat(_motionSpeedParam, motionSpeed);
    }

    public void SetLocomotion(float speed, float motionSpeed)
    {
        if (_animator == null)
            return;

        _animator.SetFloat(_speedParam, speed);
        _animator.SetFloat(_motionSpeedParam, motionSpeed);
    }
}