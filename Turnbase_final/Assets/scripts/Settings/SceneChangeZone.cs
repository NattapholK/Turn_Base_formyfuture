using UnityEngine;
using System.Collections;

/// <summary>
/// ใส่สคริปต์นี้บน GameObject ในซีน เพื่อทำ “โซนเปลี่ยนซีน”
/// - มี SphereCollider (isTrigger) เป็นเขตตรวจจับ
/// - วาด Gizmos วงกลมสีแดงใน Scene View
/// - เมื่อ Collider ที่มี Tag = playerTag เข้าเขต -> Fade + Load ซีนที่กำหนด
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class SceneChangeZone : MonoBehaviour
{
    [Header("Target Scene")]
    [Tooltip("ชื่อซีนปลายทาง (ต้องใส่ใน Build Settings)")]
    public string targetSceneName = "NextScene";

    [Header("Trigger")]
    [Tooltip("Tag ที่ต้องเป็นของ Player")]
    public string playerTag = "Player";
    [Tooltip("ป้องกันทับซ้อน: สั่งเปลี่ยนซีนได้ครั้งเดียวต่อรอบ")]
    public bool oneShot = true;

    [Header("Fade")]
    public float fadeDuration = 0.5f;
    public Color fadeColor = Color.black;
    public bool loadAsync = true;

    [Header("Gizmos (Editor)")]
    public Color gizmoFillColor = new Color(1f, 0f, 0f, 0.25f);
    public Color gizmoWireColor = new Color(1f, 0f, 0f, 1f);

    bool _used;

    void Reset()
    {
        var col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 1.5f;

        // ให้แน่ใจว่ามี SceneFader ในฉาก (สร้างอัตโนมัติหากไม่มี)
        EnsureFaderExists();
    }

    void Awake()
    {
        // กันลืม ตั้งเป็น Trigger เสมอ
        var col = GetComponent<SphereCollider>();
        col.isTrigger = true;

        EnsureFaderExists();
    }

    void EnsureFaderExists()
    {
        if (SceneFader.Instance == null)
        {
            var go = new GameObject("SceneFader");
            go.AddComponent<SceneFader>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (_used && oneShot) return;
        if (!other.CompareTag(playerTag)) return;

        _used = true;
        StartCoroutine(FadeAndChange());
    }

    IEnumerator FadeAndChange()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("[SceneChangeZone] targetSceneName ว่าง");
            yield break;
        }

        yield return SceneFader.Instance.FadeAndLoad(targetSceneName, fadeDuration, fadeColor, loadAsync);
    }

    // ===== Gizmos =====
    void OnDrawGizmos()
    {
        var col = GetComponent<SphereCollider>();
        float r = col ? col.radius : 1f;
        Gizmos.color = gizmoFillColor;
        Gizmos.DrawSphere(transform.position, r);
        Gizmos.color = gizmoWireColor;
        Gizmos.DrawWireSphere(transform.position, r);
    }
}
