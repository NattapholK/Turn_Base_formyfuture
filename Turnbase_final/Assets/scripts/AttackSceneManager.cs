using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Rendering;
public class AttackSceneManager : MonoBehaviour
{
    public int hpEnemy = 16;
    public int hpPlayer = 20;
    public List<GameObject> pokemonUIList;  // UI ของโปเกม่อนแต่ละตัว
    public List<GameObject> pokemonList;    // ตัวโปเกม่อนแต่ละตัว
    public List<Transform> targetList;    // จุดที่กล้องจะมองแต่ละตัว

    public GameObject enemy;
    private Animator enemy_anim;

    public GameObject cameraObj;
          
    public float moveTime = 1.5f; // เวลาในการเคลื่อน
    public float height = 5f;     // ความสูงคงที่ของกล้อง

    private Coroutine moveCoroutine;


    private List<Animator> pokemonAnimList = new List<Animator>();

    private int currentTurnIndex = 0;

    [Header("ตั้งค่าบอส")]
    public int skillCount = 1;   // จำนวนสกิลของบอส (ลากใส่ Inspector)
    private int bossTurnCounter = 0; // ใช้สำหรับนับเทิร์นบอส

    private bool isEnding = false;
    void Start()
    {
        enemy_anim = enemy.GetComponent<Animator>();
        MoveToPosition(targetList[0].position);

        // ดึง Animator ของแต่ละโปเกม่อนมาเก็บไว้
        foreach (GameObject poke in pokemonList)
        {
            pokemonAnimList.Add(poke.GetComponent<Animator>());
        }
    }

    void Update()
    {
        if (!isEnding && (hpEnemy <= 0 || hpPlayer <= 0))
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

        currentTurnIndex++;

        if (currentTurnIndex < pokemonList.Count)
        {
            // เปิด UI ของโปเกม่อนถัดไป
            pokemonUIList[currentTurnIndex].SetActive(true);
            MoveToPosition(targetList[currentTurnIndex].position);
        }
        else
        {
            if (!isEnding)
            {
                // ถึงเทิร์นของศัตรู
                int bossSkillToUse = 1;
                if (skillCount <= 1)
                {
                    // ถ้ามีแค่ 1 สกิล
                    bossSkillToUse = 1;
                }
                else
                {
                    //รอดูโลจิกเทิร์น

                    // // เพิ่มเทิร์น ถ้าเกิน 3 ให้วนใหม่
                    // bossTurnCounter = (bossTurnCounter + 1) % 4;
                }

                string bossTrigger = "isAttack" + bossSkillToUse;
                enemy_anim.SetBool(bossTrigger, true);

                Debug.Log("บอสใช้สกิล: " + bossSkillToUse);

                // รีเซ็ตกลับเทิร์นแรกของผู้เล่น
                currentTurnIndex = 0;

                yield return new WaitForSeconds(3f);

                // เริ่มรอบใหม่
                pokemonUIList[currentTurnIndex].SetActive(true);
                MoveToPosition(targetList[currentTurnIndex].position);
            }
        }
    }



    //ระบบโจมตี จบเกม
    public void playerAttack(int atk)
    {
        hpEnemy -= atk;
        Debug.Log("enemy เหลือ hp " + hpEnemy);
    }

    public void enemyAttack(int atk)
    {
        hpPlayer -= atk;
        Debug.Log("player เหลือ hp " + hpPlayer);
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
        hpPlayer = 0;
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