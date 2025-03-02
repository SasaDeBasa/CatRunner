using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 8f;
    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private bool hasStarted = false;
    private bool isDead = false;

    public GameObject mathQuestionPanel; // UI Panel for the question
    public Image questionImage; // UI Image to display the question
    public InputField answerInput; // Input field for the answer
    private int correctAnswer;

    private GroundScroller groundScroller;
    private ObstacleSpawner obstacleSpawner;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        groundScroller = Object.FindFirstObjectByType<GroundScroller>();
        obstacleSpawner = Object.FindFirstObjectByType<ObstacleSpawner>();
        mathQuestionPanel.SetActive(false); // Hide the panel at start
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
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // Corrected linearVelocity to velocity
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
            PauseGame(); // Stop movement
            StartCoroutine(FetchMathQuestion()); // Call API for the question
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
                Die(); // Player loses
            }
        }
        else
        {
            Debug.Log("Invalid input!");
        }
    }

    public void Die()
    {
        anim.SetBool("isDead", true);
        this.enabled = false; // Disable the player controller
        rb.linearVelocity = Vector2.zero; // Stop the player
        groundScroller.StopScrolling(); // Stop the ground
        obstacleSpawner.StopGame(); // Stop the obstacle spawner
    }
}

// Helper class for JSON parsing
[System.Serializable]
public class MathQuestionResponse
{
    public string question;  // This is the URL of the question image
    public int solution;     // This is the solution (previously 'correctAnswer')
}
