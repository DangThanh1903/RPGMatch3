using UnityEngine;

public class Bird : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    private ObjectPool pool;

    public void Initialize(ObjectPool objectPool)
    {
        pool = objectPool;
    }

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;

        // If out of screen, return to pool
        if (transform.position.x > 12f)
        {
            pool.ReturnToPool(gameObject);
        }
    }
}
