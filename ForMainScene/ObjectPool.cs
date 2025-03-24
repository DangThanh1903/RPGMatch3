using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs; // Array of different birds/clouds
    [SerializeField] private int poolSize = 10;
    private Queue<GameObject> poolQueue = new Queue<GameObject>();

    void Start()
    {
        // Pre-instantiate random prefabs
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(GetRandomPrefab());
            obj.SetActive(false);
            poolQueue.Enqueue(obj);
        }
    }

    private GameObject GetRandomPrefab()
    {
        return prefabs[Random.Range(0, prefabs.Length)];
    }

    public GameObject GetFromPool(Vector3 position)
    {
        GameObject obj;
        if (poolQueue.Count > 0)
        {
            obj = poolQueue.Dequeue();
        }
        else
        {
            obj = Instantiate(GetRandomPrefab());
        }

        obj.transform.position = position;
        obj.SetActive(true);
        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        poolQueue.Enqueue(obj);
    }
}
