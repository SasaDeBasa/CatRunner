using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 8f;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Handle jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Handle running animation
        if (isGrounded)
        {
            anim.SetBool("isRunning", true);
        }

        // Move the cat forward
        if (anim.GetBool("isRunning"))
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
    }

    void Jump()
    {
        // Apply jump force
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        anim.SetBool("isJumping", true); // Trigger jumping animation
        isGrounded = false; // No longer grounded while jumping
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isJumping", false); // Trigger landing animation
        }
    }

    // Call this method if you want to trigger the death animation
    public void Die()
    {
        anim.SetBool("isDead", true); // Trigger death animation
        this.enabled = false;  // Disable movement after death
    }
}
