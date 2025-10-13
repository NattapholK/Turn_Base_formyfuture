using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ใส่ไว้ที่ GameObject กลาง (Canvas/UIRoot)
/// - แนะนำให้ผูกปุ่ม OnClick -> ShowExclusive(targetPanel)
/// - จะปิดพาเนลอื่นทั้งหมด แล้วเปิด target ทันที (ไม่ต้องปิดก่อน)
/// - รองรับ Bring To Front สำหรับพาเนลที่วางซ้อนกัน
/// </summary>
public class UIPanelSwitcher : MonoBehaviour
{
    [Header("Panels ที่อยากจัดการ (ลากมาใส่ได้หลายตัว)")]
    [SerializeField] private List<GameObject> panels = new List<GameObject>();

    [Header("พฤติกรรม")]
    [Tooltip("ดึงพาเนลเป้าหมายขึ้นหน้า (SetAsLastSibling) กรณีวางซ้อนกันใน Canvas")]
    public bool bringToFront = true;

    [Tooltip("ถ้า panel เป้าหมายไม่อยู่ในลิสต์ ให้ auto-register เข้าไป")]
    public bool autoRegisterIfMissing = true;

    /// <summary>
    /// แนะนำใช้เมธอดนี้กับปุ่ม: เปิด target และปิดตัวอื่นทั้งหมด
    /// </summary>
    public void ShowExclusive(GameObject target)
    {
        if (!target) return;
        if (autoRegisterIfMissing && !panels.Contains(target))
            panels.Add(target);

        // ปิดตัวอื่นทั้งหมดก่อน
        for (int i = 0; i < panels.Count; i++)
        {
            var p = panels[i];
            if (!p) continue;
            if (p == target) continue;
            if (p.activeSelf) p.SetActive(false);
        }

        // เปิดเป้าหมาย (ไม่สนว่าก่อนหน้าเปิดหรือปิด)
        if (!target.activeSelf) target.SetActive(true);

        // เอาขึ้นหน้า ถ้าวางซ้อนกัน
        if (bringToFront)
            target.transform.SetAsLastSibling();
    }

    /// <summary>
    /// สลับเปิด/ปิด (ยังคงไว้เผื่ออยากใช้เป็น toggle บางปุ่ม)
    /// </summary>
    public void TogglePanel(GameObject target)
    {
        if (!target) return;
        bool next = !target.activeSelf;

        // ถ้าจะเปิดใหม่ ปิดตัวอื่นก่อน
        if (next)
        {
            for (int i = 0; i < panels.Count; i++)
            {
                var p = panels[i];
                if (!p || p == target) continue;
                if (p.activeSelf) p.SetActive(false);
            }
            if (bringToFront) target.transform.SetAsLastSibling();
        }
        target.SetActive(next);
    }

    /// <summary>
    /// เปิดจาก index ในลิสต์แบบ exclusive
    /// </summary>
    public void ShowByIndexExclusive(int index)
    {
        if (index < 0 || index >= panels.Count) return;
        ShowExclusive(panels[index]);
    }

    /// <summary>ซ่อนทุก panel</summary>
    public void HideAll()
    {
        foreach (var p in panels)
            if (p) p.SetActive(false);
    }

    // จัดการลิสต์แบบโค้ด
    public void RegisterPanel(GameObject panel)
    {
        if (panel != null && !panels.Contains(panel))
            panels.Add(panel);
    }
    public void UnregisterPanel(GameObject panel)
    {
        if (panel != null) panels.Remove(panel);
    }
}
