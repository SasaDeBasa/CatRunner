using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement; // Required for SceneManager
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 8f;
    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private bool hasStarted = false;
    private bool isDead = false;
    public int lives = 3; // Player starts with 3 lives
    public GameObject[] hearts; // Assign Heart UI objects in Inspector
    public GameObject mathQuestionPanel; // Math panel
    public GameObject gameOverPanel; // Game Over panel
    public Button restartButton; // Restart button
    public Image questionImage; // UI Image to display the question
    public InputField answerInput; // Input field for the answer
    public int score = 0; // Player's score
    public TMP_Text gameOverStatusText; // Assign in Inspector
    public Button loginButton; // Assign in Inspector
    public Text scoreText; // UI Text to display the score
    private int correctAnswer;

    private GroundScroller groundScroller;
    private ObstacleSpawner obstacleSpawner;
    private string firebaseDatabaseURL;

    //parts of the following code has been generated using AI

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        groundScroller = Object.FindFirstObjectByType<GroundScroller>();
        obstacleSpawner = Object.FindFirstObjectByType<ObstacleSpawner>();
        mathQuestionPanel.SetActive(false); // Hide the panel at start 
        gameOverPanel.SetActive(false); // Hide Game Over panel at start
        restartButton.onClick.AddListener(RestartGame); // Attach Restart function
        UpdateScoreText(); // Ensure score is displayed at start
        if (ScoreManager.Instance != null)
        {
            firebaseDatabaseURL = ScoreManager.Instance.GetDatabaseURL();
        }
        else
        {
            Debug.LogError("ScoreManager not initialized!");
        }
    }

    void Update()
    {
        if (isDead) return; // If the player is dead, no more updates.

        if (!hasStarted && Input.GetKeyDown(KeyCode.Space))
        {
            hasStarted = true;
            anim.SetBool("isRunning", true);
            groundScroller.StartScrolling();
            obstacleSpawner.StartGame();
        }

        if (hasStarted && Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }




    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // Corrected velocity to ensure proper jump behavior
        anim.SetBool("isJumping", true);
        isGrounded = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isJumping", false);
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            // Destroy the obstacle so it doesn't interfere further.
            Destroy(collision.gameObject);
            PauseGame();
            StartCoroutine(FetchMathQuestion());
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f; // Pause the game
        anim.SetBool("isRunning", false);
    }

    void ResumeGame()
    {
        Time.timeScale = 1f; // Resume the game
        anim.SetBool("isRunning", true);
        mathQuestionPanel.SetActive(false); // Hide the question panel
    }

    IEnumerator FetchMathQuestion()
    {
        string apiUrl = "https://marcconrad.com/uob/banana/api.php";
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("API Error: " + request.error);
            yield break;
        }

        // Parse the JSON response
        var json = JsonUtility.FromJson<MathQuestionResponse>(request.downloadHandler.text);
        string imageUrl = json.question;
        correctAnswer = json.solution;

        StartCoroutine(LoadImage(imageUrl));
        mathQuestionPanel.SetActive(true);
    }

    IEnumerator LoadImage(string url)
    {
        UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(url);
        yield return imageRequest.SendWebRequest();

        if (imageRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Image Load Error: " + imageRequest.error);
            yield break;
        }

        Texture2D texture = DownloadHandlerTexture.GetContent(imageRequest);
        questionImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public void CheckAnswer()
    {
        int playerAnswer;
        if (int.TryParse(answerInput.text, out playerAnswer))
        {
            if (playerAnswer == correctAnswer)
            {
                ResumeGame(); // Continue the game
            }
            else
            {
                ReduceLife();
                ResumeGame();
            }
        }
        else
        {
            Debug.Log("Invalid input!");
        }
    }

    void ReduceLife()
    {
        if (lives > 0)
        {
            lives--; // Decrease life count
            hearts[lives].SetActive(false); // Hide the corresponding heart UI
        }

        if (lives <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        isDead = true;
        anim.SetBool("isDead", true);
        mathQuestionPanel.SetActive(false); // Hide math panel
        this.enabled = false; // Disable player movement

        // Stop the game environment
        groundScroller.StopScrolling();
        obstacleSpawner.StopGame();

        // Stop all obstacles
        foreach (ObstacleMover obstacle in FindObjectsOfType<ObstacleMover>())
        {
            obstacle.StopMoving();
        }

        // Save the current score
        PlayerPrefs.SetInt("GameScore", score);

        // Show game over panel with the correct login info
        UpdateGameOverText();
        gameOverPanel.SetActive(true);

        // If logged in, submit the score
        if (PlayerPrefs.HasKey("FirebaseUserID"))
        {
            ScoreManager.Instance.SubmitScore(score);
        }
    }

    public void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the scene
    }

    public void RedirectToLogin()
    {
        PlayerPrefs.SetInt("GameScore", score); // Store the score
        PlayerPrefs.SetInt("ReturningFromGameOver", 1); // Mark that user came from Game Over
        SceneManager.LoadScene("Leaderboard"); // Redirect to leaderboard
    }

    void UpdateGameOverText()
    {
        string userId = PlayerPrefs.GetString("FirebaseUserID", "");
        string username = PlayerPrefs.GetString("Username", "Unknown Player"); // Get username from PlayerPrefs

        if (!string.IsNullOrEmpty(userId))
        {
            // Fetch high score from the leaderboard
            StartCoroutine(FetchHighScoreFromLeaderboard(userId, username));
        }
        else
        {
            gameOverStatusText.text = "Not logged in. Log in to save your progress.";
        }
    }

    private IEnumerator FetchHighScoreFromLeaderboard(string userId, string username)
    {
        string url = $"{firebaseDatabaseURL}/leaderboard/{userId}.json"; // URL to fetch score for the logged-in user

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;

                if (!string.IsNullOrEmpty(response) && response != "null")
                {
                    var playerData = JsonUtility.FromJson<LeaderboardEntryData>(response);
                    int highScore = playerData.score;

                    gameOverStatusText.text = $"Logged in as {username}\nHighest Score: {highScore}";
                }
                else
                {
                    gameOverStatusText.text = $"Logged in as {username}\nNo high score yet.";
                }
            }
            else
            {
                Debug.LogError("Failed to fetch score from leaderboard: " + request.error);
                gameOverStatusText.text = $"Logged in as {username}\nError fetching high score.";
            }
        }
    }


}

// Helper class for JSON parsing
[System.Serializable]
public class MathQuestionResponse
{
    public string question;  // This is the URL of the question image
    public int solution;     // This is the solution (previously 'correctAnswer')
}
