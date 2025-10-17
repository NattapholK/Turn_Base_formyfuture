using UnityEngine;

public class GlobalWindController : MonoBehaviour
{
    [Header("Player ที่จะใช้คำนวณระยะ")]
    [SerializeField] private Transform player;

    [Header("ตั้งค่าลม")]
    [Range(0f, 1f)] public float baseWind = 0.25f;
    [Range(0f, 1f)] public float maxWind = 1.0f;
    [Range(1f, 200f)] public float windRadius = 50f;

    void Update()
    {
        if (player == null) return;

        // ส่งค่าให้ Shader ทุกตัวในฉาก
        Shader.SetGlobalVector("_PlayerPosition", player.position);
        Shader.SetGlobalFloat("_BaseWindStrength", baseWind);
        Shader.SetGlobalFloat("_MaxWindStrength", maxWind);
        Shader.SetGlobalFloat("_WindRadius", windRadius);
    }
}
