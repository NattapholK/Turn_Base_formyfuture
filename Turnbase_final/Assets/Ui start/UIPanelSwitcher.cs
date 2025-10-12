using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ใส่สคริปต์นี้ไว้ที่ GameObject กลาง (เช่น UIRoot หรือ Canvas)
/// แล้วไปผูกปุ่ม OnClick ให้เรียกฟังก์ชันในนี้ โดยส่ง Panel เป้าหมายเข้ามาได้เลย
/// - ShowPanel(panel): โชว์ panel (และซ่อนตัวอื่นถ้าเปิดโหมด deactivateOthers)
/// - TogglePanel(panel): กดซ้ำสลับเปิด/ปิด
/// - ShowByIndex(i): เลือกจากลิสต์ panels ด้วย index
/// - HideAll(): ซ่อนพาเนลทั้งหมดในลิสต์
/// </summary>
public class UIPanelSwitcher : MonoBehaviour
{
    [Header("Panels ที่อยากจัดการ (ลากมาใส่ได้หลายตัว)")]
    [SerializeField] private List<GameObject> panels = new List<GameObject>();

    [Header("พฤติกรรม")]
    [Tooltip("เปิด Panel ใหม่แล้วให้ปิดตัวอื่นทั้งหมดอัตโนมัติ")]
    public bool deactivateOthers = true;

    [Tooltip("ถ้ากดปุ่มเดิมซ้ำบน Panel ที่เปิดอยู่ จะสลับเป็นปิด")]
    public bool toggleIfActive = true;

    /// <summary>โชว์ panel ที่ส่งมา (ปุ่ม OnClick ส่ง GameObject มาได้)</summary>
    public void ShowPanel(GameObject target)
    {
        if (!target) return;

        // Toggle ถ้าเปิดอยู่และตั้งค่า toggleIfActive
        if (toggleIfActive && target.activeSelf)
        {
            target.SetActive(false);
            return;
        }

        if (deactivateOthers) HideOthers(target);
        target.SetActive(true);
    }

    /// <summary>สลับเปิด/ปิด panel ที่ส่งมา</summary>
    public void TogglePanel(GameObject target)
    {
        if (!target) return;
        bool next = !target.activeSelf;
        if (next && deactivateOthers) HideOthers(target);
        target.SetActive(next);
    }

    /// <summary>เลือกโชว์ panel จากลิสต์ panels ด้วย index (สะดวกกับปุ่มแบบใช้ตัวเลข)</summary>
    public void ShowByIndex(int index)
    {
        if (index < 0 || index >= panels.Count) return;
        var target = panels[index];
        if (!target) return;

        if (toggleIfActive && target.activeSelf)
        {
            target.SetActive(false);
            return;
        }

        if (deactivateOthers) HideOthers(target);
        target.SetActive(true);
    }

    /// <summary>ซ่อนทุก panel ในลิสต์</summary>
    public void HideAll()
    {
        foreach (var p in panels)
            if (p) p.SetActive(false);
    }

    /// <summary>เพิ่ม/ลดรายชื่อ panel ได้จากโค้ด (ออปชัน)</summary>
    public void RegisterPanel(GameObject panel)
    {
        if (panel && !panels.Contains(panel))
            panels.Add(panel);
    }
    public void UnregisterPanel(GameObject panel)
    {
        if (panel) panels.Remove(panel);
    }

    void HideOthers(GameObject except)
    {
        foreach (var p in panels)
        {
            if (!p) continue;
            if (p == except) continue;
            if (p.activeSelf) p.SetActive(false);
        }
    }
}
