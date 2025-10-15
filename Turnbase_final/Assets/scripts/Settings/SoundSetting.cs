using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{
    [Header("🎵 AudioSource Object")]
    public GameObject GameAudioObject;
    public GameObject PlayerAudioObject;

    [Header("🎚️ UI Slider")]
    [Tooltip("ลาก Slider มาเชื่อมที่นี่")]
    public Slider GameAudioSlider;
    [Tooltip("ลาก Slider มาเชื่อมที่นี่")]
    public Slider PlayerAudioSlider;

    [Header("Scriptable Object")]
    [Tooltip("เอามาจาก scripts/Settings/ManagerValue.cs")]
    public ManagerValue managerValue;

    private AudioSource GameAudioSource;
    private AudioSource PlayerAudioSource;

    void Awake()
    {
        GameAudioSource = GameAudioObject.GetComponent<AudioSource>();
        PlayerAudioSource = PlayerAudioObject.GetComponent<AudioSource>();
    }
    
    void Start()
    {
        // set ให้ slider มี value เท่าใน managerValue
        GameAudioSlider.value = managerValue.gameSound;
        PlayerAudioSlider.value = managerValue.playerSound;

        // set ให้ AudioSource มี value เท่าใน managerValue
        GameAudioSource.volume = managerValue.gameSound;
        PlayerAudioSource.volume = managerValue.playerSound;

        GameAudioSlider.onValueChanged.AddListener(GameVolumeChanged);
        PlayerAudioSlider.onValueChanged.AddListener(PlayerVolumeChanged);
    }

    private void PlayerVolumeChanged(float value)
    {
        if (PlayerAudioSource != null)
        {
            PlayerAudioSource.volume = value; // เปลี่ยนค่า volume
            managerValue.playerSound = value; // เปลี่ยนค่า managerValue
        }
    }

    private void GameVolumeChanged(float value)
    {
        if (GameAudioSource != null)
        {
            GameAudioSource.volume = value;
            managerValue.gameSound = value;
        }
    }
}
