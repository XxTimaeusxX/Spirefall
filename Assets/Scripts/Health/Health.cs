using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthBar; // Optional: Reference to a UI slider for health display

    [Header("Score")]
    public int scoreValue = 0; // Points given when this object dies

    [Header("Audio")]
    public AudioClip takeDamageSound;
    public AudioClip deathSound;

    // Optional: Events to trigger UI updates, death animations, etc.
    public UnityEvent OnDeath; 
    public UnityEvent<int> OnHealthChanged;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }
    public void Heal(int amount)
    {
        // Don't heal if we're dead or already at max health
        if (currentHealth <= 0 || currentHealth >= maxHealth) return;

        currentHealth += amount;

        // Clamp it so it doesn't go over max
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // Update UI and Events
        if (healthBar != null) { healthBar.value = currentHealth; }
        OnHealthChanged?.Invoke(currentHealth);
    }
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if(takeDamageSound != null) { AudioManager.PlaySFX(takeDamageSound); }
        
        if (healthBar != null) { healthBar.value = currentHealth; }
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if(deathSound != null) { AudioManager.PlaySFX(deathSound); }
        if (scoreValue > 0 && ScoreManager.Instance != null) { ScoreManager.Instance.AddScore(scoreValue); }
        OnDeath?.Invoke();
        Destroy(gameObject); // Or disable/pool the object
    }
}
