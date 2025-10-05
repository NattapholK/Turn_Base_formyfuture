using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CarrotController : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraTransform;       // ถ้าไม่เซ็ต จะหา Camera.main ให้

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed  = 8f;

    [Header("Jump/Gravity")]
    public float jumpHeight = 2f;
    public float gravity    = -9.81f;

    [Header("Bounce")]
    public float defaultBounceUpSpeed = 15f;
    public float bounceCooldown = 0.05f;

    [Header("Visual / Model")]
    public Transform visualRoot;                 // child โมเดล
    public Vector3  visualLocalOffset = Vector3.zero;
    public Vector3  visualBaseEuler   = Vector3.zero; // ออฟเซ็ตแกนพื้นฐาน (แก้นอน/เอียง)

    CharacterController controller;
    Animator animator;
    Vector3 velocity;
    bool isGrounded;

    bool  bounceQueued = false;
    float queuedBounceSpeed = 0f;
    float lastBounceTime = -999f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator   = GetComponentInChildren<Animator>();

        if (!visualRoot)
        {
            var smr = GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr) visualRoot = smr.transform;
            else
            {
                var mr = GetComponentInChildren<MeshRenderer>();
                if (mr) visualRoot = mr.transform;
            }
        }

        // auto-assign main camera ถ้าไม่ได้ลากมา
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // --- Ground / vertical ---
        isGrounded = controller.isGrounded;
        if (bounceQueued) { velocity.y = queuedBounceSpeed; bounceQueued = false; }
        else if (isGrounded && velocity.y < 0f) velocity.y = -2f;

        // --- Inputs (RAW เพื่อไม่ให้หน่วง) ---
        float inputH = Input.GetAxisRaw("Horizontal");
        float inputV = Input.GetAxisRaw("Vertical");
        bool  running = Input.GetKey(KeyCode.LeftShift);

        // --- Camera-relative basis (yaw only) ---
        Vector3 camF, camR;
        GetPlanarCameraBasis(out camF, out camR);

        // --- Desired move direction (บนระนาบ) ---
        Vector3 moveDir = (camF * inputV + camR * inputH);
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        // --- Move ---
        float speed = running ? runSpeed : walkSpeed;
        controller.Move(moveDir * speed * Time.deltaTime);

        // --- Face instantly to move direction (snap yaw) ---
        if (visualRoot) visualRoot.localPosition = visualLocalOffset;
        Quaternion baseRot = Quaternion.Euler(visualBaseEuler);

        if (moveDir.sqrMagnitude > 0.0001f)
        {
            float yaw = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            Quaternion yawOnly = Quaternion.Euler(0f, yaw, 0f);

            if (visualRoot) visualRoot.rotation = yawOnly * baseRot;
            else            transform.rotation  = yawOnly;
        }
        else
        {
            // ล็อกให้ตั้งตรงเมื่อไม่ได้กด (ไม่รับ pitch/roll)
            if (visualRoot)
            {
                float y = visualRoot.eulerAngles.y;
                visualRoot.rotation = Quaternion.Euler(0f, y, 0f) * baseRot;
            }
            else
            {
                float y = transform.eulerAngles.y;
                transform.rotation = Quaternion.Euler(0f, y, 0f);
            }
        }

        // --- Jump & Gravity ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (animator) animator.SetTrigger("Jump");
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // --- Animator ---
        if (animator)
        {
            animator.SetFloat("Input X", inputH);
            animator.SetFloat("Input Z", inputV);
            animator.SetBool("Moving", moveDir.sqrMagnitude > 0.01f);
            animator.SetBool("Running", running);
        }
    }

    // คืนแกนกล้องบนระนาบ (ignore pitch/roll) และกันเคสก้มสุดจนเวกเตอร์เป็นศูนย์
    void GetPlanarCameraBasis(out Vector3 camForward, out Vector3 camRight)
    {
        if (cameraTransform)
        {
            camForward = cameraTransform.forward; camForward.y = 0f;
            camRight   = cameraTransform.right;   camRight.y  = 0f;

            float fMag = camForward.sqrMagnitude;
            float rMag = camRight.sqrMagnitude;

            if (fMag < 1e-4f && rMag < 1e-4f)
            {
                // กล้องก้ม/เงยจัด → ใช้แกนของตัวละครแทนชั่วคราว
                camForward = transform.forward; camForward.y = 0f;
                camRight   = transform.right;   camRight.y  = 0f;
            }
            else
            {
                if (fMag > 1e-6f) camForward.Normalize();
                if (rMag > 1e-6f) camRight.Normalize();
            }
        }
        else
        {
            camForward = Vector3.forward;
            camRight   = Vector3.right;
        }
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