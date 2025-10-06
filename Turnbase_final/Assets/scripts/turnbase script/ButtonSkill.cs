using UnityEngine;


public class ButtonSkill : MonoBehaviour
{
    public AttackSceneManager attackSceneManager;
    private GameObject skill1;
    private GameObject skill2; 
    private UIManager manager;
    private bool isUsingSkill1UI = false;
    private bool isUsingSkill2UI = false;
    void Awake()
    {
        manager = attackSceneManager.GetComponent<UIManager>();
        skill1 = transform.GetChild(0).gameObject;
        skill2 = transform.GetChild(1).gameObject;
    }
    void Start()
    {
        
    }


    void Update()
    {
        bool EbuttonPressed = Input.GetKeyDown(KeyCode.E);//กดใชเสกิล 1
        bool QbuttonPressed = Input.GetKeyDown(KeyCode.Q);//กดใชเสกิล 2

        if (EbuttonPressed)
        {
            if (isUsingSkill1UI)
            {
                isUsingSkill1UI = false;
                skill1.GetComponent<RectTransform>().localScale = Vector3.one * 2;
                attackSceneManager.PlayCurrentTurn(1);
            }
            else
            {
                isUsingSkill1UI = true;
                StartCoroutine(manager.ScaleUI(skill1.GetComponent<RectTransform>(), "up"));
                if (isUsingSkill2UI)
                {
                    isUsingSkill2UI = false;
                    StartCoroutine(manager.ScaleUI(skill2.GetComponent<RectTransform>(), "down"));
                }
            }
        }

        if (QbuttonPressed)
        {
            if (isUsingSkill2UI)
            {
                isUsingSkill2UI = false;
                skill2.GetComponent<RectTransform>().localScale = Vector3.one * 2;
                attackSceneManager.PlayCurrentTurn(2);
            }
            else
            {
                isUsingSkill2UI = true;
                StartCoroutine(manager.ScaleUI(skill2.GetComponent<RectTransform>(), "up"));
                if (isUsingSkill1UI)
                {
                    isUsingSkill1UI = false;
                    StartCoroutine(manager.ScaleUI(skill1.GetComponent<RectTransform>(), "down"));
                }
            }
        }
    }
}
