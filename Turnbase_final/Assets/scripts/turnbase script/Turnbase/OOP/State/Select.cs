using UnityEngine;

public class Select : StateMachine
{
    private bool isUsingSkill1UITracker;
    private bool isUsingSkill2UITracker;
    private GameObject _Skill_1_UI;
    private GameObject _Skill_2_UI;
    public Select(Character c) : base(c)
    {
        Name = STATE.SELECT;
    }

    public override void Enter()
    {
        Debug.Log("Select ของ" + Me + "ทำงาน");
        if(Me is Player p)
        {
            p._SkillUIObject.SetActive(true);
            _Skill_1_UI = p._SkillUIObject.transform.GetChild(0).gameObject;
            _Skill_2_UI = p._SkillUIObject.transform.GetChild(1).gameObject;
        }
        base.Enter();
    }
    public override void Update()
    {
        if(Me is Player p)
        {
            bool EbuttonPressed = Input.GetKeyDown(KeyCode.E);//กดใช้สกิล 1
            bool QbuttonPressed = Input.GetKeyDown(KeyCode.Q);//กดใช้สกิล 2
            RectTransform _Skill_1_Rect = _Skill_1_UI.GetComponent<RectTransform>();
            RectTransform _Skill_2_Rect = _Skill_2_UI.GetComponent<RectTransform>();

            if (EbuttonPressed)
            {
                if (isUsingSkill1UI)
                {
                    isUsingSkill1UI = false;
                    _Skill_1_Rect.localScale = Vector3.one * 2;

                    NextState = new NormalAttack(Me, p.enemies[0], 0);//เขียนทิ้งไว้ก่อน
                    p._SkillUIObject.SetActive(false);
                    Stage = EVENT.EXIT;
                }
                else
                {
                    isUsingSkill1UI = true;
                    _Skill_1_Rect.localScale = Vector3.one * 2 * 1.2f;
                    if (isUsingSkill2UI)
                    {
                        isUsingSkill2UI = false;
                        _Skill_2_Rect.localScale = Vector3.one * 2;
                    }
                }
            }

            if (QbuttonPressed)
            {
                if (isUsingSkill2UI)
                {
                    isUsingSkill2UI = false;
                    _Skill_2_Rect.localScale = Vector3.one * 2;

                    if(p.GetMana() < p._Mana) return;
                    NextState = new Skill(Me, p.enemies[0], 0);//เขียนทิ้งไว้ก่อน
                    p._SkillUIObject.SetActive(false);
                    Stage = EVENT.EXIT;
                }
                else
                {
                    isUsingSkill2UI = true;
                    _Skill_2_Rect.localScale = Vector3.one * 2 * 1.2f;
                    if (isUsingSkill1UI)
                    {
                        isUsingSkill1UI = false;
                        _Skill_1_Rect.localScale = Vector3.one * 2;
                    }
                }
            }
        }
        else if(Me is Enemy e)
        {
            //เดี๋ยวเขียนเพิ่ม
            float ProbabilitySkill = Random.Range(0f, 100f);
            int bossSkillToUse = 0; // โอกาศออก70%
            int bossSkillTarget = 0;

            if (ProbabilitySkill < 30f)   // โอกาศออก30%
            {
                bossSkillToUse = 1;
            }

            do
            {
                bossSkillTarget = Random.Range(1, e.players.Length + 1);
            }
            while(e.players[bossSkillTarget - 1].GetHp() <= 0);

            if(bossSkillToUse == 0)
            {
                NextState = new NormalAttack(Me, e.players[bossSkillTarget - 1], bossSkillTarget);    
            }
            else
            {
                NextState = new Skill(Me, e.players[bossSkillTarget - 1], bossSkillTarget);       
            }
            Stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        Debug.Log("Select ของ" + Me + "จบแล้ว");
        base.Exit();
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

                // if (value) CameraManager.Instance.currentCameraState = CameraTurnbaseState.SelectingFirstSkillButton;
                // else CameraManager.Instance.currentCameraState = CameraTurnbaseState.Idle;
                
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
                
                // if (value) CameraManager.Instance.currentCameraState = CameraTurnbaseState.SelectingSecondSkillButton;
                // else CameraManager.Instance.currentCameraState = CameraTurnbaseState.Idle;
            }
        }
    }
}