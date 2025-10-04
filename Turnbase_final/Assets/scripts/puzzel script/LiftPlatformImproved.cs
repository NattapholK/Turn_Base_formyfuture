using UnityEngine;

/// <summary>
/// ลิฟต์แพลตฟอร์มที่ยกขึ้นเมื่อมี Player ยืนอยู่บน
/// ใช้ระบบ OverlapBox สำหรับตรวจจับแทน Trigger เพื่อความแม่นยำ (เวอร์ชันปรับปรุง)
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class LiftPlatformImproved : MonoBehaviour
{
    [Header("Lift Movement Settings")]
    [Tooltip("ระยะที่ลิฟต์จะเคลื่อนที่ขึ้น")]
    public Vector3 moveOffset = new Vector3(0, 5f, 0);

    [Tooltip("ความเร็วในการเคลื่อนที่ขึ้น")]
    public float upSpeed = 2f;

    [Tooltip("ความเร็วในการเคลื่อนที่ลง")]
    public float downSpeed = 2f;

    [Tooltip("หน่วงเวลาก่อนเคลื่อนที่ขึ้น")]
    public float delayBeforeUp = 0.0f;

    [Tooltip("หน่วงเวลาก่อนเคลื่อนที่ลง")]
    public float delayBeforeDown = 0.15f;

    [Header("Player Detection Settings")]
    [Tooltip("Layer ของ Player (แนะนำให้ตั้งเป็น Player Layer)")]
    public LayerMask playerLayer = ~0;

    [Tooltip("ความหนาของกล่องตรวจจับเหนือผิวลิฟต์")]
    public float detectionThickness = 0.15f;

    [Tooltip("ระยะยกกล่องตรวจจับขึ้นจากผิวบน")]
    public float detectionOffset = 0.03f;

    [Tooltip("ขยายกล่องตรวจจับรอบด้าน (X, Z)")]
    public Vector2 detectionPadding = new Vector2(0.05f, 0.05f);

    [Header("Rider Management")]
    [Tooltip("ทำให้ Player เป็นลูกของลิฟต์เพื่อป้องกันการไถล")]
    public bool parentPlayerOnLift = true;

    [Header("Debug Visualization")]
    [Tooltip("แสดง Gizmo ในโหมด Play")]
    public bool showDebugGizmos = true;

    // === Internal Variables ===
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float upTimer;
    private float downTimer;
    private bool hasPlayerOnTop;

    // Rider tracking
    private Transform currentRider;
    private Transform riderOriginalParent;

    // Components
    private Collider platformCollider;
    private Rigidbody platformRigidbody;

    #region Unity Lifecycle

    void Reset()
    {
        SetupCollider();
        SetupRigidbody();
    }

    void Awake()
    {
        SetupCollider();
        SetupRigidbody();
        
        startPosition = transform.position;
        targetPosition = startPosition + moveOffset;
    }

    // [ปรับปรุง] ใช้ FixedUpdate สำหรับการเคลื่อนที่ที่เกี่ยวกับ Physics (Rigidbody)
    void FixedUpdate() 
    {
        // ตรวจจับ Player
        hasPlayerOnTop = DetectPlayerOnTop(out Transform detectedRider);

        // จัดการ Rider Parenting
        HandleRiderParenting(detectedRider);

        // เคลื่อนที่ลิฟต์
        MoveLift();
    }
    
    // [ปรับปรุง] ใช้ Update สำหรับ Logic ที่ไม่เกี่ยวกับ Physics เช่น Timer
    void Update() 
    {
        // จัดการ Timer
        UpdateTimers();
    }

    void OnDestroy()
    {
        UnparentRider();
    }

    #endregion

    #region Setup Methods

    private void SetupCollider()
    {
        if (platformCollider == null)
            platformCollider = GetComponent<Collider>();
            
        if (platformCollider != null)
            platformCollider.isTrigger = false;
    }

    private void SetupRigidbody()
    {
        if (!TryGetComponent<Rigidbody>(out platformRigidbody))
        {
            platformRigidbody = gameObject.AddComponent<Rigidbody>();
        }
        
        platformRigidbody.isKinematic = true;
        platformRigidbody.useGravity = false;
        platformRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
    }

    #endregion

    #region Player Detection

    private bool DetectPlayerOnTop(out Transform rider)
    {
        rider = null;

        if (platformCollider == null)
            return false;

        Bounds bounds = platformCollider.bounds;
        Vector3 halfExtents = CalculateDetectionBoxHalfExtents(bounds);
        Vector3 center = CalculateDetectionBoxCenter(bounds, halfExtents);

        Collider[] hits = Physics.OverlapBox(
            center, 
            halfExtents, 
            Quaternion.identity, 
            playerLayer, 
            QueryTriggerInteraction.Ignore
        );

        foreach (Collider hit in hits)
        {
            if (IsValidPlayer(hit))
            {
                rider = hit.transform;
                return true; // เจอ Player ที่ถูกต้องแล้ว ให้หยุดค้นหาและคืนค่า true
            }
        }

        return false;
    }

    private Vector3 CalculateDetectionBoxHalfExtents(Bounds bounds)
    {
        return new Vector3(
            bounds.extents.x + detectionPadding.x,
            detectionThickness * 0.5f,
            bounds.extents.z + detectionPadding.y
        );
    }

    private Vector3 CalculateDetectionBoxCenter(Bounds bounds, Vector3 halfExtents)
    {
        return new Vector3(
            bounds.center.x,
            bounds.max.y + halfExtents.y + detectionOffset,
            bounds.center.z
        );
    }

    private bool IsValidPlayer(Collider hit)
    {
        if (hit.transform == transform || hit.transform.IsChildOf(transform))
            return false;
            
        // [สำคัญ] เช็คด้วย Tag "Player" ตามที่โจทย์ต้องการ
        if (!hit.CompareTag("Player"))
            return false;

        // [แก้ไข] เอาการเช็ค isGrounded ออก เพราะจะทำให้ลิฟต์หยุดตอน Player กระโดด
        // แค่เช็คว่ามี CharacterController ก็เพียงพอ
        if (!hit.TryGetComponent<CharacterController>(out _))
            return false;

        return true;
    }

    #endregion

    #region Timer Management

    private void UpdateTimers()
    {
        if (hasPlayerOnTop)
        {
            downTimer = 0f;
            upTimer += Time.deltaTime;
        }
        else
        {
            upTimer = 0f;
            downTimer += Time.deltaTime;
        }
    }

    #endregion

    #region Rider Parenting

    private void HandleRiderParenting(Transform detectedRider)
    {
        if (!parentPlayerOnLift) return;

        // [ปรับปรุง] ตรรกะจัดการ Rider ให้เข้าใจง่ายขึ้น
        // กรณี: มีผู้เล่นบนลิฟต์ แต่เรายังไม่ได้ Parent ใครไว้ -> แสดงว่าเพิ่งขึ้นมา
        if (hasPlayerOnTop && currentRider == null)
        {
            ParentRider(detectedRider);
        }
        // กรณี: ไม่มีผู้เล่นบนลิฟต์ แต่เรายัง Parent ใครไว้อยู่ -> แสดงว่าลงไปแล้ว
        else if (!hasPlayerOnTop && currentRider != null)
        {
            UnparentRider();
        }
    }

    private void ParentRider(Transform rider)
    {
        if (rider == null) return;

        currentRider = rider;
        riderOriginalParent = currentRider.parent;
        
        // Parent โดยรักษาตำแหน่ง World (worldPositionStays = true)
        currentRider.SetParent(transform, true);
    }

    private void UnparentRider()
    {
        if (currentRider == null) return;

        // คืน Parent เดิม, เช็คก่อนว่า Original Parent ไม่ได้ถูกทำลายไปแล้ว
        currentRider.SetParent(riderOriginalParent, true);
        
        currentRider = null;
        riderOriginalParent = null;
    }

    #endregion

    #region Lift Movement

    private void MoveLift()
    {
        Vector3 targetPos;

        if (hasPlayerOnTop && upTimer >= delayBeforeUp)
        {
            targetPos = Vector3.MoveTowards(
                platformRigidbody.position,
                targetPosition,
                upSpeed * Time.fixedDeltaTime // [ปรับปรุง] ใช้ fixedDeltaTime ใน FixedUpdate
            );
        }
        else if (!hasPlayerOnTop && downTimer >= delayBeforeDown)
        {
            targetPos = Vector3.MoveTowards(
                platformRigidbody.position,
                startPosition,
                downSpeed * Time.fixedDeltaTime // [ปรับปรุง] ใช้ fixedDeltaTime ใน FixedUpdate
            );
        }
        else
        {
            return; // ไม่ต้องเคลื่อนที่
        }
        
        platformRigidbody.MovePosition(targetPos);
    }

    #endregion

    #region Debug Visualization

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;

        // ใช้ GetComponent ตลอดเพื่อให้ทำงานได้ดีใน Editor ที่ไม่ได้กด Play
        Collider currentCollider = GetComponent<Collider>();
        if (currentCollider == null) return;

        DrawDetectionBox(currentCollider);
        DrawLiftPath(currentCollider);
    }

    private void DrawDetectionBox(Collider coll)
    {
        Bounds bounds = coll.bounds;
        Vector3 halfExtents = CalculateDetectionBoxHalfExtents(bounds);
        Vector3 center = CalculateDetectionBoxCenter(bounds, halfExtents);

        Gizmos.color = Application.isPlaying && hasPlayerOnTop ? new Color(0, 1, 0, 0.5f) : new Color(0, 1, 1, 0.35f);
        Gizmos.DrawWireCube(center, halfExtents * 2f);
        
        Gizmos.color = new Color(0, 1, 1, 0.1f);
        Gizmos.DrawCube(center, halfExtents * 2f);
    }

    private void DrawLiftPath(Collider coll)
    {
        Vector3 start = Application.isPlaying ? startPosition : transform.position;
        Vector3 end = Application.isPlaying ? (startPosition + moveOffset) : transform.position + moveOffset;
        Vector3 size = coll.bounds.size;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(start, end);
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawWireCube(start, size);
        Gizmos.color = new Color(1, 1, 0, 0.3f);
        Gizmos.DrawWireCube(end, size);
    }
#endif

    #endregion

    #region Public Methods
    public void ResetToStartPosition()
    {
        UnparentRider();
        if(Application.isPlaying)
        {
            platformRigidbody.position = startPosition;
        }
        else
        {
            transform.position = startPosition;
        }
        upTimer = 0f;
        downTimer = 0f;
    }

    public void SetTargetOffset(Vector3 newOffset)
    {
        moveOffset = newOffset;
        if (Application.isPlaying)
        {
            targetPosition = startPosition + moveOffset;
        }
    }

    public bool IsAtStartPosition()
    {
        return Vector3.Distance(transform.position, startPosition) < 0.01f;
    }

    public bool IsAtTargetPosition()
    {
        return Vector3.Distance(transform.position, targetPosition) < 0.01f;
    }
    #endregion
}