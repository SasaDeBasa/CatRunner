using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.SceneManagement;  // To handle scene transitions


//parts of the following code has been generated using AI
public class LeaderboardManager : MonoBehaviour
{
    public GameObject leaderboardEntryPrefab;
    public Transform leaderboardContainer;
    public TextMeshProUGUI playerRankText;
    public GameObject loginPanel;

    private string firebaseDatabaseURL = "https://catrunner-2e1ee-default-rtdb.firebaseio.com/leaderboard.json";
    private string userId;  // Store Firebase User ID

    void Start()
    {
        if (PlayerPrefs.GetInt("ReturningFromGameOver", 0) == 1)
        {
            loginPanel.SetActive(true); // Show login panel
            PlayerPrefs.SetInt("ReturningFromGameOver", 0); // Reset flag so it doesn't open every time
            CheckAndUpdateScore(); // Handle score update logic after login
        }

        if (PlayerPrefs.HasKey("PendingScore"))
        {
            int pendingScore = PlayerPrefs.GetInt("PendingScore");
            ScoreManager.Instance.SubmitScore(pendingScore);
            PlayerPrefs.DeleteKey("PendingScore"); // Remove after updating
        }
        // Fetch Firebase User ID (not email) from PlayerPrefs
        userId = PlayerPrefs.GetString("FirebaseUserID", "");
        Debug.Log("Retrieved Firebase User ID from PlayerPrefs: " + userId);

        FetchLeaderboard();

        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("User ID is empty! Make sure PlayerPrefs is storing the correct Firebase User ID.");
        }
    }

    public void FetchLeaderboard()
    {
        userId = PlayerPrefs.GetString("FirebaseUserID", "");  // Refresh User ID from PlayerPrefs
        Debug.Log("Fetching leaderboard with User ID: " + userId);

        if (string.IsNullOrEmpty(userId))
        {
            playerRankText.text = "Login to track your rank!";
            return;
        }

        StartCoroutine(GetLeaderboardData());
    }

    private IEnumerator GetLeaderboardData()
    {
        Debug.Log("Sending request to: " + firebaseDatabaseURL);

        UnityWebRequest request = UnityWebRequest.Get(firebaseDatabaseURL);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error fetching leaderboard: " + request.error);
            yield break;
        }

        string json = request.downloadHandler.text;
        Debug.Log("Leaderboard Data Received (raw): " + json);  // Log raw JSON response

        if (string.IsNullOrEmpty(json) || json == "null")
        {
            Debug.LogError("Leaderboard data is empty or null!");
            yield break;
        }

        Dictionary<string, LeaderboardEntryData> leaderboardData =
            JsonConvert.DeserializeObject<Dictionary<string, LeaderboardEntryData>>(json);

        if (leaderboardData == null || leaderboardData.Count == 0)
        {
            Debug.LogError("Parsed leaderboard data is null or empty!");
            yield break;
        }

        Debug.Log($"Successfully parsed {leaderboardData.Count} leaderboard entries.");
        DisplayLeaderboard(leaderboardData);
    }

    private void DisplayLeaderboard(Dictionary<string, LeaderboardEntryData> leaderboardData)
    {
        if (leaderboardContainer == null || leaderboardEntryPrefab == null)
        {
            Debug.LogError("Leaderboard UI elements are not assigned!");
            return;
        }

        foreach (Transform child in leaderboardContainer)
        {
            Destroy(child.gameObject);
        }

        int rank = 1;
        bool userFound = false;

        foreach (var entry in leaderboardData.OrderByDescending(e => e.Value.score))
        {
            GameObject entryObject = Instantiate(leaderboardEntryPrefab, leaderboardContainer);
            entryObject.transform.SetParent(leaderboardContainer, false);
            entryObject.transform.localScale = Vector3.one;

            LeaderboardEntry entryScript = entryObject.GetComponent<LeaderboardEntry>();
            if (entryScript == null)
            {
                Debug.LogError("LeaderboardEntry script is missing from prefab!");
                continue;
            }

            entryScript.Setup(rank, entry.Value.username, entry.Value.score);

            // Check User ID instead of email
            if (!string.IsNullOrEmpty(userId) && userId == entry.Key)  // entry.Key is the Firebase User ID
            {
                Debug.Log($"User found in leaderboard! Rank: {rank}");
                playerRankText.text = $"Your Rank: {rank}";
                userFound = true;
            }

            rank++;
        }

        if (!userFound)
        {
            Debug.Log("User ID not found in leaderboard, setting rank text.");
            playerRankText.text = "Your Rank: Not on leaderboard";
        }
    }

    public void ClearLeaderboardUI()
    {
        foreach (Transform child in leaderboardContainer)
        {
            Destroy(child.gameObject);
        }
    }

    void CheckAndUpdateScore()
    {
        if (!PlayerPrefs.HasKey("GameScore")) return; // No score to update

        int currentScore = PlayerPrefs.GetInt("GameScore", 0);
        ScoreManager.Instance.SubmitScore(currentScore);
        PlayerPrefs.DeleteKey("GameScore"); // Remove stored score after updating
    }

    public void RedirectToGame()
    {
        SceneManager.LoadScene("Game"); // Redirect to the game scene
    }
}

// Helper class for JSON data mapping
[System.Serializable]
public class LeaderboardEntryData
{
    public string username;
    public int score;
}
