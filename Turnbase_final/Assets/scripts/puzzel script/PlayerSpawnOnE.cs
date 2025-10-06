// File: PlayerSpawnOnE.cs
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerSpawnOnE : MonoBehaviour
{
    [Header("Input")]
    public KeyCode spawnKey = KeyCode.E;
    public float cooldown = 0.2f;

    [Header("Prefab to Spawn")]
    public GameObject prefab;                // ลากพรีแฟบที่อยากให้เกิดมาใส่ตรงนี้

    [Header("Placement")]
    public Transform spawnPoint;             // ถ้ามี ให้สปอว์นที่จุดนี้ (เช่น empty หน้า player)
    public float forwardDistance = 1.2f;     // ถ้าไม่มี spawnPoint จะวางหน้า player ระยะนี้
    public bool snapToGround = true;         // ให้สแน็ปติดพื้นด้วย Raycast ไหม
    public float raycastUpStart = 2f;        // เริ่มยิง Ray ลงจากสูงเท่านี้
    public float raycastDownDist = 6f;       // ระยะยิง Ray ลงหา "พื้น"
    public LayerMask groundMask = ~0;        // เลเยอร์ที่นับเป็นพื้น

    [Header("Safety")]
    public float minClearFromPlayer = 0.3f;  // กันวางทับตัวผู้เล่น

    float _lastSpawn = -999f;
    CharacterController _cc;

    void Awake()
    {
        _cc = GetComponent<CharacterController>();
        if (!prefab)
            Debug.LogWarning($"[{nameof(PlayerSpawnOnE)}] ยังไม่ได้เซ็ต Prefab");
    }

    void Update()
    {
        if (Input.GetKeyDown(spawnKey))
            TrySpawn();
    }

    void TrySpawn()
    {
        if (!prefab) return;
        if (Time.time - _lastSpawn < cooldown) return;

        // 1) หา position/rotation เบื้องต้น
        Vector3 pos;
        Quaternion rot;

        if (spawnPoint) {
            pos = spawnPoint.position;
            rot = spawnPoint.rotation;
        } else {
            // วางหน้า player
            Vector3 fwd = transform.forward; fwd.y = 0f; fwd.Normalize();
            rot = Quaternion.LookRotation(fwd == Vector3.zero ? Vector3.forward : fwd, Vector3.up);

            // จุดเริ่มต้น (กลางลำตัว + ด้านหน้า)
            pos = transform.position + fwd * (forwardDistance + _cc.radius);

            // กันวางชิดตัวเกินไป
            pos += fwd * minClearFromPlayer;

            // ถ้าให้สแน็ปพื้น: ยิง Ray ลงจากด้านบน
            if (snapToGround)
            {
                Vector3 start = pos + Vector3.up * raycastUpStart;
                if (Physics.Raycast(start, Vector3.down, out RaycastHit hit, raycastDownDist, groundMask, QueryTriggerInteraction.Ignore))
                {
                    pos = hit.point;
                }
            }
        }

        // 2) สปอว์น
        Instantiate(prefab, pos, rot);
        _lastSpawn = Time.time;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying && !_cc) _cc = GetComponent<CharacterController>();
        if (!_cc) return;

        Vector3 fwd = transform.forward; fwd.y = 0f; fwd.Normalize();
        Vector3 basePos = transform.position + fwd * (forwardDistance + _cc.radius + minClearFromPlayer);
        Vector3 rayStart = basePos + Vector3.up * raycastUpStart;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(rayStart, rayStart + Vector3.down * raycastDownDist);
        Gizmos.DrawSphere(rayStart, 0.05f);
    }
#endif
}
