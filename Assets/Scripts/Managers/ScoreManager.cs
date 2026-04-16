using UnityEngine;
using UnityEngine.Events;

using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public int currentScore = 0;
    public  TextMeshProUGUI scoreText; // Reference to your UI Text piece

    public UnityEvent<int> OnScoreChanged;

    private void Awake()
    {
        // Simple Singleton Setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI();
        OnScoreChanged?.Invoke(currentScore);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString();
        }
    }
}
