using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


///  คุมปุ่ม Play / Settings / Exit ในหน้าเมนูหลัก
/// - ผูกปุ่มใน Inspector หรือใช้ OnClick() เรียกฟังก์ชันได้
/// - รองรับโหลดซีนด้วยชื่อหรือ Index (เลือกใน Inspector)
/// - Toggle แผง Settings ได้
/// - ปิดการกดปุ่มซ้ำระหว่างโหลดซีน

public class UI_MainMenu : MonoBehaviour
{
    public enum PlayLoadMode { BySceneName, BySceneIndex }

    [Header("— Buttons —")] //ใส่ไว้เฉยๆ ไว้คุมไม่ให้กดซ้ำมั้งตอนเปลี่ยน scene
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;



    [Header("— Settings Panel —")]
    [Tooltip("ลาก GameObject ของหน้า Settings มาวาง (เช่น Panel_Settings)")]
    [SerializeField] private GameObject settingsPanel;


    [Header("— Play Config —")]
    [SerializeField] private PlayLoadMode loadMode = PlayLoadMode.BySceneName;
    [Tooltip("ใส่ชื่อ Scene ที่จะเล่น (ต้อง Add เข้า Build Settings)")]
    [SerializeField] private string playSceneName = "Game";
    [Tooltip("หรือใช้ Index ของซีนใน Build Settings")]
    [SerializeField] private int playSceneIndex = 1;
    [Tooltip("โหลดแบบ Async (ไม่ค้างจอ)")]
    [SerializeField] private bool loadAsync = true;

    private bool _isLoading; // เช็คว่าเปลี่ยน scene อยู่มั้ย



    void Awake()
    {
        // ให้แน่ใจว่า Settings ปิดไว้ตอนเริ่ม (ถ้ากำหนด)
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    // กด play
    public void OnPlayClicked()
    {
        if (_isLoading) return;
        StartCoroutine(PlayRoutine());
    }

    // กด setting
    public void OnSettingsClicked()
    {
        if (settingsPanel == null)
        {
            Debug.LogWarning("[UI_MainMenu] ยังไม่ได้อ้างอิง Settings Panel");
            return;
        }
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    // กด exit
    public void OnExitClicked()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // ปิดเกมบน Build จริง 
    #else
                Application.Quit();
    #endif
    }

    // ปุ่ม play
    IEnumerator PlayRoutine()
    {
        _isLoading = true;
        SetButtonsInteractable(false);


        // โหลดซีน
        if (loadAsync) //โหลดแบบไม่ค้างเฟรม
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
            // โหลดแบบค้างเฟรม
            if (loadMode == PlayLoadMode.BySceneName)
                SceneManager.LoadScene(playSceneName, LoadSceneMode.Single);
            else
                SceneManager.LoadScene(playSceneIndex, LoadSceneMode.Single);
        }
    }

    // set ให้กดปุ่มไม่ได้สักปุ่ม
    void SetButtonsInteractable(bool interactable)
    {
        if (playButton) playButton.interactable = interactable;
        if (settingsButton) settingsButton.interactable = interactable;
        if (exitButton) exitButton.interactable = interactable;
    }

    // Optional: ปุ่มปิด Settings ในแผง Settings 
    public void CloseSettings()
    {
        if (settingsPanel) settingsPanel.SetActive(false);
    }
}
