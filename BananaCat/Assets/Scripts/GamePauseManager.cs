using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePauseManager : MonoBehaviour
{
    public GameObject pausePanel; // Assign in Inspector
    public GameObject pauseButton; // Assign in Inspector

    private bool isPaused = false;

    void Start()
    {
        pausePanel.SetActive(false); // Hide pause panel at start
    }

    //method to toggle pause state is generated
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // Pause the game
            pausePanel.SetActive(true);
            pauseButton.SetActive(false); // Hide pause button
        }
        else
        {
            ResumeGame();
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // Resume game
        pausePanel.SetActive(false);
        pauseButton.SetActive(true); // Show pause button again
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Ensure time is normal before restart
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f; // Ensure time is normal before quitting
        SceneManager.LoadScene("HomePage"); // Change to your main menu scene name
    }
}

