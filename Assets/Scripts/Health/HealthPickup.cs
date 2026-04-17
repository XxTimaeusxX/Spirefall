using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public int healAmount = 25;
    public Rigidbody rb;
    public float Speed = 100f;
    public float maxLifetime = 3f;
    public AudioClip pickupSound;
    public GameObject pickupEffect; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // FIX: Move precisely down the global Z-axis towards the player. 
        // This stops the drifting issue when attached to angled polygon faces.
        rb.linearVelocity = Vector3.back * Speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        // SAFETY CHECK: Only allow the player to pick this up
        if (!other.CompareTag("Player") && other.GetComponent<Shipmovement>() == null)
        {
            return; 
        }

        Health targetHealth = other.GetComponent<Health>();

        if (targetHealth != null && targetHealth.currentHealth < targetHealth.maxHealth)
        {
            targetHealth.Heal(healAmount);

            if (pickupSound != null) { AudioManager.PlaySFX(pickupSound); }
            if (pickupEffect != null) { Instantiate(pickupEffect, transform.position, Quaternion.identity); }
            
            gameObject.SetActive(false);
        }
    }
}
