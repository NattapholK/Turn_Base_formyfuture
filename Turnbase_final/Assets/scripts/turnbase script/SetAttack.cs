using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class setattack : MonoBehaviour
{
    [Header("type")]
    public bool isPlayer = true;

    [Header("สำหรับ player")]
    public bool isCabbage = false;
    private bool isReduceDmg = false;
    public int playerAtk = 3;
    public GameObject enemy;

    [Header("สำหรับ  boss")]
    public bool isBird = false;
    public int bossAtk = 2;
    public GameObject poke1;
    public GameObject poke2;
    public GameObject poke3;

    [Header("setting")]
    public GameObject attackManager;

    private Animator anim;//เรียก animator ของตัวเอง

    //ใช้กับ player
    private Animator enemyanim;

    //ใช้กับ boss
    private Animator poke1anim;
    private Animator poke2anim;
    private Animator poke3anim;

    //อื่นๆ
    private StatusSystemScript statusScript;
    public void Start()
    {
        statusScript = attackManager.GetComponent<StatusSystemScript>();
        anim = GetComponent<Animator>();
        if (isPlayer)
        {
            enemyanim = enemy.GetComponent<Animator>();
        }
        else
        {
            poke1anim = poke1.GetComponent<Animator>();
            poke2anim = poke2.GetComponent<Animator>();
            poke3anim = poke3.GetComponent<Animator>();

        }
    }

    public void OnPlayerAttackFinished()
    {
        bool isAttacking1 = anim.GetBool("isAttack1");
        bool isAttacking2 = anim.GetBool("isAttack2");

        if (isAttacking1)
        {
            switch (gameObject.name)
            {
                case "player1":
                    statusScript.useMana(1, 1);
                    break;
                case "player2":
                    statusScript.useMana(2, 1);
                    break;
                case "player3":
                    statusScript.useMana(3, 1);
                    break;
                default:
                    Debug.Log("เป้าหมายไม่ถูกต้อง");
                    break;
            }
            statusScript.playerAttack(playerAtk);
            //ใส่take damage ตรงนี้
        }
        if (isAttacking2)
        {
            switch (gameObject.name)
            {
                case "player1":
                    statusScript.useMana(1, 2);
                    //ใส่take damage ตรงนี้
                    break;
                case "player2":
                    statusScript.useMana(2, 2);
                    break;
                case "player3":
                    statusScript.useMana(3, 2);
                    //ใส่take damage ตรงนี้
                    break;
                default:
                    Debug.Log("เป้าหมายไม่ถูกต้อง");
                    break;
            }
            if (!isCabbage)
            {
                statusScript.playerAttack(Mathf.RoundToInt(playerAtk * 1.5f));
            }
            else
            {
                Debug.Log(gameObject.name + " ล่อศัตรู 2 เทิร์น");
            }
        }

        anim.SetBool("isAttack1", false);
        anim.SetBool("isAttack2", false);
    }

    public void OnBossAttackFinished()
    {
        int atk = bossAtk;

        bool isAttacking10 = anim.GetBool("isAttack10");
        bool isAttacking20 = anim.GetBool("isAttack20");
        bool isAttacking30 = anim.GetBool("isAttack30");
        bool isAttacking11 = anim.GetBool("isAttack11");
        bool isAttacking21 = anim.GetBool("isAttack21");
        bool isAttacking31 = anim.GetBool("isAttack31");

        if (isAttacking11 || isAttacking21 || isAttacking31)
        {
            if (!isBird)
            {
                if (isAttacking11)
                {
                    statusScript.enemyAttack(atk * 3, 1, isReduceDmg);
                    //ใส่take damage ตรงนี้
                }
                if (isAttacking21)
                {
                    statusScript.enemyAttack(atk * 3, 2, isReduceDmg);
                    //ใส่take damage ตรงนี้
                }
                if (isAttacking31)
                {
                    statusScript.enemyAttack(atk * 3, 3, isReduceDmg);
                    //ใส่take damage ตรงนี้
                }
            }
            else
            {
                statusScript.enemyAttack(atk * 3, 1, isReduceDmg);
                statusScript.enemyAttack(atk * 3, 2, isReduceDmg);
                statusScript.enemyAttack(atk * 3, 3, isReduceDmg);
                //ใส่take damage ตรงนี้
            }
        }
        else
        {
            if (isAttacking10)
            {
                statusScript.enemyAttack(atk, 1, isReduceDmg);
                //ใส่take damage ตรงนี้
            }
            if (isAttacking20)
            {
                statusScript.enemyAttack(atk, 2, isReduceDmg);
                //ใส่take damage ตรงนี้
            }
            if (isAttacking30)
            {
                statusScript.enemyAttack(atk, 3, isReduceDmg);
                //ใส่take damage ตรงนี้
            }
        }

        anim.SetBool("isAttack10", false);
        anim.SetBool("isAttack20", false);
        anim.SetBool("isAttack30", false);
        anim.SetBool("isAttack11", false);
        anim.SetBool("isAttack21", false);
        anim.SetBool("isAttack31", false);
    }

    public void OnTakeDamgeFinished()
    {
        anim.SetBool("isTakeDamage", false);
    }

    public void ChangeStateReduceDmg(bool state)
    {
        isReduceDmg = state;
    } 
}
