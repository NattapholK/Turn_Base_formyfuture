using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject backgroundUI;
    private float bgUIHight;
    private RectTransform bgUItranform;
    void Start()
    {
        bgUItranform = backgroundUI.GetComponent<RectTransform>();
        bgUIHight = bgUItranform.sizeDelta.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
