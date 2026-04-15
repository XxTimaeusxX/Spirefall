using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 20f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        ProjectileMove();
        Destroy(gameObject, 3f);
    }
    public void ProjectileMove()
        {
        rb.linearVelocity = transform.forward * speed;
    }
  /*  private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            
            Destroy(gameObject); // Example: destroy enemy on collision
            Destroy(other.gameObject); // Example: destroy enemy on collision
        }
    }*/
}
