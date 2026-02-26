using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("What to spawn")]
    [SerializeField] private EnemyChaseTarget enemyPrefab;

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
        if (enemyPrefab == null || _target == null)
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
        Vector3 position = transform.position + Random.insideUnitSphere * _spawnRadius;
        position.y = transform.position.y;

        EnemyChaseTarget enemy = Instantiate(enemyPrefab, position, transform.rotation);
        enemy.SetTarget(_target);

        _alive++;

        Destroy(enemy.gameObject, _enemyLifetime);

        var life = enemy.gameObject.AddComponent<SpawnedLifetimeHook>();
        life.Init(this);
    }

    private class SpawnedLifetimeHook : MonoBehaviour
    {
        private EnemySpawner _spawner;

        public void Init(EnemySpawner spawner) => _spawner = spawner;

        private void OnDestroy()
        {
            if (_spawner != null)
                _spawner._alive = Mathf.Max(0, _spawner._alive - 1);
        }
    }
}