using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private ObjectPool birdPool;  // Pool for birds
    [SerializeField] private ObjectPool cloudPool; // Pool for clouds

    [SerializeField] private float spawnRate = 2f;
    [SerializeField] private float birdYMin = 2f, birdYMax = 5f;
    [SerializeField] private float cloudYMin = 1f, cloudYMax = 4f;
    [SerializeField] private float spawnX = -10f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnBird), 1f, spawnRate);
        InvokeRepeating(nameof(SpawnCloud), 2f, spawnRate * 1.5f);
    }

    void SpawnBird()
    {
        Vector3 position = new Vector3(spawnX, Random.Range(birdYMin, birdYMax), 0);
        GameObject bird = birdPool.GetFromPool(position);
        bird.GetComponent<Bird>().Initialize(birdPool);
    }

    void SpawnCloud()
    {
        Vector3 position = new Vector3(spawnX, Random.Range(cloudYMin, cloudYMax), 0);
        GameObject cloud = cloudPool.GetFromPool(position);
        cloud.GetComponent<Cloud>().Initialize(cloudPool);
    }
}
