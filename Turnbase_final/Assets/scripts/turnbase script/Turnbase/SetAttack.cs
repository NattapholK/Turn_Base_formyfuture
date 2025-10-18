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

    [Header("Other Sound (Optional)")]
    public AudioClip otherSound1;


    [HideInInspector] public AudioSource PlayerAudioSource; 
    [HideInInspector] public List<AudioClip> skillSound = new List<AudioClip>();
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
    private AudioClip takeDamageSound;
    private char lastName;
    private int PlayerIndex;
    public void Start()
    {
        attackSceneManager = attackManager.GetComponent<AttackSceneManager>();
        statusScript = attackSceneManager.GetComponent<StatusSystemScript>();
        if (attackSceneManager.PlayerAudioSource != null)
        {
            PlayerAudioSource = attackSceneManager.PlayerAudioSource;
        }
        anim = GetComponent<Animator>();

        if (isPlayer)
        {
            lastName = NamePlayer[NamePlayer.Length - 1];
            PlayerIndex = int.Parse(lastName.ToString());
            takeDamageSound = attackSceneManager.playerData[PlayerIndex - 1].takeDamageSound;
            var skill1Sound = attackSceneManager.playerData[PlayerIndex - 1].skill1Sound;
            var skill2Sound = attackSceneManager.playerData[PlayerIndex - 1].skill2Sound;
            if (skill1Sound != null)
            {
                skillSound.Add(skill1Sound);
            }
            if (skill2Sound != null)
            {
                skillSound.Add(skill2Sound);
            }
            enemyanim = attackSceneManager.enemy.GetComponent<Animator>();
        }
        else
        {
            for (int i = 0; i < attackSceneManager.playerData.Count; i++)
            {
                pokeAnim.Add(attackSceneManager.playerData[i].playerObject.GetComponent<Animator>());
            }
        }
    }

    public void OnPlayerAttackFinished()
    {
        int atk = playerAtk;
        int targetSkill = -1;

        CameraShake.Instance.ExecuteCameraShakeWithThisAnimation(anim);

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
                statusScript.playerAttack(atk);
                break;
            case 2:
                if (!isCabbage)
                {
                    atk = Mathf.RoundToInt(atk * 1.5f);
                    statusScript.playerAttack(atk);
                }
                else
                {
                    Debug.Log(gameObject.name + " ล่อศัตรู 2 เทิร์น");
                }
                break;
        }

        statusScript.useMana(PlayerIndex, targetSkill);

        if (statusScript.CurrenthpEnemy <= 0)
        {
            enemyanim.SetBool("isDie", true);
        }
        else
        {
            enemyanim.SetBool("isTakingDamage", true);
        }
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
            for (int i = 0; i < attackSceneManager.playerData.Count; i++)
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
        CameraShake.Instance.PresetShake_GeneralAttack();
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

    public void SoundEffectSkill()
    {
        int targetSkill = -1;
        string parameterName = "Idle";

        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (anim.GetBool(param.name))
            {
                char lastChar = param.name[param.name.Length - 1];

                if (int.TryParse(lastChar.ToString(), out int result))
                {
                    targetSkill = result;
                    Debug.Log("param.name = " + param.name);
                    break;
                }
                else
                {
                    Debug.LogWarning("ตัวอักษรสุดท้ายของ param.name ไม่ใช่ตัวเลข: " + param.name); // เอาไว้เผื่อใส่เอฟเฟตตอนตาย
                    parameterName = param.name;
                }
            }
        }

        if (targetSkill == -1) // sound ตอนตายใส่ตรงนี้
        {
            if (takeDamageSound != null)
            {
                PlayerAudioSource.PlayOneShot(takeDamageSound);
            }

        }
        else
        {
            if (targetSkill > 0 && targetSkill - 1 < skillSound.Count)
            {
                PlayerAudioSource.PlayOneShot(skillSound[targetSkill - 1]);
            }
        }
    }
    
    public void OtherSoundEffectSkill()
    {
        if(otherSound1 != null)
        {
            PlayerAudioSource.PlayOneShot(otherSound1);
        } 
    }
}
