using UnityEngine;

public class EnemyLifecycle : MonoBehaviour
{
    public event System.Action Destroyed;
    private void OnDestroy() => Destroyed?.Invoke();
}