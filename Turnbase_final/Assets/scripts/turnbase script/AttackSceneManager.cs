using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Rendering;
using System.Linq;
public class AttackSceneManager : MonoBehaviour
{
    [Header("ตั้งค่า UI โปเกม่อน")]
    public List<GameObject> pokemonUIList;  // UI ของโปเกม่อนแต่ละตัว
    public List<GameObject> pokemonProfileUIList;

    [Header("ตั้งค่าโปเกม่อน")]
    public List<GameObject> pokemonList;    // ตัวโปเกม่อนแต่ละตัว

    [Header("ตั้งค่ากล้องแต่ละจุดเรียงตามตัวโปเกม่อน")]
    public List<Transform> targetList;    // จุดที่กล้องจะมองแต่ละตัว

    [Header("ตั้งค่ากล้องจุดบอสและจุดจบ")]
    public Transform targetBoss;
    public Transform targetEnd;
    public Transform targetAll;

    [Header("ตั้งค่าบอส")]
    public GameObject enemy;
    public int skillCount = 1;   // จำนวนสกิลของบอส

    [Header("ตั้งค่ากล้อง")]
    public GameObject cameraObj;
    public float moveTime = 1.5f; // เวลาในการเคลื่อน


    private bool isLure = false;
    private int numberOfCabbage = 0;
    private int turnCount = 0;
    [HideInInspector] public List<BattleUnit> allUnits = new List<BattleUnit>();
    private int currentTurnIndex = 0;
    private bool isEnding = false;
    private Coroutine moveCoroutine;
    private setattack setAttackBoss;
    private List<setattack> setAtkScriptList = new List<setattack>();
    private StatusSystemScript statusScript;
    private UIManager uiScript;
    private List<Animator> pokemonAnimList = new List<Animator>();

    void Start()
    {
        Cursor.visible = false; // ซ่อนเมาส์
        Cursor.lockState = CursorLockMode.Locked; // ล็อคเมาส์ให้อยู่กลางจอ

        setAttackBoss = enemy.GetComponent<setattack>();
        uiScript = GetComponent<UIManager>();
        statusScript = GetComponent<StatusSystemScript>();

        //ขยับจอไปที่โปเกม่อนตัวแรก
        MoveToPosition(targetList[0].position);

        foreach (GameObject poke in pokemonList)
        {
            pokemonAnimList.Add(poke.GetComponent<Animator>());
            setAtkScriptList.Add(poke.GetComponent<setattack>());
        }

        for (int i = 0; i < pokemonList.Count; i++) //allUnit คือ list หลักที่จะเอาไว้เรียงเทิร์น
        {
            statusScript.AddHpUI(pokemonProfileUIList[i].transform.GetChild(1).gameObject, i);
            allUnits.Add(new BattleUnit
            {
                speed = statusScript.speedPlayerList[i],
                animator = pokemonList[i].GetComponent<Animator>(),
                uiObj = pokemonUIList[i],
                targetPos = targetList[i],
                proFileUI = pokemonProfileUIList[i].GetComponent<RectTransform>(),
                isBoss = false,
                index = i,
                isdied = false
            });
        }
        allUnits.Add(new BattleUnit
        {
            speed = statusScript.speedBoss,
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
        }
        else
        {
            // บอส auto ใช้สกิล
            StartCoroutine(BossTurn(unit));
        }

        MoveToPosition(unit.targetPos.position);
    }

    IEnumerator EndTurn()
    {
        StartCoroutine(uiScript.CheckDiedPlayerIcon());
        yield return new WaitForSeconds(2f);

        if (isEnding)
        {
            MoveToPosition(targetEnd.position);
            yield break;
        }

        currentTurnIndex++;
        if (currentTurnIndex >= allUnits.Count)
            currentTurnIndex = 0; // วนกลับรอบใหม่

        while (allUnits[currentTurnIndex].isdied && !allUnits[currentTurnIndex].isBoss)
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
            for (int i = 0; i < statusScript.speedPlayerList.Count; i++)
            {
                if (unit.speed == statusScript.speedPlayerList[i])
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
        StartCoroutine(uiScript.DeleteTurnIcon());
        StartCoroutine(uiScript.ScaleUI(unit.proFileUI, "down"));

        StartCoroutine(EndTurn());
    }

    IEnumerator BossTurn(BattleUnit unit)
    {
        yield return new WaitForSeconds(2f);

        // ถึงเทิร์นของศัตรู
        int bossSkillTarget = 0;
        int bossSkillToUse = 1;
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
                    for (int i = 0; i <= pokemonList.Count; i++)
                    {
                        index.Add(i);
                    }
                    index.Remove(0);
                    index.Remove(numberOfCabbage + 1);
                    bossSkillTarget = index[Random.Range(0, index.Count)];
                }
                else              // โอกาศออก70%
                {
                    bossSkillTarget = numberOfCabbage + 1;
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
                bossSkillTarget = Random.Range(1, pokemonList.Count + 1);
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
            MoveToPosition(targetAll.position);
        }
        else
        {
            Debug.Log("bossSkillTarget = " + bossSkillTarget);
            MoveToPosition(targetList[bossSkillTarget - 1].position);
        }

        yield return new WaitForSeconds(2f);

        string bossTrigger = "isAttack" + bossSkillTarget + bossSkillToUse;
        unit.animator.SetBool(bossTrigger, true);

        Debug.Log("บอสใช้สกิล: " + bossSkillToUse + "กับ player " + bossSkillTarget);
        StartCoroutine(uiScript.DeleteTurnIcon());

        yield return new WaitForSeconds(1f);
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