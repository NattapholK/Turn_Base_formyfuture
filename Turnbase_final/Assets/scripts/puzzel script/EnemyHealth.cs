// File: Health.cs
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 100;
    public int currentHP = 100;

    [Header("Events")]
    public UnityEvent onDamaged;
    public UnityEvent onDied;

    bool _dead;

    void Reset()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        if (_dead) return;
        currentHP = Mathf.Max(0, currentHP - Mathf.Abs(amount));
        onDamaged?.Invoke();

        if (currentHP <= 0) Die();
    }

    public void Heal(int amount)
    {
        if (_dead) return;
        currentHP = Mathf.Min(maxHP, currentHP + Mathf.Abs(amount));
    }

    void Die()
    {
        if (_dead) return;
        _dead = true;
        onDied?.Invoke();

        // ปิดการทำงานหลัก ๆ (ไม่ลบทันที เพื่อให้เล่นอนิเมชัน/เอฟเฟกต์ได้)
        var col = GetComponent<Collider>();
        if (col) col.enabled = false;
        var rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        // ปิดสคริปต์อื่น ๆ ที่อาจทำให้ตัวละครยังขยับ
        var ai = GetComponent<EnemyAIZone>();
        if (ai) ai.enabled = false;

        // ถ้าอยากให้หายไปจริง ๆ ค่อย Destroy ทีหลัง:
        // Destroy(gameObject, 5f);
    }

    public bool IsDead() => _dead;
}
