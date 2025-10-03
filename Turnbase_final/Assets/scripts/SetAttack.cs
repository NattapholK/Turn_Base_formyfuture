using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class setattack : MonoBehaviour
{
    [Header("type")]
    public bool isPlayer = true;

    [Header("player")]
    public GameObject enemy;
    [Header("enemy")]
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
    private AttackSceneManager managerScript;
    public void Start()
    {
        managerScript = attackManager.GetComponent<AttackSceneManager>();
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

        if(isAttacking1)
        {
            managerScript.playerAttack(3);
            //ใส่take damage ตรงนี้
        }
        if(isAttacking2)
        {
            managerScript.playerAttack(5);
            //ใส่take damage ตรงนี้
        }

        anim.SetBool("isAttack1", false);
        anim.SetBool("isAttack2", false);
    }

    public void OnBossAttackFinished()
    {
        bool isAttacking10 = anim.GetBool("isAttack10");
        bool isAttacking20 = anim.GetBool("isAttack20");
        bool isAttacking30 = anim.GetBool("isAttack30");
        bool isAttacking11 = anim.GetBool("isAttack11");
        bool isAttacking21 = anim.GetBool("isAttack21");
        bool isAttacking31 = anim.GetBool("isAttack31");

        if(isAttacking10)
        {
            managerScript.enemyAttack(2,"player1");
            //ใส่take damage ตรงนี้
        }
        if(isAttacking20)
        {
            managerScript.enemyAttack(2,"player2");
            //ใส่take damage ตรงนี้
        }
        if(isAttacking30)
        {
            managerScript.enemyAttack(2,"player3");
            //ใส่take damage ตรงนี้
        }
        if(isAttacking11)
        {
            managerScript.enemyAttack(5,"player1");
            //ใส่take damage ตรงนี้
        }
        if(isAttacking21)
        {
            managerScript.enemyAttack(5,"player2");
            //ใส่take damage ตรงนี้
        }
        if(isAttacking31)
        {
            managerScript.enemyAttack(5,"player3");
            //ใส่take damage ตรงนี้
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
}
