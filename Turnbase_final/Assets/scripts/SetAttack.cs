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

        }
    }

    public void OnAttackFinished()
    {
        anim.SetBool("isAttack1", false);
        if (isPlayer)
        {
            managerScript.playerAttack(5);
            enemyanim.SetBool("isTakeDamage", true);
        }
        else
        {
            managerScript.enemyAttack(2);
            poke1anim.SetBool("isTakeDamage", true);
            poke2anim.SetBool("isTakeDamage", true);
        }
    }

    public void OnTakeDamgeFinished()
    {
        anim.SetBool("isTakeDamage", false);
    }
}
