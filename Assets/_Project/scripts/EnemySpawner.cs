using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("What to spawn")]
    [SerializeField] private ChaseTargetSteering _enemyPrefab;

    [Header("Where to go")]
    [SerializeField] private Transform _target;

    [Header("Spawn settings")]
    [SerializeField] private float _spawnInterval = 2.0f;
    [SerializeField] private int _maxAlive = 10;
    [SerializeField] private float _spawnRadius = 0.5f;

    [Header("Lifetime")]
    [SerializeField] private float _enemyLifetime = 20f;

    private Coroutine _spawnRoutine;
    private int _alive;

    private void OnEnable()
    {
        _spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    private void OnDisable()
    {
        if (_spawnRoutine != null)
            StopCoroutine(_spawnRoutine);
    }

    private IEnumerator SpawnRoutine()
    {
        while (enabled)
        {
            TrySpawnOne();
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    private void TrySpawnOne()
    {
        if (_enemyPrefab == null || _target == null)
            return;

        if (_alive >= _maxAlive)
            return;

        SpawnOne();
    }

    private void SpawnOne()
    {
        Vector3 position = transform.position + Random.insideUnitSphere * _spawnRadius;
        position.y = transform.position.y;

        ChaseTargetSteering enemy = Instantiate(_enemyPrefab, position, transform.rotation);
        enemy.SetTarget(_target);

        _alive++;

        EnemyLifecycle lifecycle = enemy.gameObject.AddComponent<EnemyLifecycle>();
        lifecycle.Destroyed += OnEnemyDestroyed;

        Destroy(enemy.gameObject, _enemyLifetime);
    }

    private void OnEnemyDestroyed()
    {
        _alive = Mathf.Max(0, _alive - 1);
    }
}