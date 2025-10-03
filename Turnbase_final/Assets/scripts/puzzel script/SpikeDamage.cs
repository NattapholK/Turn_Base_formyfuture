using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpikeDamage : MonoBehaviour
{
    [Header("Damage")]
    public int damagePerTick = 10;
    [Tooltip("ระยะเวลาในการ “ตอด” ซ้ำขณะยืนทับหนาม")]
    public float tickInterval = 0.25f;

    [Header("Filter")]
    public string playerTag = "Player";   // ให้ Player ตั้ง Tag = "Player"

    // เก็บ cooldown แยกต่อเป้าหมาย
    private readonly Dictionary<PlayerHealth, float> _nextHitTime = new();

    void Reset()
    {
        // ทำให้ collider เป็น trigger อัตโนมัติ
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)  => TryDamage(other);
    void OnTriggerStay(Collider other)   => TryDamage(other);

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            var hp = other.GetComponent<PlayerHealth>();
            if (hp) _nextHitTime.Remove(hp);
        }
    }

    void TryDamage(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        var hp = other.GetComponent<PlayerHealth>();
        if (!hp) return;

        float now = Time.time;
        if (!_nextHitTime.TryGetValue(hp, out float next) || now >= next)
        {
            hp.TakeDamage(damagePerTick);
            _nextHitTime[hp] = now + tickInterval;
        }
    }
}
