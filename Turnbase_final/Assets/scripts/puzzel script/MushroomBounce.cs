using UnityEngine;

public class MushroomBounce : MonoBehaviour
{
    [Header("Bounce Settings")]
    [SerializeField] private float bounceForce = 15f;
    [SerializeField] private float minFallSpeedToBounce = -1f; // ควรเป็นค่าติดลบ เช่น -1f

    [Header("Animation Settings")]
    [SerializeField] private float squashAmount = 0.3f;
    [SerializeField] private float squashDuration = 0.2f;

    [Header("Effects")]
    [SerializeField] private ParticleSystem bounceEffect;
    [SerializeField] private AudioClip bounceSound;

    private Vector3 originalScale;
    private bool isSquashing = false;
    private float squashTimer = 0f;
    private AudioSource audioSource;

    void Start()
    {
        originalScale = transform.localScale;

        // AudioSource
        if (bounceSound != null)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = bounceSound;
            audioSource.playOnAwake = false;
        }

        // ย้ำ: เห็ดต้องมี Collider (Is Trigger = false)
        // และอย่างน้อยอีกฝั่ง (เช่น Player) ต้องมี Rigidbody (isKinematic = false)
    }

    void Update()
    {
        // Squash & Stretch
        if (isSquashing)
        {
            squashTimer += Time.deltaTime;

            if (squashTimer < squashDuration * 0.5f)
            {
                float t = squashTimer / (squashDuration * 0.5f);
                transform.localScale = Vector3.Lerp(
                    originalScale,
                    new Vector3(originalScale.x * 1.2f, originalScale.y * squashAmount, originalScale.z * 1.2f),
                    t
                );
            }
            else if (squashTimer < squashDuration)
            {
                float t = (squashTimer - squashDuration * 0.5f) / (squashDuration * 0.5f);
                transform.localScale = Vector3.Lerp(
                    new Vector3(originalScale.x * 1.2f, originalScale.y * squashAmount, originalScale.z * 1.2f),
                    originalScale,
                    t
                );
            }
            else
            {
                transform.localScale = originalScale;
                isSquashing = false;
                squashTimer = 0f;
            }
        }
    }

    // เคสผู้เล่นเป็น Rigidbody
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        // ชนด้านบน? normal ของ "เห็ด" ชี้ขึ้น (ไปหา player)
        ContactPoint cp = collision.contacts[0];
        if (cp.normal.y <= 0.7f) return;

        Rigidbody playerRb = collision.rigidbody; // เร็วกว่า GetComponent
        if (playerRb != null)
        {
            // ต้องเป็นการตกลงมา (ความเร็วแนวดิ่งติดลบ)
            if (playerRb.linearVelocity.y < minFallSpeedToBounce)
            {
                // reset Y ก่อนเด้ง เพื่อให้ความสูงคงที่
                Vector3 v = playerRb.linearVelocity;
                v.y = 0f;
                playerRb.linearVelocity = v;

                // เด้งขึ้น
                playerRb.AddForce(Vector3.up * bounceForce, ForceMode.VelocityChange);

                TriggerBounceEffects();
            }
        }
        else
        {
            // ผู้เล่นไม่มี Rigidbody → อาจใช้ CC อยู่ ให้ไปใช้ทาง OnControllerColliderHit แทน
        }
    }

    // เคสผู้เล่นเป็น CharacterController (Mushroom จะไม่ได้รับ OnCollisionEnter จาก CC)
    // วิธีที่เสถียรคือให้ "ฝั่ง Player" ตรวจชนแล้วเรียก public API ของเห็ด
    // แต่ถ้าอยากให้เห็ดรองรับเอง ให้ผู้เล่นเรียกฟังก์ชันนี้จาก Player เมื่อชนด้านบนเห็ด
    public void BounceFromCharacterController(GameObject playerGO)
    {
        // สมมุติ Player Script ของคุณมีเมธอด Bounce(float force)
        // playerGO.SendMessage("Bounce", bounceForce, SendMessageOptions.DontRequireReceiver);

        // หรือถ้า Player มีตัวแปรแนวดิ่งเอง ให้แก้ค่าที่นั่น
        // ตัวอย่าง (แก้ตามสคริปต์ของคุณ):
        // var controller = playerGO.GetComponent<CharacterController>();
        // var myMove = playerGO.GetComponent<MyCharacterControllerLike>();
        // if (myMove) myMove.VerticalSpeed = bounceForce;

        TriggerBounceEffects();
    }

    void TriggerBounceEffects()
    {
        // เล่นแอนิเมชัน
        isSquashing = true;
        squashTimer = 0f;

        if (bounceEffect) bounceEffect.Play();
        if (audioSource && bounceSound) audioSource.Play();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.5f, 0.2f);
    }
}
