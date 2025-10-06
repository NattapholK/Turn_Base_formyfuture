// File: PlayerCarry.cs
using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerCarry : MonoBehaviour
{
    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Detection (by Tag)")]
    [Tooltip("Tag ของวัตถุที่ยกได้ (ต้องตรงตัวพิมพ์)")]
    public string carryableTag = "carryable";
    public float grabRange = 2.2f;
    public float grabHeightOffset = 0.8f;

    [Header("Hold / Anchor")]
    public Transform holdAnchor;                 // ถ้าเว้นว่าง จะสร้างให้
    public Vector3 holdLocalOffset = Vector3.zero;
    public float liftDuration = 0.25f;           // เวลา 'ดูด' ขึ้นหัว
    public bool keepObjectUpright = true;

    [Header("Drop")]
    public Vector3 dropExtraVelocity = Vector3.zero;

    [Header("Physics Layers (while carried)")]
    [Tooltip("เลเยอร์ที่จะใช้ขณะถือของ (ไปตั้ง Collision Matrix ให้ไม่ชน Player)")]
    public string carriedLayerName = "Carried";

    CharacterController _cc;
    Carryable _current;
    bool _busy;

    void Awake()
    {
        _cc = GetComponent<CharacterController>();

        if (!holdAnchor)
        {
            var go = new GameObject("HoldAnchor");
            holdAnchor = go.transform;
            holdAnchor.SetParent(transform, false);
            float aboveHead = Mathf.Max(1.6f, _cc.height * 0.75f);
            holdAnchor.localPosition = new Vector3(0f, aboveHead, 0.2f);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(interactKey) && !_busy)
        {
            if (_current == null)
            {
                var target = FindNearestCarryable();
                if (target != null && target.CanBePicked())
                    StartCoroutine(AttachRoutine(target));
            }
            else
            {
                StartCoroutine(DetachRoutine());
            }
        }

        if (_current != null && keepObjectUpright)
        {
            // ให้ตั้งตรง (ถ้าอยากให้หันตามหัวผู้เล่นก็เอาบรรทัดนี้ออก)
            _current.transform.rotation = Quaternion.identity;
        }
    }

    Carryable FindNearestCarryable()
    {
        Vector3 center = transform.position + Vector3.up * grabHeightOffset;
        // ไม่ใช้ layermask แล้ว กรองด้วย Tag + Component
        Collider[] hits = Physics.OverlapSphere(center, grabRange);

        var candidates = hits
            .Select(h => h.attachedRigidbody ? h.attachedRigidbody.gameObject : h.gameObject)
            .Select(go => go.GetComponentInParent<Carryable>())
            .Where(c => c != null && c.CanBePicked() && HasCarryableTag(c.gameObject))
            .Distinct()
            .ToList();

        if (candidates.Count == 0) return null;

        Carryable best = null; float bestScore = float.MaxValue;
        foreach (var c in candidates)
        {
            float dist = Vector3.Distance(center, c.transform.position);
            float backPenalty = Vector3.Dot(transform.forward, (c.transform.position - transform.position).normalized) < 0f ? 0.5f : 0f;
            float score = dist + backPenalty;
            if (score < bestScore) { bestScore = score; best = c; }
        }
        return best;
    }

    bool HasCarryableTag(GameObject go)
    {
        // ยอมรับแท็กจากตัวเองหรือพาเรนต์
        return (go.CompareTag(carryableTag) ||
                (go.transform.parent && go.transform.parent.CompareTag(carryableTag)) ||
                (go.transform.root && go.transform.root.CompareTag(carryableTag)));
    }

    IEnumerator AttachRoutine(Carryable target)
    {
        _busy = true;
        try
        {
            yield return StartCoroutine(
                target.AttachTo(holdAnchor, holdLocalOffset, liftDuration, carriedLayerName)
            );
            _current = target;
        }
        finally { _busy = false; }
    }

    IEnumerator DetachRoutine()
    {
        _busy = true;
        try
        {
            if (_current != null)
            {
                Vector3 dropVel = dropExtraVelocity;
                yield return StartCoroutine(_current.Detach(dropVel));
                _current = null;
            }
        }
        finally { _busy = false; }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.25f);
        Vector3 center = transform.position + Vector3.up * grabHeightOffset;
        Gizmos.DrawSphere(center, grabRange);
    }
#endif
}
