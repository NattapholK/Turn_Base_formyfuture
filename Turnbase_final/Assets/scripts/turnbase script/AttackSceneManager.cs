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

    [Header("ตั้งค่าโปเกม่อน")]
    public List<GameObject> pokemonList;    // ตัวโปเกม่อนแต่ละตัว

    [Header("ตั้งค่ากล้องแต่ละจุดเรียงตามตัวโปเกม่อน")]
    public List<Transform> targetList;    // จุดที่กล้องจะมองแต่ละตัว

    [Header("ตั้งค่ากล้องจุดบอสและจุดจบ")]
    public Transform targetBoss;
    public Transform targetEnd;

    [Header("ตั้งค่าบอส")]
    public GameObject enemy;
    public int skillCount = 1;   // จำนวนสกิลของบอส (ลากใส่ Inspector)
    private Animator enemy_anim; // เก็บไว้เผื่อใช้

    [Header("ตั้งค่ากล้อง")]
    public GameObject cameraObj;
    public float moveTime = 1.5f; // เวลาในการเคลื่อน



    private List<BattleUnit> allUnits = new List<BattleUnit>();
    private int currentTurnIndex = 0;
    private bool isEnding = false;
    private Coroutine moveCoroutine;
    private StatusSysyemScript statusScript;
    private List<Animator> pokemonAnimList = new List<Animator>();

    void Start()
    {
        statusScript = GetComponent<StatusSysyemScript>();
        // enemy_anim = enemy.GetComponent<Animator>(); เก็บไว้เผื่อใช้

        //ขยับจอไปที่โปเกม่อนตัวแรก
        MoveToPosition(targetList[0].position);

        // ดึง Animator ของแต่ละโปเกม่อนมาเก็บไว้
        foreach (GameObject poke in pokemonList)
        {
            pokemonAnimList.Add(poke.GetComponent<Animator>());
        }

        for (int i = 0; i < pokemonList.Count; i++)
        {
            allUnits.Add(new BattleUnit
            {
                speed = statusScript.speedPlayerList[i],
                animator = pokemonList[i].GetComponent<Animator>(),
                uiObj = pokemonUIList[i],
                targetPos = targetList[i],
                isBoss = false
            });
        }
        allUnits.Add(new BattleUnit
        {
            speed = statusScript.speedBoss,
            animator = enemy.GetComponent<Animator>(),
            uiObj = null,
            targetPos = targetBoss,
            isBoss = true
        });

        allUnits = allUnits.OrderByDescending(u => u.speed).ToList();
        StartTurn(allUnits[currentTurnIndex]);// เริ่มเทิร์นแรก

        // foreach (BattleUnit unit in allUnits)
        // {
        //     Debug.Log("############################");
        //     Debug.Log("speed = " + unit.speed);
        //     Debug.Log("animator = " + unit.animator);
        //     Debug.Log("uiObj = " + unit.uiObj);
        //     Debug.Log("targetPos = " + unit.targetPos);
        //     Debug.Log("isBoss = " + unit.isBoss);
        //     Debug.Log("############################");
        // }เอาไว้เทส
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
        yield return new WaitForSeconds(2f);

        if (isEnding)
        {
            MoveToPosition(targetEnd.position);
            yield break;
        }

        currentTurnIndex++;
        if (currentTurnIndex >= allUnits.Count)
            currentTurnIndex = 0; // วนกลับรอบใหม่

        StartTurn(allUnits[currentTurnIndex]);
    }

    // เรียกใช้ผ่าน UI Button
    public void PlayCurrentTurn(int skillID)
    {
        BattleUnit unit = allUnits[currentTurnIndex];

        // รัน animation
        string skillTrigger = "isAttack" + skillID;
        unit.animator.SetBool(skillTrigger, true);
        unit.uiObj.SetActive(false);

        StartCoroutine(EndTurn());
    }

    IEnumerator BossTurn(BattleUnit unit)
    {
        yield return new WaitForSeconds(2f);

        // ถึงเทิร์นของศัตรู
        int bossSkillTarget = 0;
        int bossSkillToUse = 1;

        float Probability = Random.Range(0f, 100f);
        bossSkillTarget = Random.Range(1, 4);

        switch (skillCount)
        {
            case 1:
                break;
            case 2:
                if (Probability < 30f)   // โอกาศออก30%
                {
                    bossSkillToUse = 1;
                }
                else              // โอกาศออก70%
                {
                    bossSkillToUse = 0;
                }
                break;
            case 3:
                if (Probability < 70f)   // โอกาศออก70%
                {
                    bossSkillToUse = 0;
                }
                else if (Probability < 90f)     // โอกาศออก20%
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
        MoveToPosition(targetList[bossSkillTarget - 1].position);
        yield return new WaitForSeconds(2f);

        string bossTrigger = "isAttack" + bossSkillTarget + bossSkillToUse;
        unit.animator.SetBool(bossTrigger, true);

        Debug.Log("บอสใช้สกิล: " + bossSkillToUse + "กับ player " + bossSkillTarget);

        yield return new WaitForSeconds(2f);
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