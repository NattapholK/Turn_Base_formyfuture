using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Rendering;
public class AttackSceneManager : MonoBehaviour
{
    public int hpEnemy = 16;
    public int hpPlayer1 = 20;
    public int hpPlayer2 = 20;
    public int hpPlayer3 = 20;

    public List<GameObject> pokemonUIList;  // UI ของโปเกม่อนแต่ละตัว
    public List<GameObject> pokemonList;    // ตัวโปเกม่อนแต่ละตัว
    public List<Transform> targetList;    // จุดที่กล้องจะมองแต่ละตัว
    public Transform targetBoss;
    public Transform targetEnd;

    public GameObject enemy;
    private Animator enemy_anim;

    public GameObject cameraObj;
          
    public float moveTime = 1.5f; // เวลาในการเคลื่อน

    private Coroutine moveCoroutine;


    private List<Animator> pokemonAnimList = new List<Animator>();

    private int currentTurnIndex = 0;

    [Header("ตั้งค่าบอส")]
    public int skillCount = 1;   // จำนวนสกิลของบอส (ลากใส่ Inspector)
    private bool isEnding = false;
    void Start()
    {
        enemy_anim = enemy.GetComponent<Animator>();

        //ขยับจอไปที่โปเกม่อนตัวแรก
        MoveToPosition(targetList[0].position);

        // ดึง Animator ของแต่ละโปเกม่อนมาเก็บไว้
        foreach (GameObject poke in pokemonList)
        {
            pokemonAnimList.Add(poke.GetComponent<Animator>());
        }
    }

    void Update()
    {
        if (!isEnding && (hpEnemy <= 0 || hpPlayer1 <= 0))
        {
            isEnding = true;
            endFight();
        }
    }

    // เรียกใช้ผ่าน UI Button
    public void PlayCurrentTurn(int skillID)
    {
        if (currentTurnIndex < 0 || currentTurnIndex >= pokemonList.Count)
            return;

        // Logic แต้มสกิล
        switch (skillID)
        {
            case 1:
                // สกิลธรรมดา
                break;
            case 2:
                // สกิลที่ใช้แต้ม
                break;
            default:
                Debug.Log("สกิลไม่ถูกต้อง");
                return;
        }

        // รัน animation
        string skillTrigger = "isAttack" + skillID;
        pokemonAnimList[currentTurnIndex].SetBool(skillTrigger, true);
        pokemonUIList[currentTurnIndex].SetActive(false);

        StartCoroutine(WaitAndNextTurn());
    }


    IEnumerator WaitAndNextTurn()
    {
        yield return new WaitForSeconds(2f);


        if (isEnding)
        {
            MoveToPosition(targetEnd.position);
            yield break;
        }

        currentTurnIndex++;

        if (currentTurnIndex < pokemonList.Count)
        {
            // เปิด UI ของโปเกม่อนถัดไป
            pokemonUIList[currentTurnIndex].SetActive(true);
            MoveToPosition(targetList[currentTurnIndex].position);
        }
        else
        {
            //ขยับจอไปที่บอส
            MoveToPosition(targetBoss.position);
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
                    if (Probability < 70f)   // โอกาศออก50%
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
            MoveToPosition(targetList[bossSkillTarget-1].position);
            yield return new WaitForSeconds(2f);

            string bossTrigger = "isAttack" + bossSkillTarget + bossSkillToUse;
            enemy_anim.SetBool(bossTrigger, true);

            Debug.Log("บอสใช้สกิล: " + bossSkillToUse + "กับ player " + bossSkillTarget);

            // รีเซ็ตกลับเทิร์นแรกของผู้เล่น
            currentTurnIndex = 0;

            yield return new WaitForSeconds(3f);

            // เริ่มรอบใหม่
            pokemonUIList[currentTurnIndex].SetActive(true);
            MoveToPosition(targetList[currentTurnIndex].position);
        }
    }



    //ระบบโจมตี จบเกม
    public void playerAttack(int atk)
    {
        hpEnemy -= atk;
        Debug.Log("enemy เหลือ hp " + hpEnemy);
    }

    public void enemyAttack(int atk, string target)
    {
        switch (target)
        {
            case "player1":
                hpPlayer1 -= atk;
                Debug.Log("player1 เหลือ hp " + hpPlayer1);
                break;
            case "player2":
                hpPlayer2 -= atk;
                Debug.Log("player2 เหลือ hp " + hpPlayer2);
                break;
            case "player3":
                hpPlayer3 -= atk;
                Debug.Log("player3 เหลือ hp " + hpPlayer3);
                break;
            default:
                Debug.Log("เป้าหมายไม่ถูกต้อง");
                break;
        }
    }

    public void endFight()
    {
        if (hpEnemy <= 0)
        {
            Debug.Log("Enemy defeated!");
        }
        else
        {
            Debug.Log("Player defeated!");
        }
    }

    public void resignGame()
    {
        hpPlayer1 = 0;
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