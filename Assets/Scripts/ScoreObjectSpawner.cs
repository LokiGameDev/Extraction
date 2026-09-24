using System.Collections;
using UnityEngine;

public class ScoreObjectSpawner : MonoBehaviour
{
    [SerializeField] public ScoreObject scoreObjectPrefab;
    [SerializeField] private float spawnInterval = 3;

    [SerializeField] private Vector2 minValue = new(-50, -50);
    [SerializeField] private Vector2 maxValue = new(50, 50);

    private ObjectPool<ScoreObject> scorePool;

    private void Awake()
    {
        scorePool = new ObjectPool<ScoreObject>(
            scoreObjectPrefab,
            10,
            transform
        );
    }

    public void StartSpawning()
    {
        StartCoroutine(StartSpawningRandomly());
    }

    private IEnumerator StartSpawningRandomly()
    {
        while(GamePlayManager.Instance.IsGameOn)
        {
            SpawnScoreObject();
            yield return new WaitForSeconds(Random.Range(spawnInterval, spawnInterval + 5));
        }
    }

    private Vector3 GetRandomSpawnPoint()
    {
        return new Vector3(Random.Range(minValue.x, maxValue.x), 1.5f, Random.Range(minValue.y, maxValue.y));
    }

    public void SpawnScoreObject()
    {
        ScoreObject scoreObject = scorePool.Get(GetRandomSpawnPoint(), Quaternion.identity);

        scoreObject.GetComponent<ScoreObject>().Setup(Random.Range(5,10), this);
    }

    public void ReturnScore(ScoreObject score)
    {
        scorePool.Return(score);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(maxValue.x * 2, 0.1f, maxValue.y * 2)
        );
    }
}
