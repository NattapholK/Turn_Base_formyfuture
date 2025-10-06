// File: Carryable.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Carryable : MonoBehaviour
{
    [Header("Basic")]
    public float massLimit = 999f;
    public bool freezeRotationWhileCarried = true;

    [Header("Offsets")]
    public Vector3 carriedLocalRotation = Vector3.zero;

    Rigidbody _rb;
    readonly List<Collider> _cols = new List<Collider>();
    bool _isCarried = false;

    // backups
    bool _backupUseGravity;
    bool _backupIsKinematic;
    RigidbodyInterpolation _backupInterpolation;
    RigidbodyConstraints _backupConstraints;
    int _originalLayer;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null) Debug.LogWarning($"Carryable '{name}' ต้องมี Rigidbody");

        GetComponentsInChildren(true, _cols);
        if (_cols.Count == 0) Debug.LogWarning($"Carryable '{name}' ต้องมี Collider อย่างน้อย 1 ตัว");
    }

    public bool CanBePicked()
    {
        return !_isCarried && _rb != null && _cols.Count > 0;
    }

    public IEnumerator AttachTo(Transform anchor, Vector3 anchorLocalOffset, float liftDuration, string carriedLayerName)
    {
        if (!CanBePicked()) yield break;

        _isCarried = true;

        // สำรองสถานะ RB
        _backupUseGravity    = _rb.useGravity;
        _backupIsKinematic   = _rb.isKinematic;
        _backupInterpolation = _rb.interpolation;
        _backupConstraints   = _rb.constraints;

        // สลับเลเยอร์ชั่วคราวขณะถือ (กันชน Player ด้วย Layer Matrix)
        _originalLayer = gameObject.layer;
        int carriedLayer = LayerMask.NameToLayer(carriedLayerName);
        if (carriedLayer != -1) gameObject.layer = carriedLayer;

        // ปิดฟิสิกส์ชั่วคราว
        _rb.useGravity = false;
        _rb.isKinematic = true;
        _rb.interpolation = RigidbodyInterpolation.None;
        if (freezeRotationWhileCarried)
            _rb.constraints = RigidbodyConstraints.FreezeRotation;

        // ลากขึ้นจุดยึดแบบนุ่ม
        Transform t = transform;
        Vector3 startPos = t.position;
        Quaternion startRot = t.rotation;

        Vector3 targetPos = anchor.TransformPoint(anchorLocalOffset);
        Quaternion targetRot = anchor.rotation * Quaternion.Euler(carriedLocalRotation);

        float t01 = 0f;
        while (t01 < 1f)
        {
            t01 += (liftDuration <= 0f ? 1f : Time.deltaTime / Mathf.Max(0.0001f, liftDuration));
            float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t01));
            t.position = Vector3.Lerp(startPos, targetPos, k);
            t.rotation = Quaternion.Slerp(startRot, targetRot, k);
            yield return null;
        }

        // ทำให้เป็นลูกของ Anchor
        t.SetParent(anchor, worldPositionStays: true);
        t.position = targetPos;
        t.rotation = targetRot;
    }

    public IEnumerator Detach(Vector3 dropVelocity)
    {
        Transform t = transform;
        t.SetParent(null, worldPositionStays: true);

        // คืนเลเยอร์เดิม
        gameObject.layer = _originalLayer;

        // คืนฟิสิกส์
        _rb.useGravity    = _backupUseGravity;
        _rb.isKinematic   = _backupIsKinematic;
        _rb.interpolation = _backupInterpolation;
        _rb.constraints   = _backupConstraints;

        if (!_rb.isKinematic)
        {
            _rb.velocity = dropVelocity;
            _rb.WakeUp();
        }

        _isCarried = false;
        yield return null;
    }
}
