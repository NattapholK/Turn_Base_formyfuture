using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 100;
    [Tooltip("เวลาล่องหนหลังโดน 1 ครั้ง (กันโดนย้ำ)")]
    public float invincibleTime = 0.5f;

    [Header("Debug/Status")]
    public int currentHP;
    public bool IsDead { get; private set; }

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
            // TODO: ใส้เอฟเฟกต์โดนตี/กระพริบ/เสียง ได้ที่นี่
        }
    }

    public void Heal(int amount)
    {
        if (IsDead) return;
        currentHP = Mathf.Min(maxHP, currentHP + Mathf.Abs(amount));
        Debug.Log($"[PlayerHealth] +{amount} HP => {currentHP}/{maxHP}");
    }

    void Die()
    {
        IsDead = true;
        Debug.Log("[PlayerHealth] DEAD");
        // TODO: ปิดการบังคับ, เล่นอนิเมชันล้ม, เรียก GameOver ฯลฯ
        // ตัวอย่าง:
        // GetComponent<CapsuleController>().enabled = false;
    }
}
