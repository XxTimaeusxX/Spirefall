using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    public int damageAmount = 10;
    public string targetTag = ""; // Set this in inspector

    public bool DestroyOnHit = true; // Optional: Destroy the object after dealing damage
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            // Try to find the Health component on the object we collided with
            Health targetHealth = other.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damageAmount);
                Debug.Log($"Dealt {damageAmount} damage to {other.name}");
            }

          
            if (DestroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}
