using UnityEngine;

public class GroundScroller : MonoBehaviour
{
    public float groundWidth = 20f;  // Set this to match the exact width of your ground sprite
    private Transform player;
    private bool hasMoved = false;  // Prevents unnecessary repositioning

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player.position.x > transform.position.x + groundWidth && !hasMoved)
        {
            RepositionGround();
            hasMoved = true;
        }
        else if (player.position.x < transform.position.x)  // Reset if player is behind
        {
            hasMoved = false;
        }
    }
    void RepositionGround()
    {
        GameObject farthestGround = FindFarthestGround();
        float newX = farthestGround.transform.position.x + groundWidth;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    GameObject FindFarthestGround()
    {
        GameObject[] grounds = GameObject.FindGameObjectsWithTag("Ground"); // Make sure both grounds have the "Ground" tag
        GameObject farthest = grounds[0];

        foreach (GameObject ground in grounds)
        {
            if (ground.transform.position.x > farthest.transform.position.x)
            {
                farthest = ground;
            }
        }
        return farthest;
    }
}
