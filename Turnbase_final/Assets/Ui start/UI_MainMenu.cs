// File: UI_MainMenu.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// คุมปุ่ม Play / Settings / Exit ในหน้าเมนูหลัก
/// - ผูกปุ่มใน Inspector หรือใช้ OnClick() เรียกฟังก์ชันได้
/// - รองรับโหลดซีนด้วยชื่อหรือ Index (เลือกใน Inspector)
/// - Toggle แผง Settings ได้
/// - ปิดการกดปุ่มซ้ำระหว่างโหลดซีน
/// </summary>
public class UI_MainMenu : MonoBehaviour
{
    public enum PlayLoadMode { BySceneName, BySceneIndex }

    [Header("— Buttons (optional, เผื่ออยากปิด/เปิดระหว่างโหลด) —")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    [Header("— Settings Panel —")]
    [Tooltip("ลาก GameObject ของหน้า Settings มาวาง (เช่น Panel_Settings)")]
    [SerializeField] private GameObject settingsPanel;

    [Header("— Play Config —")]
    [SerializeField] private PlayLoadMode loadMode = PlayLoadMode.BySceneName;
    [Tooltip("ใส่ชื่อซีนที่จะเล่น (ต้อง Add เข้า Build Settings)")]
    [SerializeField] private string playSceneName = "Game";
    [Tooltip("หรือใช้ Index ของซีนใน Build Settings")]
    [SerializeField] private int playSceneIndex = 1;
    [Tooltip("โหลดแบบ Async (ไม่ค้างจอ)")]
    [SerializeField] private bool loadAsync = true;

    [Header("— Optional Fade (ถ้ามี Animator ทำ Fade Out) —")]
    [SerializeField] private Animator fadeAnimator;
    [Tooltip("ชื่อ Trigger ที่ใช้สั่ง Fade Out ใน Animator")]
    [SerializeField] private string fadeOutTrigger = "FadeOut";
    [Tooltip("เวลารอให้ Fade Out ก่อนเริ่มโหลด (วินาที)")]
    [SerializeField] private float fadeOutDelay = 0.35f;

    bool _isLoading;

    void Awake()
    {
        // ให้แน่ใจว่า Settings ปิดไว้ตอนเริ่ม (ถ้ากำหนด)
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    // === Public methods สำหรับผูกกับปุ่ม ===
    public void OnPlayClicked()
    {
        if (_isLoading) return;
        StartCoroutine(PlayRoutine());
    }

    public void OnSettingsClicked()
    {
        if (settingsPanel == null)
        {
            Debug.LogWarning("[UI_MainMenu] ยังไม่ได้อ้างอิง Settings Panel");
            return;
        }
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void OnExitClicked()
    {
        // ปิดเกมบน Build จริง / หยุด Play Mode บน Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // === Helpers ===
    IEnumerator PlayRoutine()
    {
        _isLoading = true;
        SetButtonsInteractable(false);

        // เรียก Fade Out ถ้ามี
        if (fadeAnimator != null && !string.IsNullOrEmpty(fadeOutTrigger))
        {
            fadeAnimator.SetTrigger(fadeOutTrigger);
            if (fadeOutDelay > 0f) yield return new WaitForSeconds(fadeOutDelay);
        }

        // โหลดซีน
        if (loadAsync)
        {
            AsyncOperation op = null;
            if (loadMode == PlayLoadMode.BySceneName)
            {
                if (string.IsNullOrEmpty(playSceneName))
                {
                    Debug.LogError("[UI_MainMenu] ไม่ได้กำหนดชื่อซีนสำหรับ Play");
                    SetButtonsInteractable(true);
                    _isLoading = false;
                    yield break;
                }
                op = SceneManager.LoadSceneAsync(playSceneName, LoadSceneMode.Single);
            }
            else
            {
                if (playSceneIndex < 0)
                {
                    Debug.LogError("[UI_MainMenu] Scene Index ไม่ถูกต้อง");
                    SetButtonsInteractable(true);
                    _isLoading = false;
                    yield break;
                }
                op = SceneManager.LoadSceneAsync(playSceneIndex, LoadSceneMode.Single);
            }

            // รอจนโหลดเสร็จ
            while (op != null && !op.isDone)
                yield return null;
        }
        else
        {
            // โหลดแบบบล็อก (เร็วแต่ค้างเฟรม)
            if (loadMode == PlayLoadMode.BySceneName)
                SceneManager.LoadScene(playSceneName, LoadSceneMode.Single);
            else
                SceneManager.LoadScene(playSceneIndex, LoadSceneMode.Single);
        }
    }

    void SetButtonsInteractable(bool interactable)
    {
        if (playButton) playButton.interactable = interactable;
        if (settingsButton) settingsButton.interactable = interactable;
        if (exitButton) exitButton.interactable = interactable;
    }

    // — Optional: ปุ่มปิด Settings ในแผง Settings —
    public void CloseSettings()
    {
        if (settingsPanel) settingsPanel.SetActive(false);
    }
}
