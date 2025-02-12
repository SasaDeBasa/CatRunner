using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public float spawnIntervalMin = 1.5f;
    public float spawnIntervalMax = 3f;
    public Transform spawnPoint;

    private bool hasStarted = false;
    private bool isGameOver = false;

    void Start()
    {
        StartSpawning();
    }

    void StartSpawning()
    {
        Invoke("SpawnObstacle", Random.Range(spawnIntervalMin, spawnIntervalMax));
    }

    void SpawnObstacle()
    {
        if (!hasStarted || isGameOver) return;

        Instantiate(obstaclePrefab, spawnPoint.position, Quaternion.identity);

        // Schedule the next spawn
        Invoke("SpawnObstacle", Random.Range(spawnIntervalMin, spawnIntervalMax));
    }

    public void StartGame()
    {
        hasStarted = true;
        StartSpawning();
    }

    public void StopGame()
    {
        isGameOver = true;
    }
}
