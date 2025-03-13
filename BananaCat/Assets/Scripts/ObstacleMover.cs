using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    public float speed = 5f;
    private bool isGameOver = false;

    void Update()
    {
        if (isGameOver) return;

        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // Destroy obstacle if it moves off-screen
        if (transform.position.x < -10f)
        {
            Destroy(gameObject);
        }

        if (transform.position.x < -10f) // If obstacle moves out of view
        {
            FindObjectOfType<PlayerController>().score += 1; // Increase score
            FindObjectOfType<PlayerController>().UpdateScoreText(); // Update UI
            Destroy(gameObject); // Remove the obstacle
        }
    }

    public void StopMoving()
    {
        isGameOver = true;
    }
}
