// File: SpawnToggleOnHead.cs
using UnityEngine;

public class SpawnToggleOnHead : MonoBehaviour
{
    [Header("What to spawn")]
    public GameObject prefab;                 // เลือก Prefab ใน Inspector

    [Header("Where to spawn (child marker)")]
    public Transform spawnPoint;              // ลากจุดลูก (เช่น HeadSpawn) มาวาง

    [Header("Options")]
    public KeyCode toggleKey = KeyCode.Q;     // ปุ่มกดสลับ
    public bool parentToPlayer = true;        // ให้ตัวที่เกิดเป็นลูกของ Player ไหม
    public bool matchRotation = true;         // ให้หมุนตาม spawnPoint ไหม
    public bool matchScale = false;           // ให้สเกลตาม spawnPoint ไหม

    // runtime
    private GameObject _spawned;

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleSpawn();
        }
    }

    public void ToggleSpawn()
    {
        if (_spawned == null)
        {
            Spawn();
        }
        else
        {
            Despawn();
        }
    }

    private void Spawn()
    {
        if (prefab == null)
        {
            Debug.LogWarning("[SpawnToggleOnHead] Prefab ยังไม่ถูกตั้งค่า");
            return;
        }

        // ถ้าไม่ได้ตั้ง spawnPoint จะใช้ตำแหน่งของ player แทน
        Vector3 pos = (spawnPoint != null) ? spawnPoint.position : transform.position + Vector3.up * 1.5f;
        Quaternion rot = (spawnPoint != null && matchRotation) ? spawnPoint.rotation : Quaternion.identity;

        _spawned = Instantiate(prefab, pos, rot);

        if (parentToPlayer)
        {
            Transform parent = (spawnPoint != null) ? spawnPoint : transform;
            _spawned.transform.SetParent(parent, worldPositionStays: true);

            if (matchScale && spawnPoint != null)
                _spawned.transform.localScale = spawnPoint.lossyScale;
        }
    }

    private void Despawn()
    {
        if (_spawned != null)
        {
            Destroy(_spawned);
            _spawned = null;
        }
    }

#if UNITY_EDITOR
    // วาดจุดใน Scene ให้เห็นตำแหน่ง spawn
    private void OnDrawGizmosSelected()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(spawnPoint.position, 0.1f);
            Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + spawnPoint.up * 0.3f);
        }
    }
#endif
}
