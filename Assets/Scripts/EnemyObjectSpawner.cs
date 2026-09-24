using System.Collections;
using UnityEngine;

public class EnemyObjectSpawner : MonoBehaviour
{
    [SerializeField] public EnemyObject enemyObjectPrefab;
    [SerializeField] private float spawnInterval = 3;

    [SerializeField] private Vector2 minValue = new(-50, -50);
    [SerializeField] private Vector2 maxValue = new(50, 50);

    public void StartSpawning()
    {
        StartCoroutine(StartSpawningRandomly());
    }

    private IEnumerator StartSpawningRandomly()
    {
        while(GamePlayManager.Instance.IsGameOn)
        {
            Instantiate(enemyObjectPrefab, GetRandomSpawnPoint(), Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(spawnInterval, spawnInterval + 5));
        }
    }

    private Vector3 GetRandomSpawnPoint()
    {
        return new Vector3(Random.Range(minValue.x, maxValue.x), 1.5f, Random.Range(minValue.y, maxValue.y));
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(maxValue.x * 2, 0.1f, maxValue.y * 2)
        );
    }
}
