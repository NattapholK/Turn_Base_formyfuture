using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class setattack : MonoBehaviour
{
    [Header("for player")]
    public bool isCabbage = false;
    private bool isReduceDmg = false;

    [Header("for  boss")]
    public bool isBird = false;


    [HideInInspector] public string NamePlayer;
    [HideInInspector] public bool isPlayer = true;
    [HideInInspector]public int playerAtk = 0;
    [HideInInspector] public int bossAtk = 0;
    [HideInInspector]public GameObject attackManager;

    private Animator anim;//เรียก animator ของตัวเอง
    private Animator enemyanim; //ใช้กับ player
    private List<Animator> pokeAnim = new List<Animator>(); //ใช้กับ boss
    //อื่นๆ
    private AttackSceneManager attackSceneManager;
    private StatusSystemScript statusScript;
    public void Start()
    {
        attackSceneManager = attackManager.GetComponent<AttackSceneManager>();
        statusScript = attackManager.GetComponent<StatusSystemScript>();
        anim = GetComponent<Animator>();

        if (isPlayer)
        {
            enemyanim = attackSceneManager.enemy.GetComponent<Animator>();
        }
        else
        {
            foreach (GameObject pokemon in attackSceneManager.pokemonList)
            {
                pokeAnim.Add(pokemon.GetComponent<Animator>());
            }
        }
    }

    public void OnPlayerAttackFinished()
    {

        int atk = playerAtk;
        char lastName = NamePlayer[NamePlayer.Length - 1];
        int PlayerIndex = int.Parse(lastName.ToString());
        int targetSkill = -1;

        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (anim.GetBool(param.name))
            {
                char lastChar = param.name[param.name.Length - 1];

                targetSkill = int.Parse(lastChar.ToString());
                Debug.Log("param.name = " + param.name);
                break;
            }
        }

        switch (targetSkill)
        {
            case 1:
                break;
            case 2:
                if (!isCabbage)
                {
                    atk = Mathf.RoundToInt(atk * 1.5f);
                }
                else
                {
                    atk = 0;
                    Debug.Log(gameObject.name + " ล่อศัตรู 2 เทิร์น");
                }
                break;
        }

        statusScript.playerAttack(atk);
        statusScript.useMana(PlayerIndex, targetSkill);
        //ใส่ take damage ตรงนี้
    }

    public void OnBossAttackFinished()
    {
        int atk = bossAtk;
        int targetPlayer = -1;
        int targetSkill = -1;

        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (anim.GetBool(param.name))
            {
                char lastChar = param.name[param.name.Length - 1];
                char secondlastChar = param.name[param.name.Length - 2];

                targetPlayer = int.Parse(secondlastChar.ToString());
                targetSkill = int.Parse(lastChar.ToString());
                Debug.Log("param.name = " + param.name);
                break;
            }
        }

        Debug.Log("targetPlayer = " + targetPlayer);
        Debug.Log("targetSkill = " + targetSkill);

        switch (targetSkill)
        {
            case 0:
                break;
            case 1:
                atk *= 2;
                break;
            default:
                Debug.Log("มาเช็คตรงนี้ setattack");
                break;
        }

        if (isBird && targetSkill == 1)
        {
            for (int i = 0; i < attackSceneManager.pokemonList.Count; i++)
            {
                statusScript.enemyAttack(atk, i + 1, isReduceDmg);
                if (statusScript.CurrenthpPlayerList[i] <= 0)
                {
                    pokeAnim[i].SetBool("isDie", true);
                }
                else
                {
                    pokeAnim[i].SetBool("isTakingDamage", true);
                }
            }
        }
        else
        {
            statusScript.enemyAttack(atk, targetPlayer, isReduceDmg);
            if (statusScript.CurrenthpPlayerList[targetPlayer - 1] <= 0)
            {
                pokeAnim[targetPlayer - 1].SetBool("isDie", true);
            }
            else
            {
                pokeAnim[targetPlayer - 1].SetBool("isTakingDamage", true);
            }
        }
    }

    public void OnTakeDamgeFinished()
    {
        anim.SetBool("isTakingDamage", false);
    }

    public void OnAnimFinished()
    {
        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            anim.SetBool(param.name, false);
        }
    }

    public void ChangeStateReduceDmg(bool state)
    {
        isReduceDmg = state;
    }

    public void OnPlayerDied()
    {
        anim.SetBool("isDie", false);
        gameObject.SetActive(false);
    }
}
