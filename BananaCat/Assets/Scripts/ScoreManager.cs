using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

//parts of the following code has been generated using AI

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager instance;
    public static ScoreManager Instance
    {
        get { return instance; }
    }

    private string firebaseDatabaseURL = "https://catrunner-2e1ee-default-rtdb.firebaseio.com/";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    public void SubmitScore(int score)
    {
        UpdateScore(score);
    }

    public void UpdateScore(int newScore)
    {
        StartCoroutine(UpdatePlayerScore(newScore));
    }

    IEnumerator UpdatePlayerScore(int newScore)
    {
        string userId = PlayerPrefs.GetString("FirebaseUserID");
        string username = PlayerPrefs.GetString("Username");

        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogWarning("No user logged in, skipping score update.");
            yield break;
        }

        string url = $"{firebaseDatabaseURL}/leaderboard/{userId}.json";  // Firebase URL with user ID

        // Fetch the current score from Firebase
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Parse the current score from Firebase
                var currentScoreData = request.downloadHandler.text;
                int currentScore = 0;

                if (!string.IsNullOrEmpty(currentScoreData) && currentScoreData != "null")
                {
                    var currentData = JsonUtility.FromJson<ScoreData>(currentScoreData);
                    currentScore = currentData.score;
                }

                // Only update the score if the new score is higher
                if (newScore > currentScore)
                {
                    // Prepare the data to update the score
                    string json = $"{{\"username\": \"{username}\", \"score\": {newScore}}}";

                    // Update the score in Firebase
                    using (UnityWebRequest updateRequest = UnityWebRequest.Put(url, json))
                    {
                        updateRequest.method = "PATCH";
                        updateRequest.SetRequestHeader("Content-Type", "application/json");
                        yield return updateRequest.SendWebRequest();

                        if (updateRequest.result == UnityWebRequest.Result.Success)
                        {
                            Debug.Log("Score updated successfully!");
                        }
                        else
                        {
                            Debug.LogError("Failed to update score: " + updateRequest.error);
                        }
                    }
                }
                else
                {
                    Debug.Log("New score is not higher than the current score, not updating.");
                }
            }
            else
            {
                Debug.LogError("Failed to fetch current score: " + request.error);
            }
        }
    }

     public string GetDatabaseURL()
    {
        return firebaseDatabaseURL;
    }


}
[System.Serializable]
public class ScoreData
{
    public string username;
    public int score;
}
