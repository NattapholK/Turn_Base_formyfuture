using UnityEngine;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{
    [Header("🎵 ตั้งค่าเสียง")]
    [Tooltip("ลาก AudioSource ที่ต้องการปรับเสียงมาใส่")]
    public AudioSource targetAudioSource;

    [Header("🎚️ UI Slider สำหรับควบคุม Volume")]
    [Tooltip("ลาก Slider จาก Canvas มาเชื่อมที่นี่")]
    public Slider volumeSlider;

    private void Start()
    {
        if (targetAudioSource == null)
        {
            Debug.LogWarning("⚠️ ยังไม่ได้ใส่ AudioSource ใน SoundSetting!");
        }

        if (volumeSlider == null)
        {
            Debug.LogWarning("⚠️ ยังไม่ได้ใส่ Slider ใน SoundSetting!");
            return;
        }

        // โหลดค่าจาก PlayerPrefs ถ้ามี (จำค่าครั้งก่อน)
        float savedVolume = PlayerPrefs.GetFloat("GameVolume", 1f);
        targetAudioSource.volume = savedVolume;
        volumeSlider.value = savedVolume;

        // ตั้งให้ slider เรียกฟังก์ชันเมื่อมีการเปลี่ยนค่า
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnVolumeChanged(float value)
    {
        if (targetAudioSource != null)
        {
            targetAudioSource.volume = value;
            Debug.Log($"🎚️ Volume set to: {value:F2}");
        }

        // บันทึกค่าไว้เพื่อให้จำได้ตอนเปิดเกมใหม่
        PlayerPrefs.SetFloat("GameVolume", value);
    }
}
