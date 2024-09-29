using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] private float move_speed = 10f;
    [SerializeField] private float jump_force = 25f;
    [SerializeField] private LayerMask ground_layer;
    private Rigidbody2D rb;
    public static bool should_fall = false;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        // Idk why not
        if (rb == null) {
            Debug.LogError("Player does not have a Rigidbody component. Adding a default one.");
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
    }

    void Update() {
        // Moving left-right logic
        float xInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(xInput * move_speed, rb.velocity.y);

        // Handle falling down from a platform
        if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.Space)) {
            should_fall = true;
            // When rigid body moves slower than Sleep Threshold, it will be set to sleep.
            // This makes sure that a player can fall even if he stands still
            rb.WakeUp();
            return;
        }

        // Handle jump
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded()) {
            rb.velocity = new Vector2(rb.velocity.x, jump_force);
        }
    }

    private bool IsGrounded() {
        float y_distance = 1.1f;  // Player scale is 2, so 1.1f is just at the foot
        float x_distance = 0.6f;  // horizontal offset so that player can jump even if he's barely on a platform

        Vector3 left_pos = new Vector3(transform.position.x - x_distance, transform.position.y);
        Vector3 right_pos = new Vector3(transform.position.x + x_distance, transform.position.y);
        RaycastHit2D hit_left = Physics2D.Raycast(left_pos, Vector2.down, y_distance, ground_layer);
        RaycastHit2D hit_right = Physics2D.Raycast(right_pos, Vector2.down, y_distance, ground_layer);

        return hit_left.collider != null || hit_right.collider != null;
    }
}