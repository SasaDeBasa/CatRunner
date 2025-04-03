using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Game"); // Switches to Game Scene
    }

    public void OpenScoreboard()
    {
        SceneManager.LoadScene("Leaderboard"); // Create this scene later
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
