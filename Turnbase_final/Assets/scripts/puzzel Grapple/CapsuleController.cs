using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CapsuleController : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraTransform;           // ลาก Main Camera มา

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed  = 8f;
    public float rotationSpeed = 12f;

    [Header("Jump/Gravity")]
    public float jumpHeight = 2f;
    public float gravity    = -9.81f;

    [Header("Bounce")]
    [Tooltip("ความเร็วขึ้นตอนเด้ง ถ้าเห็ดไม่ส่งมาก็ใช้ค่านี้")]
    public float defaultBounceUpSpeed = 15f;
    [Tooltip("เปิดกันเด้งซ้ำหลายครั้งในเฟรมเดียว")]
    public float bounceCooldown = 0.05f;

    // --- internal ---
    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;      // เก็บความเร็วแนวดิ่ง (และใช้ Move ในขั้นตอนสุดท้าย)
    private bool isGrounded;

    // คิวเด้ง (กันโดน isGrounded รีเซ็ตความเร็วทับ)
    private bool   bounceQueued = false;
    private float  queuedBounceSpeed = 0f;
    private float  lastBounceTime = -999f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator   = GetComponent<Animator>();
    }

    void Update()
    {
        // 1) Ground check
        isGrounded = controller.isGrounded;

        // ถ้ามีคิวเด้ง → ตั้งความเร็วขึ้นและกันไม่ให้ snap ลงพื้นในเฟรมนี้
        if (bounceQueued)
        {
            velocity.y = queuedBounceSpeed;
            bounceQueued = false;
            // ทำให้เฟรมนี้ไม่ถูกรีเซ็ตความเร็วลง (-2) เพราะถือว่าเราเพิ่งดีดตัวขึ้น
        }
        else
        {
            // snap ลงเล็กน้อยเฉพาะกรณี grounded และกำลังลง
            if (isGrounded && velocity.y < 0f)
                velocity.y = -2f;
        }

        // 2) Inputs
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");
        bool  running = Input.GetKey(KeyCode.LeftShift);

        // 3) ทำทิศอิงกล้อง
        Vector3 camForward = Vector3.forward, camRight = Vector3.right;
        if (cameraTransform != null)
        {
            Vector3 f = cameraTransform.forward; f.y = 0f; f.Normalize();
            Vector3 r = cameraTransform.right;   r.y = 0f; r.Normalize();
            camForward = f; camRight = r;
        }

        Vector3 moveDir = (camForward * inputV + camRight * inputH);
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        // 4) เดิน/วิ่ง
        float speed = running ? runSpeed : walkSpeed;
        controller.Move(moveDir * speed * Time.deltaTime);

        // 5) หมุนตัวตามทิศเคลื่อน
        if (moveDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // 6) Jump (กด Space)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (animator) animator.SetTrigger("Jump");
        }

        // 7) Gravity
        velocity.y += gravity * Time.deltaTime;

        // 8) Move โดยใช้ความเร็วแนวดิ่ง
        controller.Move(velocity * Time.deltaTime);

        // 9) Animator params
        if (animator)
        {
            animator.SetFloat("Input X", inputH);
            animator.SetFloat("Input Z", inputV);
            animator.SetBool("Moving", moveDir.sqrMagnitude > 0.01f);
            animator.SetBool("Running", running);
        }
    }

    /// <summary>
    /// เรียกให้เด้งจากวัตถุภายนอก (เช่น เห็ด)
    /// ใช้ “ความเร็วขึ้นทันที” แทนแรง เพราะ CharacterController ไม่ใช้ฟิสิกส์ Rigidbody
    /// </summary>
    public void Bounce(float upSpeed)
    {
        // กันเด้งถี่เกินไปในเฟรม/ช่วงเวลาสั้นมาก
        if (Time.time - lastBounceTime < bounceCooldown) return;

        lastBounceTime = Time.time;
        bounceQueued = true;
        queuedBounceSpeed = (upSpeed > 0f ? upSpeed : defaultBounceUpSpeed);
    }

    /// <summary>
    /// ตรวจชนของ CharacterController (เรียกทุกครั้งที่ CC ชนอะไรตอน Move)
    /// ใช้จับเคส “เหยียบด้านบนเห็ด” -> เด้ง
    /// </summary>
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // ต้องชนสิ่งที่เป็น "เห็ด" (แท็กหรือมีคอมโพเนนต์ MushroomBounce)
        bool isMushroom =
            hit.collider.CompareTag("Mushroom") ||
            hit.collider.GetComponent<MushroomBounce>() != null;

        if (!isMushroom) return;

        // เงื่อนไข: ชนจาก “ด้านบน” ของเห็ด + เรากำลังตกลงมา
        // normal ของพื้นยอดเห็ดจะชี้ขึ้น (0,1,0) → dot กับ Vector3.up สูง
        bool hitTopSurface = hit.normal.y > 0.6f;
        bool fallingDown   = velocity.y <= 0f;

        if (hitTopSurface && fallingDown)
        {
            // ถ้าเห็ดส่งความแรงมาให้ก็ใช้ ไม่งั้นใช้ค่า default
            float upSpeed = defaultBounceUpSpeed;

            // แจ้งเห็ดให้เล่นเอฟเฟกต์
            var mush = hit.collider.GetComponent<MushroomBounce>();
            if (mush != null)
            {
                // ให้ฝั่งเห็ดทราบว่า CC มาเหยียบ (เล่นอนิเมชัน/พาร์ติเคิล/เสียง)
                mush.BounceFromCharacterController(gameObject);
            }

            // คิวเด้ง (ไปตั้งความเร็วขึ้นในเฟรมนี้ก่อนจะโดน snap)
            Bounce(upSpeed);
        }
    }
}
