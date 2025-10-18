using UnityEngine;


public class ButtonSkill : MonoBehaviour
{
    public GameObject attackSceneObject;
    private GameObject skill1;
    private GameObject skill2;
    private AttackSceneManager attackSceneManager;
    // private UIManager manager;

    //เปลี่ยนไปใช้อีกแบบ ดูข้างล่างสุด
    //private bool isUsingSkill1UI = false;
    //private bool isUsingSkill2UI = false;
    void Awake()
    {
        attackSceneManager = attackSceneObject.GetComponent<AttackSceneManager>();
        // manager = attackSceneObject.GetComponent<UIManager>();
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
                skill1.GetComponent<RectTransform>().localScale = Vector3.one * 2 * 1.2f;
                if (isUsingSkill2UI)
                {
                    isUsingSkill2UI = false;
                    skill2.GetComponent<RectTransform>().localScale = Vector3.one * 2;
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
                skill2.GetComponent<RectTransform>().localScale = Vector3.one * 2 * 1.2f;
                if (isUsingSkill1UI)
                {
                    isUsingSkill1UI = false;
                    skill1.GetComponent<RectTransform>().localScale = Vector3.one * 2;
                }
            }
        }
    }





    //ทำให้ isUsingSkill1UI isUsingSkill2UI เวลาเปลี่ยนค่ามันจะรันโค้ดเพิ่มเติม
    private bool isUsingSkill1UI
    {
        get => isUsingSkill1UITracker;
        set
        {
            if (isUsingSkill1UITracker != value)
            {
                isUsingSkill1UITracker = value;

                if (value) CameraManager.Instance.currentCameraState = CameraTurnbaseState.SelectingFirstSkillButton;
                else CameraManager.Instance.currentCameraState = CameraTurnbaseState.Idle;
                
            }
        }
    }

    private bool isUsingSkill2UI
    {
        get => isUsingSkill2UITracker;
        set
        {
            if (isUsingSkill2UITracker != value)
            {
                isUsingSkill2UITracker = value;
                
                if (value) CameraManager.Instance.currentCameraState = CameraTurnbaseState.SelectingSecondSkillButton;
                else CameraManager.Instance.currentCameraState = CameraTurnbaseState.Idle;
            }
        }
    }

    private bool isUsingSkill1UITracker;
    private bool isUsingSkill2UITracker;
}
