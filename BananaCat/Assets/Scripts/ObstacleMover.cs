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
    }

    public void StopMoving()
    {
        isGameOver = true;
    }
}
