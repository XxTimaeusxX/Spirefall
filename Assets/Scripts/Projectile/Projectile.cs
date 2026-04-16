using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 20f;
    
    private float lifeTimer = 0f;
    private float maxLifeTime = 3f;

    // Use Awake to get references since Start only runs once and pooling reuses the object
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Called every time the object is activated from the pool
    void OnEnable()
    {
        lifeTimer = maxLifeTime; 
    }

    // Update is called once per frame
    void Update()
    {
        ProjectileMove();

        // Handle the lifetime timer manually instead of using Destroy()
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            ReturnToPool();
        }
    }

    public void ProjectileMove()
    {
        rb.linearVelocity = transform.forward * speed;
    }

    private void ReturnToPool()
    {
        // Return to the pool if it exists, otherwise fallback to just deactivating
        if (ProjectilePool.Instance != null)
        {
            ProjectilePool.Instance.ReturnProjectile(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // If your projectile hits a target, call ReturnToPool() inside your OnTriggerEnter/OnCollisionEnter
}
