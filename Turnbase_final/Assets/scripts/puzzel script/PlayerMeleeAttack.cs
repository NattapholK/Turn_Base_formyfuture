// File: PlayerMeleeAttack.cs
using UnityEngine;

/// <summary>
/// สคริปต์ตีระยะประชิดแบบง่าย ๆ สำหรับเทส: กด Mouse0 เพื่อตีด้วย OverlapBox ข้างหน้า
/// ตีเฉพาะเป้าหมายที่มี Tag = enemyTag และมี Health
/// </summary>
public class PlayerMeleeAttack : MonoBehaviour
{
    public string enemyTag = "Enemy";
    public LayerMask enemyLayers = ~0;

    [Header("Hit Box (local-space)")]
    public Vector3 hitBoxOffset = new Vector3(0f, 0.9f, 0.9f);
    public Vector3 hitBoxSize   = new Vector3(1.0f, 1.0f, 1.0f);

    public int damage = 15;
    public float attackCooldown = 0.6f;

    float _nextTime;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= _nextTime)
        {
            _nextTime = Time.time + attackCooldown;

            Vector3 worldCenter = transform.TransformPoint(hitBoxOffset);
            Quaternion worldRot = transform.rotation;
            var hits = Physics.OverlapBox(worldCenter, hitBoxSize * 0.5f, worldRot, enemyLayers, QueryTriggerInteraction.Ignore);

            foreach (var h in hits)
            {
                if (!h.CompareTag(enemyTag)) continue;
                var hp = h.GetComponentInParent<EnemyHealth>() ?? h.GetComponent<EnemyHealth>();
                if (hp != null && !hp.IsDead()) hp.TakeDamage(damage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        var worldCenter = transform.TransformPoint(hitBoxOffset);
        var rot = transform.rotation;
        var old = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(worldCenter, rot, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, hitBoxSize);
        Gizmos.matrix = old;
    }
}
