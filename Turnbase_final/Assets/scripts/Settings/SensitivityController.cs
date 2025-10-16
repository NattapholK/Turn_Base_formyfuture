using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SensitivityController : MonoBehaviour
{
    [Header("Sensitve Slider")]
    [Tooltip("ลาก slider มา")]
    public Slider sensitiveSlider;
    [Tooltip("text ที่ไว้อัพเดตค่า sensitive ให้เห็น")]
    public TextMeshProUGUI senText;

    [Header("Cinemachine Object")]
    [Tooltip("ลาก cinemachine ของแต่ละ scene มา")]
    public GameObject CinemachineObject;
    private CinemachineInputAxisController cinemachine;

    [Header("ManagerValue")]
    public ManagerValue managerValue;

    void Start()
    {
        if (CinemachineObject != null)
        {
            cinemachine = CinemachineObject.GetComponent<CinemachineInputAxisController>();
        }

        sensitiveSlider.value = (managerValue.sensitiveValue - 0.5f) / 7.5f;

        OnSensitivityChanged(sensitiveSlider.value);

        sensitiveSlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    private void OnSensitivityChanged(float value)
    {
        float sensitive = 7.5f * value + 0.5f;
        managerValue.sensitiveValue = sensitive;
        senText.text = sensitive.ToString("F2");
        // ตรวจสอบว่ามี Controller อยู่หรือไม่ และมีอย่างน้อย 2 แกน (X และ Y)
        if (cinemachine != null && cinemachine.Controllers.Count >= 2)
        {
            cinemachine.Controllers[0].Input.Gain = sensitive;
            cinemachine.Controllers[1].Input.Gain = -sensitive;
        }
    }
}