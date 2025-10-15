using System.Collections.Generic;
using UnityEngine;


public class UIPanelSwitcher : MonoBehaviour
{
    [Header("Panels")]
    [Tooltip("ลาก Panels แต่ละ setting มาใส่")]
    [SerializeField] private List<GameObject> Panels = new List<GameObject>();


    public void OpenPanal(GameObject ob)
    {
        if (ob.activeSelf) //เช็คว่าเปิดอยู่มั้ย
        {
            ob.SetActive(!ob.activeSelf); // ปิดตัวเอง
        }
        else
        {
            HideAll();
            ob.SetActive(!ob.activeSelf); // เปิดตัวเอง
        }
    }

    private void HideAll() //ปิดทั้งหมด
    {
        for (int i = 0; i < Panels.Count; i++)
        {
            Panels[i].SetActive(false);
        }
    }
}
