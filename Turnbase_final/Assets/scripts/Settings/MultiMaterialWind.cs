using UnityEngine;

public class MultiMaterialWind : MonoBehaviour
{
    public Material[] materials; // ใส่ Material ทั้งหมดของวัตถุ
    public Transform playerCamera;
    public float fullDistance = 10f;
    public float zeroDistance = 30f;

    void Update()
    {
        // float distance = Vector3.Distance(transform.position, playerCamera.position);
        // float t = Mathf.Clamp01((distance - fullDistance) / (zeroDistance - fullDistance));
        // float windStrength = Mathf.Lerp(0.05f, 0f, t);

        // foreach (var mat in materials)
        // {
        //     mat.SetFloat("_Wind_Strength", 0.05ด);
        // }
    }
}