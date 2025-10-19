// File: EnemyAIZone.cs
using UnityEngine;

/// <summary>
/// Enemy แบบแคปซูลทดสอบ: มีโซน (Gizmos สีแดง), ไล่/ตีผู้เล่นด้วย OverlapBox, ออกจากโซนจะกลับบ้าน
/// - โจมตีเฉพาะ Target ที่ Tag = playerTag และ/หรืออยู่ใน Layer ที่กำหนด
/// - ตายแล้วไม่เกิดใหม่จนกว่าจะโหลดซีนใหม่
/// - เตรียมพารามิเตอร์ Animator: "IsIdle" (bool), "IsChasing" (bool), "Attack" (trigger)
/// </summary>
[RequireComponent(typeof(EnemyHealth))]
public class EnemyAIZone : MonoBehaviour
{
    public enum State { Idle, Chasing, Returning }

    [Header("Target")]
    [Tooltip("ลาก Player ที่ต้องโจมตี (ถ้าไม่ใส่จะหาตาม Tag ตอน Start)")]
    public Transform playerTarget;
    [Tooltip("Tag ของ Player เพื่อความปลอดภัย")]
    public string playerTag = "Player";
    [Tooltip("เฉพาะเลเยอร์ที่ถือว่าเป็น Player เวลาตรวจ OverlapBox (เลือกเป็น Default ก็ได้)")]
    public LayerMask playerLayers = ~0;

    [Header("Zone (World Space)")]
    [Tooltip("จุดศูนย์กลางโซน (อ้างจากตำแหน่งปัจจุบันเป็นค่าเริ่ม)")]
    public Vector3 zoneCenterOffset = Vector3.zero;
    [Tooltip("ขนาดโซน (Box)")]
    public Vector3 zoneSize = new Vector3(10f, 3f, 10f);

    [Header("Move")]
    public float moveSpeed = 3f;
    public float rotateSpeed = 360f;   // deg/sec
    [Tooltip("ระยะหยุดไม่ให้ชน Player (เริ่มโจมตีใกล้ ๆ)")]
    public float stopDistance = 1.5f;
    [Tooltip("ระยะถือว่ากลับถึงบ้าน")]
    public float homeArriveDistance = 0.2f;

    [Header("Attack (OverlapBox)")]
    public int attackDamage = 10;
    public float attackCooldown = 1.2f;
    [Tooltip("กล่องตรวจโจมตี (ท้องศัตรู) ใน local-space")]
    public Vector3 attackBoxOffset = new Vector3(0f, 0.9f, 0.8f);
    public Vector3 attackBoxSize   = new Vector3(0.8f, 1.0f, 1.0f);

    [Header("Animator (Optional)")]
    public Animator animator;
    public string idleBool   = "IsIdle";
    public string chaseBool  = "IsChasing";
    public string attackTrig = "Attack";

    State _state = State.Idle;
    Vector3 _homePos;
    EnemyHealth _health;
    float _nextAttackTime;

    void Awake()
    {
        _health = GetComponent<EnemyHealth>();
        _homePos = transform.position;

        if (!playerTarget)
        {
            var go = GameObject.FindGameObjectWithTag(playerTag);
            if (go) playerTarget = go.transform;
        }
    }

    void Update()
    {
        if (_health.IsDead()) return;
        if (!playerTarget) { SetState(State.Idle); return; }

        bool playerInZone = IsInZone(playerTarget.position);

        switch (_state)
        {
            case State.Idle:
                if (playerInZone) SetState(State.Chasing);
                break;

            case State.Chasing:
                if (!playerInZone) { SetState(State.Returning); break; }
                ChaseAndMaybeAttack();
                break;

            case State.Returning:
                if (playerInZone) { SetState(State.Chasing); break; }
                ReturnHome();
                break;
        }
    }

    void SetState(State s)
    {
        _state = s;
        if (animator)
        {
            animator.SetBool(idleBool,   s == State.Idle);
            animator.SetBool(chaseBool,  s == State.Chasing);
            // โจมตีใช้ Trigger ตอนตีจริง
        }
    }

    bool IsInZone(Vector3 worldPos)
    {
        // ทำ Box ใน world-space
        var center = _homePos + zoneCenterOffset;
        // แปลง worldPos -> local ของ zone box (แต่เราใช้ AABB world ก็พอ: เช็คด้วย Bounds)
        var half = zoneSize * 0.5f;
        return (worldPos.x >= center.x - half.x && worldPos.x <= center.x + half.x) &&
               (worldPos.y >= center.y - half.y && worldPos.y <= center.y + half.y) &&
               (worldPos.z >= center.z - half.z && worldPos.z <= center.z + half.z);
    }

    void ChaseAndMaybeAttack()
    {
        Vector3 toPlayer = playerTarget.position - transform.position;
        toPlayer.y = 0f;

        // หมุนหาผู้เล่น
        if (toPlayer.sqrMagnitude > 0.0001f)
        {
            Quaternion look = Quaternion.LookRotation(toPlayer.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, look, rotateSpeed * Time.deltaTime);
        }

        float dist = toPlayer.magnitude;

        // เดินเข้าใกล้จนถึงระยะหยุด
        if (dist > stopDistance)
        {
            transform.position += transform.forward * (moveSpeed * Time.deltaTime);
        }
        else
        {
            // ถึงระยะโจมตี -> ลองโจมตีตามคูลดาวน์
            TryAttack();
        }
    }

    void ReturnHome()
    {
        Vector3 toHome = _homePos - transform.position;
        toHome.y = 0f;

        if (toHome.sqrMagnitude <= homeArriveDistance * homeArriveDistance)
        {
            // ถึงบ้าน
            SetState(State.Idle);
            return;
        }

        if (toHome.sqrMagnitude > 0.0001f)
        {
            Quaternion look = Quaternion.LookRotation(toHome.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, look, rotateSpeed * Time.deltaTime);
        }

        transform.position += transform.forward * (moveSpeed * Time.deltaTime);
    }

    void TryAttack()
{
    if (Time.time < _nextAttackTime) return;
    _nextAttackTime = Time.time + attackCooldown;

    if (animator && !string.IsNullOrEmpty(attackTrig))
        animator.SetTrigger(attackTrig);

    Vector3 worldCenter = transform.TransformPoint(attackBoxOffset);
    Quaternion worldRot = transform.rotation;

    // ถ้า Player ใช้ Collider เป็น Trigger และอยากให้โดน ให้เปลี่ยนเป็น Collide
    var hits = Physics.OverlapBox(
        worldCenter, attackBoxSize * 0.5f, worldRot,
        playerLayers, QueryTriggerInteraction.Collide
    );

    foreach (var h in hits)
    {
        if (!h.CompareTag(playerTag)) continue;

        // ลองหา PlayerHealth ก่อน
        var ph = h.GetComponentInParent<PlayerHealth>() ?? h.GetComponent<PlayerHealth>();
        if (ph != null && !ph.IsDead)
        {
            ph.TakeDamage(attackDamage);
            break;
        }

        // สำรอง: รองรับ Health เดิม (เผื่อใช้สคริปต์นั้นกับ enemy อื่น)
        var hp = h.GetComponentInParent<EnemyHealth>() ?? h.GetComponent<EnemyHealth>();
        if (hp != null && !hp.IsDead())
        {
            hp.TakeDamage(attackDamage);
            break;
        }
    }
}


    // ===== Gizmos =====
    void OnDrawGizmosSelected()
    {
        // วาด Zone (แดงโปร่ง)
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Vector3 home = Application.isPlaying ? _homePos : transform.position;
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.DrawCube(home + zoneCenterOffset, zoneSize);
        Gizmos.color = new Color(1f, 0f, 0f, 1f);
        Gizmos.DrawWireCube(home + zoneCenterOffset, zoneSize);

        // วาดกล่องโจมตี (เขียว)
        Gizmos.color = Color.green;
        var worldCenter = transform.TransformPoint(attackBoxOffset);
        var rot = transform.rotation;
        Matrix4x4 old = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(worldCenter, rot, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, attackBoxSize);
        Gizmos.matrix = old;
    }
}
