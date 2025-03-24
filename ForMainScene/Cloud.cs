using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    private ObjectPool pool;

    public void Initialize(ObjectPool objectPool)
    {
        pool = objectPool;
    }

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;

        // Return to pool if out of screen
        if (transform.position.x > 12f)
        {
            pool.ReturnToPool(gameObject);
        }
    }
}
