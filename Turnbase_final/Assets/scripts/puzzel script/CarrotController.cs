using UnityEngine;

public class CarrotController : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    [Header("Speed Settings")]
    public float walk = 6f;
    public float run = 12f;
    private float speed;

    [Header("Jump/Gravity")]
    public float jumpHeight = 2f;
    public float gravity    = -9.81f;
    private bool isGrounded;          // เช็คว่าติดพื้น

    [Header("Rotation Settings")]
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    bool  bounceQueued = false;
    float queuedBounceSpeed = 0f;
    float lastBounceTime = -999f;

    [Header("Bounce")]
    public float defaultBounceUpSpeed = 15f;
    public float bounceCooldown = 0.05f;
    Vector3 velocity;

    void Update()
    {
        // --- เช็คว่าติดพื้นมั้ย ---
        isGrounded = controller.isGrounded;
        if (bounceQueued) { velocity.y = queuedBounceSpeed; bounceQueued = false; }
        else if (isGrounded && velocity.y < 0f) velocity.y = -2f;



        // --- เดิน / วิ่ง ---
        if (Input.GetKey(KeyCode.LeftShift))
            speed = run;
        else
            speed = walk;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        // --- Jump & Gravity ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    
    public void Bounce(float upSpeed)
    {
        if (Time.time - lastBounceTime < bounceCooldown) return;
        lastBounceTime = Time.time;
        bounceQueued = true;
        queuedBounceSpeed = (upSpeed > 0f ? upSpeed : defaultBounceUpSpeed);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        bool isMushroom =
            hit.collider.CompareTag("Mushroom") ||
            hit.collider.GetComponent<MushroomBounce>() != null;

        if (!isMushroom) return;

        bool hitTopSurface = hit.normal.y > 0.6f;
        bool fallingDown   = velocity.y <= 0f;

        if (hitTopSurface && fallingDown)
        {
            var mush = hit.collider.GetComponent<MushroomBounce>();
            if (mush != null) mush.BounceFromCharacterController(gameObject);
            Bounce(defaultBounceUpSpeed);
        }
    }
}
