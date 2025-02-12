using UnityEngine;

public class GroundScroller : MonoBehaviour
{
    public float speed = 5f;
    public Transform[] groundPieces; // Assign both ground pieces here in the inspector
    private float groundWidth; // Width of each ground piece

    private bool isMoving = false; // Controls movement

    void Start()
    {
        groundWidth = groundPieces[0].GetComponent<SpriteRenderer>().bounds.size.x; // Get ground width
    }

    void Update()
    {
        if (!isMoving) return;

        foreach (Transform ground in groundPieces)
        {
            // Move each ground piece left
            ground.Translate(Vector2.left * speed * Time.deltaTime);

            // If the ground piece moves past reset point, reposition it to the right
            if (ground.position.x <= -groundWidth)
            {
                float rightMostX = GetRightMostGroundPosition();
                ground.position = new Vector2(rightMostX + groundWidth, ground.position.y);
            }
        }
    }

    float GetRightMostGroundPosition()
    {
        float rightMostX = groundPieces[0].position.x;
        foreach (Transform ground in groundPieces)
        {
            if (ground.position.x > rightMostX)
            {
                rightMostX = ground.position.x;
            }
        }
        return rightMostX;
    }

    public void StartScrolling()
    {
        isMoving = true;
    }

    public void StopScrolling()
    {
        isMoving = false;
    }
}
