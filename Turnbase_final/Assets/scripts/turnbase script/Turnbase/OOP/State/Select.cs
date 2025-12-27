using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Select : StateMachine
{
    bool isUsingSkill1UITracker;
    bool isUsingSkill2UITracker;
    GameObject _Skill_1_UI;
    GameObject _Skill_2_UI;
    float timer1 = 0f;
    float timer2 = 0f;

    //for nuutor
    int useSkill = 0;
    int bossTarget = 0;
    
    public Select(Character c) : base(c)
    {
        Name = STATE.SELECT;
    }

    public override void Enter()
    {
        // Debug.Log("Select ของ" + Me + "ทำงาน");
        if(Me is Player p)
        {
            p._SkillUIObject.SetActive(true);
            _Skill_1_UI = p._SkillUIObject.transform.GetChild(0).gameObject;
            _Skill_2_UI = p._SkillUIObject.transform.GetChild(1).gameObject;
        }
        else if(Me is Enemy e)
        {
            timer1 += Time.deltaTime;
            if(timer1 < 2f) return;

            float ProbabilitySkill = Random.Range(0f, 100f);
            int bossSkillToUse = 0; // โอกาศออก70%
            int bossSkillTarget = 0;

            if (ProbabilitySkill < 30f)   // โอกาศออก30%
            {
                bossSkillToUse = 1;
            }

            List<int> targets = new List<int>();
            for(int i= 0; i < e.players.Count(); i++)
            {
                for(int j = 0; j < e.players[i].GetTaunt(); j++)
                {
                    targets.Add(i);
                }
            }

            do
            {
                int target = Random.Range(0, targets.Count());
                bossSkillTarget = targets[target];
                Debug.Log("bossSkillTarget = " + bossSkillTarget);
            }
            while(e.players[bossSkillTarget].GetHp() <= 0);

            if(bossSkillToUse == 0)
            {
                NextState = new NormalAttack(Me, e.players[bossSkillTarget], bossSkillTarget);    
            }
            else
            {
                NextState = new Skill(Me, e.players[bossSkillTarget], bossSkillTarget);       
            }

            useSkill = bossSkillToUse;
            bossTarget = bossSkillTarget;
            CameraManager.Instance.CameraChangeTurnTransition(bossSkillTarget, false);
            
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

                    Debug.Log("p.GetMana() = " + p.GetMana());
                    Debug.Log("p._Mana = " + p._Mana);

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
            timer2 += Time.deltaTime;
            if(timer2 < 2f) return;
            
            nuutor(useSkill,bossTarget);
            
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

    public void nuutor(int bossSkillToUse ,int bossSkillTarget)
    {
        //โค้ดชั่วคราว ถ้าไม่ได้เปลี่ยนก็ใช้งี้ได้ | รัน animation ถ้าเจอบอส nuutor เวอร์ชั่นใหม่ ของเอิร์ท
        GameObject bossRoot = GameObject.Find("Boss - Nuutor Rat");
        if (bossRoot != null)
        {
            Transform turnAnimatorTransform = bossRoot.transform.Find("GameObject/GameObject/Turn Animator");
            if (turnAnimatorTransform != null)
            {
                if (bossSkillToUse == 0)
                {
                    //หันแล้วก็โดดไปตี
                    Debug.Log("triggerName = " + "ratTurnAttack_" + bossSkillTarget + 1);
                    string triggerName = "ratTurnAttack_" + bossSkillTarget + 1;

                    if (Me._Manager.SceneIndex == 2 && CameraManager.Instance != null)
                    {
                        CameraManager.Instance.bossRotateTowardsTarget.AssignTargetAndEnable(CameraManager.Instance.GetCharacterTargetPosition(bossSkillTarget));
                        triggerName = "ratTurnAttack"; //ชั่วคราว 
                    }
                    
                    turnAnimatorTransform.GetComponent<Animator>().SetTrigger(triggerName);
                }
                else if (bossSkillToUse == 1)
                {
                    //หันเฉยๆ (สกิลเสกหนู)
                    Debug.Log("triggerName = " + "ratTurn_" + bossSkillTarget + 1);

                    if (Me._Manager.SceneIndex == 2 && CameraManager.Instance != null)
                    {
                        CameraManager.Instance.bossRotateTowardsTarget.AssignTargetAndEnable(CameraManager.Instance.GetCharacterTargetPosition(bossSkillTarget));
                    } else
                    {
                        string skillTriggerName = "ratTurn_" + bossSkillTarget + 1;
                        turnAnimatorTransform.GetComponent<Animator>().SetTrigger(skillTriggerName);
                    }
                }
            }
        }
    }
}