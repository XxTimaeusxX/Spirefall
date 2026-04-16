using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject GameOverMenu;
    public GameObject PauseMenu;

    [Header("Player Reference")]
    public Health PlayerHealth;

    private bool isGameOver = false;
    private bool isPaused = false;

    private void Start()
    {
        AudioManager.PlayMainBGM();
        // 1. Ensure all menus are hidden when the game starts
        if (GameOverMenu != null) GameOverMenu.SetActive(false);
        if (PauseMenu != null) PauseMenu.SetActive(false);

        // 2. Subscribe to the player's death event
        if (PlayerHealth != null)
        {
            PlayerHealth.OnDeath.AddListener(HandlePlayerDeath);
        }
        else
        {
            Debug.LogWarning("PlayerHealth is not assigned in MenuManager!");
        }
    }

    private void Update()
    {
        // Don't allow pausing if the player is already dead
        if (isGameOver) return;

        // Toggle pause menu with the Escape key
        /*   if (Input.GetKeyDown(KeyCode.Escape))
             {
                 if (isPaused)
                 {
                     ResumeGame();
                 }
                 else
                 {
                     PauseGame();
                 }
             }*/
    }

    // --- STATE HANDLING ---

    private void HandlePlayerDeath()
    {
        isGameOver = true;

        // Hide pause menu if it was open during death
        if (PauseMenu != null) PauseMenu.SetActive(false);

        if (GameOverMenu != null) GameOverMenu.SetActive(true);

        FreezeGameLocally();
    }

    public void PauseGame()
    {
        isPaused = true;
        if (PauseMenu != null) PauseMenu.SetActive(true);
        FreezeGameLocally();
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (PauseMenu != null) PauseMenu.SetActive(false);

        Time.timeScale = 1f;
        // Lock the cursor back to the game (Update this depending on your camera/aiming style)
        // Cursor.lockState = CursorLockMode.Locked; 
        // Cursor.visible = false;
    }

    private void FreezeGameLocally()
    {
        Time.timeScale = 0f;

        // Unlock the cursor so the player can click menu buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // --- BUTTON METHODS ---
    public void LoadLevel()
    {
        SceneManager.LoadScene("Level"); // Replace "Level1" with the exact string name of your level scene
    }
    public void RestartGame()
    {
        Time.timeScale = 1f; // Must reset time scale before loading!
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        // Replace "MainMenu" with the exact string name of your main menu scene
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitDesktop()
    {
        Debug.Log("Quitting to Desktop...");
        Application.Quit();
    }
}
