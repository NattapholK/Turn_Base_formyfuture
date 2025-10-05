// File: DamageZone.cs
using System.Collections.Generic;
using UnityEngine;

/// วางสคริปต์นี้บน GameObject ที่ทำหน้าที่เป็น "โซนรับดาเมจ"
/// - จะบังคับให้มี Collider และตั้งเป็น isTrigger อัตโนมัติ
/// - โดนเฉพาะวัตถุที่มี PlayerHealth (โดยตรง) + ตรงกับ Layer/Tag ที่กำหนด
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class DamageZone : MonoBehaviour
{
    [Header("Filter")]
    [Tooltip("กำหนด Tag ของ Player (ปล่อยว่างถ้าไม่อยากใช้ตัวกรอง Tag)")]
    public string playerTag = "Player";
    [Tooltip("กรองด้วย LayerMask เพิ่มเติม (ปล่อยว่างให้เป็น Everything ถ้าไม่ใช้)")]
    public LayerMask layers = ~0;

    [Header("Damage")]
    [Tooltip("ดาเมจครั้งเดียวทันทีตอนเข้าโซน")]
    public float damageOnEnter = 0f;
    [Tooltip("ดาเมจต่อวินาที ขณะยืนในโซน")]
    public float damagePerSecond = 10f;
    [Tooltip("เวลาระหว่างการติ๊กดาเมจ (วินาที) เช่น 0.25 = โดน 4 ครั้ง/วิ")]
    [Min(0.05f)] public float tickInterval = 0.25f;

    [Header("Options")]
    [Tooltip("ให้ทำงานเฉพาะกับคอมโพเนนต์ PlayerHealth ที่อยู่บน root ของตัวผู้เล่น")]
    public bool findOnRoot = true;
    [Tooltip("ไม่ลดซ้ำในเฟรมเดียวกันเมื่อเข้า-ออกถี่ ๆ")]
    public bool preventDoubleHitInSameFrame = true;

    // เก็บเวลา next tick ของแต่ละ PlayerHealth
    private readonly Dictionary<PlayerHealth, float> _nextTickAt = new();
    private Collider _col;

    void Reset()
    {
        _col = GetComponent<Collider>();
        if (_col)
        {
            _col.isTrigger = true;
            // ถ้ายังเป็นคอลลิเดอร์เล็ก ๆ ให้เดาง่าย ๆ เป็นโซนขนาด 2x2x2
            if (_col is BoxCollider b && b.size == Vector3.one)
                b.size = new Vector3(2, 2, 2);
        }
    }

    void Awake()
    {
        _col = GetComponent<Collider>();
        _col.isTrigger = true; // สำคัญ: ใช้เป็นโซน ไม่ชนจริงเพื่อลด jitter
    }

    void OnTriggerEnter(Collider other)
    {
        if (!PassFilter(other)) return;

        var hp = GetPlayerHealth(other);
        if (!hp) return;

        if (preventDoubleHitInSameFrame && _nextTickAt.ContainsKey(hp))
            return;

        if (damageOnEnter > 0f)
        {
            hp.TakeDamage((int)damageOnEnter);
        }

        // ตั้งเวลาติ๊กดาเมจครั้งถัดไป
        var now = Time.time;
        if (!_nextTickAt.ContainsKey(hp))
            _nextTickAt.Add(hp, now + tickInterval);
        else
            _nextTickAt[hp] = now + tickInterval;
    }

    void OnTriggerStay(Collider other)
    {
        if (!PassFilter(other)) return;

        var hp = GetPlayerHealth(other);
        if (!hp) return;

        if (!_nextTickAt.TryGetValue(hp, out float next)) 
        {
            _nextTickAt[hp] = Time.time + tickInterval;
            return;
        }

        if (damagePerSecond > 0f && Time.time >= next)
        {
            // คิดดาเมจตาม dps ต่อ 1 tick
            float dmg = damagePerSecond * tickInterval;
            hp.TakeDamage((int)dmg);

            // นัดรอบถัดไป
            _nextTickAt[hp] = Time.time + tickInterval;
        }
    }

    void OnTriggerExit(Collider other)
    {
        var hp = GetPlayerHealth(other);
        if (hp) _nextTickAt.Remove(hp);
    }

    private bool PassFilter(Collider other)
    {
        // Layer filter
        if (((1 << other.gameObject.layer) & layers) == 0) return false;
        // Tag filter (ถ้าใส่)
        if (!string.IsNullOrEmpty(playerTag) && !other.CompareTag(playerTag)) return false;
        return true;
    }

    private PlayerHealth GetPlayerHealth(Collider other)
    {
        if (findOnRoot)
        {
            var root = other.attachedRigidbody ? other.attachedRigidbody.transform.root : other.transform.root;
            return root.GetComponent<PlayerHealth>();
        }
        else
        {
            // หาเฉพาะที่ชนมา (รวมถึงใน parent)
            return other.GetComponentInParent<PlayerHealth>();
        }
    }

#if UNITY_EDITOR
    [Header("Gizmos")]
    public Color gizmoColor = new Color(1f, 0.2f, 0.2f, 0.25f);

    void OnDrawGizmos()
    {
        var c = gizmoColor;
        Gizmos.color = c;
        if (TryGetComponent<BoxCollider>(out var box))
        {
            var m = transform.localToWorldMatrix;
            Gizmos.matrix = m;
            Gizmos.DrawCube(box.center, box.size);
            Gizmos.color = new Color(c.r, c.g, c.b, 0.9f);
            Gizmos.DrawWireCube(box.center, box.size);
        }
    }
#endif
}
