using UnityEngine;
using TMPro;

public class ShowRealPoint : MonoBehaviour
{
    public GameObject UIprefab;
    public GameObject BossTarget;
    public GameObject player;
    public Canvas UICanvas;
    private GameObject UIingame;

    void Start()
    {
        showfloatingUI();
    }
    void Update()
    {
        UpdateFloatingUI();
    }

    private void showfloatingUI()
    {
        // Instantiate บน Canvas
        UIingame = Instantiate(UIprefab, UICanvas.transform);
    }
    
    private void UpdateFloatingUI()
    {
        float distance = Vector3.Distance(player.transform.position, BossTarget.transform.position);

        Vector3 worldPos = BossTarget.transform.position;

        // แปลงไปเป็น screen position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        if (screenPos.z < 0)
        {
            UIingame.SetActive(false);
            return;
        }
        UIingame.SetActive(true);
        UIingame.transform.position = screenPos;
    
        var tmp = UIingame.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        tmp.text = ((int)distance).ToString() + " M";
    }
}
