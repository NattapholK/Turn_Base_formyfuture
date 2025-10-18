using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Rendering;
using System.Linq;

[System.Serializable]
public struct Data
{
    public GameObject playerObject;
    public GameObject playerSkillUI;
    public GameObject playerProfileUI;
    public GameObject turnPlayerUIPrefab;
    public Transform targetPlayerCamera;
    public AudioClip takeDamageSound;
    public AudioClip skill1Sound;
    public AudioClip skill2Sound;
    public int hpPlayer;
    public int atkPlayer;
    public int speedPlayer;
}

public class AttackSceneManager : MonoBehaviour
{

    public List<Data> playerData = new List<Data>();

    [Header("ตั้งค่ากล้องจุดบอสและจุดจบ")]
    public Transform targetBoss;
    public Transform targetEnd;
    public Transform targetAll;

    [Header("ตั้งค่าบอส")]
    public GameObject enemy;
    public GameObject turnEnemyUIPrefab;
    public int hpEnemy = 100;
    public int atkBoss = 0;
    public int speedBoss = 0;
    public int skillCount = 1;   // จำนวนสกิลของบอส

    [Header("ตั้งค่ากล้อง")]
    public GameObject cameraObj;
    public float moveTime = 1.5f; // เวลาในการเคลื่อน

    [Header("ตั้งค่าเกม")]
    public float gameSpeed = 1f; //เอาค่าไปหาร พวก WaitForSeconds ใน Coroutine || เอาไว้ตอนทดสอบ ขี้เกียจรอ
    public int SceneIndex;

    [Header("Debug Setting")]
    public bool useOldCameraMode = false;
    public bool lockPlayerCursorOnStart = true;

    [Header("Audio Source")]
    public AudioSource PlayerAudioSource;
    public AudioSource GameAudioSource;


    private bool isLure = false;
    private int turnCount = 0;
    private int numberOfCabbage = 0;
    private int currentTurnIndex = 0; //ทำเป็น public เพราะจะดูค่า
    private bool isEnding = false;
    private UIManager uiScript;
    private Coroutine moveCoroutine;
    private setattack setAttackBoss;
    private StatusSystemScript statusScript;
    private List<setattack> setAtkScriptList = new List<setattack>();
    [HideInInspector] public List<BattleUnit> allUnits = new List<BattleUnit>();

    void Awake()
    {
        if (lockPlayerCursorOnStart)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        setAttackBoss = enemy.GetComponent<setattack>();
        uiScript = GetComponent<UIManager>();
        statusScript = GetComponent<StatusSystemScript>();
    }

    void Start()
    {
        //ขยับจอไปที่โปเกม่อนตัวแรก
        if (useOldCameraMode) MoveToPosition(playerData[0].targetPlayerCamera.position);
        else CameraManager.Instance.CameraChangeTurnTransition(currentTurnIndex);

        for (int i = 0; i < playerData.Count; i++) //allUnit คือ list หลักที่จะเอาไว้เรียงเทิร์น
        {
            statusScript.AddHpUI(playerData[i].playerProfileUI.transform.GetChild(1).gameObject, i);
            setAtkScriptList.Add(playerData[i].playerObject.GetComponent<setattack>());
            allUnits.Add(new BattleUnit
            {
                speed = playerData[i].speedPlayer,
                animator = playerData[i].playerObject.GetComponent<Animator>(),
                uiObj = playerData[i].playerSkillUI,
                targetPos = playerData[i].targetPlayerCamera,
                proFileUI = playerData[i].playerProfileUI.GetComponent<RectTransform>(),
                isBoss = false,
                index = i,
                isdied = false,
                unitIndex = i //ชั่วคราว
            });
        }
        allUnits.Add(new BattleUnit
        {
            speed = speedBoss,
            animator = enemy.GetComponent<Animator>(),
            uiObj = null,
            targetPos = targetBoss,
            proFileUI = null,
            isBoss = true,
            index = allUnits.Count,
            isdied = false
        });

        allUnits = allUnits.OrderByDescending(u => u.speed).ToList(); //เรียงเทิร์นตาม speed

        StartTurn(allUnits[currentTurnIndex]);// เริ่มเทิร์นแรก

    }

    void Update()
    {
        if (statusScript.checkEndGame() && !isEnding)
        {
            isEnding = true;
            statusScript.endFight();
        }
    }

    void StartTurn(BattleUnit unit)
    {
        if (!unit.isBoss)
        {
            // เปิด UI ให้ Player เลือกสกิล
            unit.uiObj.SetActive(true);
            StartCoroutine(uiScript.ScaleUI(unit.proFileUI, "up"));

            CameraManager.Instance.CameraChangeTurnTransition(unit.unitIndex); //ชั่วคราว
        }
        else
        {
            // บอส auto ใช้สกิล
            StartCoroutine(BossTurn(unit));
            MoveToPosition(unit.targetPos.position);
        }

        // ใช้ระบบกล้องใหม่
        //MoveToPosition(unit.targetPos.position);

        /*if (useOldCameraMode) MoveToPosition(unit.targetPos.position);
        else CameraManager.Instance.CameraChangeTurnTransition(currentTurnIndex);*/
    }

    IEnumerator EndTurn()
    {
        yield return new WaitForSeconds(1.3f);
        StartCoroutine(uiScript.CheckDiedPlayerIcon());
        yield return new WaitForSeconds(1f / gameSpeed);

        if (isEnding) //จบแล้วเอากล้องไปตรงนี้
        {
            MoveToPosition(targetEnd.position);
            yield break;
        }

        currentTurnIndex++;
        if (currentTurnIndex >= allUnits.Count)
            currentTurnIndex = 0; // วนกลับรอบใหม่

        while (allUnits[currentTurnIndex].isdied)
        {
            currentTurnIndex++;
            if (currentTurnIndex >= allUnits.Count)
            {
                currentTurnIndex = 0; // วนกลับรอบใหม่
            }
        }

        StartTurn(allUnits[currentTurnIndex]);
    }

    // เรียกใช้ผ่าน UI Button
    public void PlayCurrentTurn(int skillID)
    {
        BattleUnit unit = allUnits[currentTurnIndex];
        if (skillID == 2)
        {
            int playerIndex = 0;
            for (int i = 0; i < playerData.Count; i++)
            {
                if (unit.speed == playerData[i].speedPlayer)
                {
                    playerIndex = i;
                    break;
                }
            }
            if (statusScript.manaPlayerList[playerIndex] < statusScript.maxManaData())
            {
                Debug.Log("Player มานาไม่พอ");
                return;
            }

            if (setAtkScriptList[playerIndex].isCabbage)
            {
                setAttackBoss.ChangeStateReduceDmg(true);
                numberOfCabbage = playerIndex;
                statusScript.setCabbageIndex(numberOfCabbage);
                isLure = true;
            }
        }


        // รัน animation
        string skillTrigger = "isAttack" + skillID;
        unit.animator.SetBool(skillTrigger, true);
        unit.uiObj.SetActive(false);
        StartCoroutine(uiScript.ScaleUI(unit.proFileUI, "down"));

        StartCoroutine(EndTurn());
    }

    IEnumerator BossTurn(BattleUnit unit)
    {
        yield return new WaitForSeconds(2f / gameSpeed);

        // ถึงเทิร์นของศัตรู
        int bossSkillTarget = 0;
        int bossSkillToUse = 0;
        bool StatusChecked = true;

        float ProbabilitySkill = Random.Range(0f, 100f);
        float ProbabilityTarget = Random.Range(0f, 100f);

        while (StatusChecked)
        {
            if (isLure && turnCount < 2)
            {
                if (ProbabilityTarget < 30f)   // โอกาศออก30%
                {
                    List<int> index = new List<int>();
                    for (int i = 0; i <= playerData.Count; i++)
                    {
                        index.Add(i);
                    }
                    index.Remove(0);
                    index.Remove(numberOfCabbage + 1);
                    bossSkillTarget = index[Random.Range(0, index.Count)];
                    Debug.Log("บอสบิด");
                }
                else              // โอกาศออก70%
                {
                    bossSkillTarget = numberOfCabbage + 1;
                    Debug.Log("บอสโดนล่อเรียบร้อย");
                }
                turnCount++;
                if (turnCount == 2)
                {
                    isLure = false;
                    turnCount = 0;
                    setAttackBoss.ChangeStateReduceDmg(false);
                    statusScript.setCabbageIndex(-2);
                }
            }
            else
            {
                bossSkillTarget = Random.Range(1, playerData.Count + 1);
            }

            foreach (BattleUnit player in allUnits)
            {
                if (player.index == bossSkillTarget - 1 && !player.isdied)
                {
                    StatusChecked = false;
                    break;
                }
            }
        }

        switch (skillCount)
        {
            case 1:
                break;
            case 2:
                if (ProbabilitySkill < 30f)   // โอกาศออก30%
                {
                    bossSkillToUse = 1;
                }
                else              // โอกาศออก70%
                {
                    bossSkillToUse = 0;
                }
                break;
            case 3:
                if (ProbabilitySkill < 70f)   // โอกาศออก70%
                {
                    bossSkillToUse = 0;
                }
                else if (ProbabilitySkill < 90f)     // โอกาศออก20%
                {
                    bossSkillToUse = 1;
                }
                else  // โอกาศออก10%
                {
                    bossSkillToUse = 2;
                }
                break;
            default:
                Debug.Log("จำนวนสกิลบอสไม่ถูกต้อง");
                break;
        }

        //ขยับจอไปที่เป้าหมาย
        if (setAttackBoss.isBird && bossSkillToUse == 1)
        {
            //MoveToPosition(targetAll.position);
        }
        else
        {
            Debug.Log("bossSkillTarget = " + bossSkillTarget);
            //MoveToPosition(playerData[bossSkillTarget - 1].targetPlayerCamera.position);
        }

        CameraManager.Instance.CameraChangeTurnTransition(bossSkillTarget - 1, false); //ชั่วคราว 

        yield return new WaitForSeconds(2f / gameSpeed);

        

        //โค้ดชั่วคราว ถ้าไม่ได้เปลี่ยนก็ใช้งี้ได้ | รัน animation ถ้าเจอบอส nuutor เวอร์ชั่นใหม่
        GameObject bossRoot = GameObject.Find("Boss - Nuutor Rat");
        if (bossRoot != null)
        {
            Transform turnAnimatorTransform = bossRoot.transform.Find("GameObject/GameObject/Turn Animator");
            if (turnAnimatorTransform != null)
            {
                if (bossSkillToUse == 0)
                {
                    //หันแล้วก็โดดไปตี
                    Debug.Log("triggerName = " + "ratTurnAttack_" + bossSkillTarget);
                    string triggerName = "ratTurnAttack_" + bossSkillTarget;

                    if (SceneIndex == 2 && CameraManager.Instance != null)
                    {
                        CameraManager.Instance.bossRotateTowardsTarget.AssignTargetAndEnable(CameraManager.Instance.GetCharacterTargetPosition(bossSkillTarget - 1));
                        triggerName = "ratTurnAttack"; //ชั่วคราว 
                    }
                    
                    turnAnimatorTransform.GetComponent<Animator>().SetTrigger(triggerName);

                }
                else if (bossSkillToUse == 1)
                {
                    //หันเฉยๆ (สกิลเสกหนู)
                    Debug.Log("triggerName = " + "ratTurn_" + bossSkillTarget);

                    if (SceneIndex == 2 && CameraManager.Instance != null)
                    {
                        CameraManager.Instance.bossRotateTowardsTarget.AssignTargetAndEnable(CameraManager.Instance.GetCharacterTargetPosition(bossSkillTarget - 1));
                    } else
                    {
                        string skillTriggerName = "ratTurn_" + bossSkillTarget;
                        turnAnimatorTransform.GetComponent<Animator>().SetTrigger(skillTriggerName);
                    }
                    
                }
                string bossTrigger = "isAttack" + bossSkillTarget + bossSkillToUse;
                unit.animator.SetBool(bossTrigger, true);
            }
        } else
        {
            //รัน animation แบบเดิม ถ้าไม่ได้ตีกับ boss nuutor เวอร์ชั่นใหม่
            string bossTrigger = "isAttack" + bossSkillTarget + bossSkillToUse;
            unit.animator.SetBool(bossTrigger, true);
        }



        Debug.Log("บอสใช้สกิล: " + bossSkillToUse + "กับ player " + bossSkillTarget);

        yield return new WaitForSeconds(1f / gameSpeed);
        StartCoroutine(EndTurn());
    }


    // เรียกใช้เพื่อย้ายกล้องไปตำแหน่งใหม่
    public void MoveToPosition(Vector3 newPos)
    {
        if (cameraObj == null)
        {
            Debug.LogWarning("ยังไม่ได้กำหนด cameraObj ใน Inspector");
            return;
        }

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(SmoothMove(newPos));
    }

    IEnumerator SmoothMove(Vector3 targetPos)
    {
        GameObject cameraObj = CameraManager.Instance.CodeMovementObj; //อยู่ในช่วงลองผิดลองถูก

        Vector3 startPos = cameraObj.transform.position;
        float elapsed = 0f;


        while (elapsed < moveTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveTime);

            cameraObj.transform.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }

        cameraObj.transform.position = targetPos;
    }
}