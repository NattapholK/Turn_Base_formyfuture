using UnityEngine;

public class OpenSettingMenu : MonoBehaviour
{
    public GameObject settingPanel;
    private bool isOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !isOpen)
        {
            OpenPanel();
        }
    }

    private void OpenPanel()
    {
        Time.timeScale = 0f;
        isOpen = true;

        // เปิดเมาส์
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        settingPanel.SetActive(true);
    }
    
    public void ClosePanel()
    {
        Time.timeScale = 1f;
        isOpen = false;

        // Optional : ล็อกจอ
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;

        settingPanel.SetActive(false);
    }
}
