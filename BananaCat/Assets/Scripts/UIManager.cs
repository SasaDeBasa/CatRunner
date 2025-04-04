using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//parts of the following code has been generated using AI

public class UIManager : MonoBehaviour
{
    public GameObject leaderboardPanel;
    public GameObject loginPanel;
    public GameObject registerPanel;
    public LeaderboardManager leaderboardManager; // Reference to LeaderboardManager
     public FirebaseAuthManager firebaseAuthManager; // Reference to FirebaseAuthManager
    public Button loginButton;  // Login button in leaderboard
    public Button logoutButton; // Logout button in leaderboard

    void Start()
    {
        ShowLeaderboard(); // Start with the leaderboard panel
    }

    public void ShowLeaderboard()
    {
        firebaseAuthManager.ClearErrorMessages();
        leaderboardPanel.SetActive(false);  // Force refresh
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);

        Invoke(nameof(EnableLeaderboard), 0.2f); // Delay to force UI refresh
    }

    private void EnableLeaderboard()
    {
        leaderboardPanel.SetActive(true);
        LeaderboardManager leaderboardManager = FindObjectOfType<LeaderboardManager>();
        if (leaderboardManager != null)
        {
            leaderboardManager.FetchLeaderboard();  // Force refresh
        }

        UpdateAuthUI(); // Update login/logout button
    }

    public void ShowLogin()
    {
        firebaseAuthManager.ClearErrorMessages();
        leaderboardPanel.SetActive(false);
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }

    public void ShowRegister()
    {
        firebaseAuthManager.ClearErrorMessages();
        leaderboardPanel.SetActive(false);
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }

    public void UpdateAuthUI()
    {
        bool isLoggedIn = !string.IsNullOrEmpty(PlayerPrefs.GetString("FirebaseUserID", ""));

        loginButton.gameObject.SetActive(!isLoggedIn);  // Show login button if NOT logged in
        logoutButton.gameObject.SetActive(isLoggedIn);  // Show logout button if logged in
    }

    public void LoadHomeScene()
    {
        SceneManager.LoadScene("HomePage"); // Switch to the home scene
    }
}
