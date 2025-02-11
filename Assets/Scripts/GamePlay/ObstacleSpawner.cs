using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public static ObstacleSpawner Instance { get; private set; }
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject spawnAreaObject;
    [SerializeField] private int minObstacles;
    [SerializeField] private int maxObstacles;
    [SerializeField] private Transform obstacleParent;

    private BoxCollider spawnAreaCollider;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Spawn();

    }

    public void Spawn()
    {
        spawnAreaCollider = spawnAreaObject.GetComponent<BoxCollider>();
        if (spawnAreaCollider != null)
        {
            ClearExistingObstacles();
            SpawnObstacles();
        }
        else
        {
            Debug.LogError("Spawn area object does not have a BoxCollider component.");
        }
    }

    private void ClearExistingObstacles()
    {
        foreach (Transform child in obstacleParent)
        {
            // Destroy(child.gameObject);
        }
    }

    private void SpawnObstacles()
    {
        int obstacleCount = Random.Range(minObstacles, maxObstacles);
        for (int i = 0; i < obstacleCount; i++)
        {
            Vector3 spawnPosition = GetRandomPositionInSpawnArea();
            var randomRotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            var obstacle = Instantiate(obstaclePrefab, spawnPosition, randomRotation);
            obstacle.transform.SetParent(obstacleParent);
        }
    }

    Vector3 GetRandomPositionInSpawnArea()
    {
        Vector3 center = spawnAreaCollider.bounds.center;
        Vector3 size = spawnAreaCollider.bounds.size;

        float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float y = Random.Range(center.y - size.y / 2, center.y + size.y / 2);
        float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

        return new Vector3(x, y, z);
    }
}
