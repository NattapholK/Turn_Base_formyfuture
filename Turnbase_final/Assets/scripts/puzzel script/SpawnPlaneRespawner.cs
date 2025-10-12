// File: SpawnPlaneRespawner.cs
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SpawnPlaneRespawner : MonoBehaviour
{
    [Header("Player Reference")]
    [Tooltip("ลาก Player มาวางที่นี่ (จำเป็น)")]
    public Transform player;

    [Header("Spawn Anchors (Children)")]
    [Tooltip("ถ้าเปิด จะรวบรวม Child Transforms เป็นจุดเกิดให้อัตโนมัติ")]
    public bool autoCollectChildren = true;
    [Tooltip("ถ้าไม่ auto คุณสามารถกำหนดเองได้")]
    public List<Transform> spawnAnchors = new List<Transform>();

    [Header("Pick Mode")]
    [Tooltip("ลาก Marker ไว้จุดที่อยากให้ Respawn ใกล้ที่สุดบนแท่น")]
    public Transform marker;                  // ใช้เลือก anchor ที่ “ใกล้ที่สุด”
    [Tooltip("ถ้าไม่ตั้ง Marker จะใช้ Anchor ลำดับแรกแทน")]
    public int fallbackAnchorIndex = 0;

    [Header("Positioning")]
    [Tooltip("ยกตัวจากพื้นนิดหน่อยกันจมพื้น")]
    public float liftUpOffset = 0.1f;

    [Header("Reset Options")]
    [Tooltip("รีเซ็ตความเร็ว Rigidbody เมื่อ Respawn")]
    public bool resetRigidbodyVelocity = true;
    [Tooltip("รีเซ็ตทิศกล้อง/หมุนตัวผู้เล่นกลับ 0")]
    public bool resetRotation = false;

    PlayerHealth _hp;
    CharacterController _cc;
    Rigidbody _rb;

    void Reset()
    {
        autoCollectChildren = true;
        liftUpOffset = 0.1f;
        fallbackAnchorIndex = 0;
        resetRigidbodyVelocity = true;
    }

    void Awake()
    {
        if (!player)
        {
            Debug.LogError("[SpawnPlaneRespawner] ต้องกำหนด Player ใน Inspector");
            enabled = false;
            return;
        }

        _hp = player.GetComponent<PlayerHealth>();
        if (!_hp)
        {
            Debug.LogError("[SpawnPlaneRespawner] ไม่พบ PlayerHealth บน Player ที่กำหนด");
            enabled = false;
            return;
        }

        _cc = player.GetComponent<CharacterController>();
        _rb = player.GetComponent<Rigidbody>();

        if (autoCollectChildren)
        {
            CollectChildrenAsAnchors();
        }

        if (spawnAnchors.Count == 0)
        {
            Debug.LogWarning("[SpawnPlaneRespawner] ไม่มีจุดเกิด (child anchors) จะใช้ตำแหน่ง Plane เป็นสำรอง");
        }

        // สมัครรับอีเวนต์ตายจาก PlayerHealth
        _hp.OnDied += HandlePlayerDied;
    }

    void OnDestroy()
    {
        if (_hp != null) _hp.OnDied -= HandlePlayerDied;
    }

    void CollectChildrenAsAnchors()
    {
        spawnAnchors.Clear();
        foreach (Transform t in transform)
        {
            // เก็บทุก child (สามารถซ่อน Gizmo/ชื่อได้ตามใจ)
            spawnAnchors.Add(t);
        }
    }

    void HandlePlayerDied()
    {
        // เลือก Anchor
        Transform anchor = ChooseAnchor();
        Vector3 targetPos = anchor ? anchor.position : transform.position;
        targetPos.y += liftUpOffset;

        // เคลื่อนย้ายแบบปลอดภัย
        SafeTeleportPlayer(targetPos);

        // เติมเลือด + ปลดสถานะตาย
        _hp.ReviveFull();
    }

    Transform ChooseAnchor()
    {
        if (spawnAnchors.Count == 0)
            return null;

        if (marker != null)
        {
            // หา child ที่ใกล้ Markerที่สุด
            float best = float.MaxValue;
            Transform chosen = null;
            foreach (var a in spawnAnchors)
            {
                if (!a) continue;
                float d = (a.position - marker.position).sqrMagnitude;
                if (d < best)
                {
                    best = d;
                    chosen = a;
                }
            }
            if (chosen) return chosen;
        }

        // ถ้าไม่มี Marker หรือหาไม่เจอ ใช้ index สำรอง
        int idx = Mathf.Clamp(fallbackAnchorIndex, 0, spawnAnchors.Count - 1);
        return spawnAnchors[idx];
    }

    void SafeTeleportPlayer(Vector3 targetPos)
    {
        // ปิด CC ชั่วคราวกัน snap/fighting
        if (_cc)
        {
            bool wasEnabled = _cc.enabled;
            _cc.enabled = false;
            player.position = targetPos;
            if (resetRotation) player.rotation = Quaternion.identity;
            _cc.enabled = wasEnabled;
        }
        else
        {
            player.position = targetPos;
            if (resetRotation) player.rotation = Quaternion.identity;
        }

        // ถ้ามี Rigidbody ให้หยุดความเร็ว
        if (_rb && resetRigidbodyVelocity)
        {
            _rb.linearVelocity = Vector3.zero;   // Unity 6 / .velocity ในเวอร์ชันก่อน
            _rb.angularVelocity = Vector3.zero;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // วาด anchor + marker ให้เห็นใน Scene
        Gizmos.matrix = Matrix4x4.identity;

        // Anchors
        if (autoCollectChildren && Application.isEditor && !Application.isPlaying)
        {
            // preview children
            foreach (Transform t in transform)
            {
                DrawAnchorGizmo(t, Color.cyan);
            }
        }
        else
        {
            if (spawnAnchors != null)
                foreach (var a in spawnAnchors)
                    if (a) DrawAnchorGizmo(a, Color.cyan);
        }

        // Marker
        if (marker)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(marker.position, 0.25f);
        }
    }

    void DrawAnchorGizmo(Transform t, Color c)
    {
        Gizmos.color = c;
        Gizmos.DrawWireSphere(t.position, 0.2f);
        Gizmos.DrawLine(t.position, t.position + Vector3.up * 0.5f);
    }
#endif
}
