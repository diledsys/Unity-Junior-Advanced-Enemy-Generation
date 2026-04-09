using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("What to spawn")]
    [SerializeField] private GameObject _enemyPrefab; // prefab ñ EnemyChaseTargetProvider + Motor + AnimatorDriver

    [Header("Where to go")]
    [SerializeField] private Transform _target;

    [Header("Spawn settings")]
    [SerializeField] private float _spawnInterval = 2.0f;
    [SerializeField] private int _maxAlive = 10;
    [SerializeField] private float _spawnRadius = 0.5f;

    [Header("Lifetime")]
    [SerializeField] private float _enemyLifetime = 20f;

    private float _timer;
    private int _alive;

    private void Update()
    {
        if (_enemyPrefab == null || _target == null)
            return;

        if (_alive >= _maxAlive)
            return;

        _timer -= Time.deltaTime;

        if (_timer > 0f)
            return;

        SpawnOne();
        _timer = _spawnInterval;
    }

    private void SpawnOne()
    {
        Vector3 pos = transform.position + Random.insideUnitSphere * _spawnRadius;
        pos.y = transform.position.y;

        GameObject enemyGo = Instantiate(_enemyPrefab, pos, transform.rotation);

        var provider = enemyGo.GetComponent<EnemyChaseTargetProvider>();

        if (provider != null)
            provider.SetTarget(_target);

        _alive++;

        var lifecycle = enemyGo.AddComponent<EnemyLifecycle>();
        lifecycle.Destroyed += OnEnemyDestroyed;

        Destroy(enemyGo, _enemyLifetime);
    }

    private void OnEnemyDestroyed()
    {
        _alive = Mathf.Max(0, _alive - 1);
    }
}

public class EnemyLifecycle : MonoBehaviour
{
    public event System.Action Destroyed;
    private void OnDestroy() => Destroyed?.Invoke();
}