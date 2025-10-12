using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 100;
    [Tooltip("เวลาล่องหนหลังโดน 1 ครั้ง (กันโดนย้ำ)")]
    public float invincibleTime = 0.5f;

    [Header("Debug/Status")]
    public int currentHP;
    public bool IsDead { get; private set; }

    public event Action OnDied;       // ← เพิ่มอีเวนต์
    public event Action OnHealed;     // (เผื่อไว้ถ้าจะใช้ต่อ)

    float invTimer;

    void Awake()
    {
        currentHP = maxHP;
        IsDead = false;
    }

    void Update()
    {
        if (invTimer > 0f) invTimer -= Time.deltaTime;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        if (invTimer > 0f) return; // ยังล่องหนอยู่

        currentHP = Mathf.Max(0, currentHP - Mathf.Abs(amount));
        invTimer = invincibleTime;

        Debug.Log($"[PlayerHealth] -{amount} HP => {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            Die();
        }
        else
        {
            // TODO: เอฟเฟกต์โดนตี
        }
    }

    public void Heal(int amount)
    {
        if (IsDead) return;
        currentHP = Mathf.Min(maxHP, currentHP + Mathf.Abs(amount));
        OnHealed?.Invoke();
        Debug.Log($"[PlayerHealth] +{amount} HP => {currentHP}/{maxHP}");
    }

    void Die()
    {
        if (IsDead) return;
        IsDead = true;
        Debug.Log("[PlayerHealth] DEAD");
        OnDied?.Invoke();   // ← แจ้งคนที่สมัครรับอีเวนต์
        // TODO: ปิดการบังคับ/เล่นอนิเมชัน ถ้าต้องการ
    }

    /// ใช้ชุบชีวิต + เติมเลือดเต็ม สำหรับระบบ Respawn
    public void ReviveFull()
    {
        currentHP = maxHP;
        IsDead = false;
        invTimer = 0f;
        Debug.Log("[PlayerHealth] ReviveFull()");
    }
}
