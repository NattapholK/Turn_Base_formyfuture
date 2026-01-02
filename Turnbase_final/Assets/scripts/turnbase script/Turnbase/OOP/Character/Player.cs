using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{
    public float _Mana;
    public float _EnergyReCharge = 100;

    [Header("Player UI&Sound")]
    public Image _FillSkillUIObject;
    public GameObject _SkillUIObject;
    public GameObject _ProfileUIObject;
    public AudioClip _FirstSkillSound;
    public AudioClip _SecondSkillSound;

    [HideInInspector] public Enemy[] enemies;

    public float Current_Mana{get; protected set;}

    protected override void SetCurrentStatus()
    {
        base.SetCurrentStatus();
        Current_Mana = 0;
    }

    public void IncreaseMana()
    {
        Current_Mana += 20 * (_EnergyReCharge / 100);
        _FillSkillUIObject.fillAmount = 1 - Current_Mana/_Mana;
    }

    public void UseMana()
    {
        Current_Mana -= _Mana;
        _FillSkillUIObject.fillAmount = 1 - Current_Mana/_Mana;
    }

    public void SkillSound(int skill_index)
    {
        switch (skill_index)
        {
            case 1:
                _SoundSource.PlayOneShot(_FirstSkillSound);
                break;
            case 2:
                _SoundSource.PlayOneShot(_FirstSkillSound);
                break;
            default:
                Debug.LogError("SkillSound ใส่ index ผิด");
                break;
        }
    }

    public override void Skill01()
    {
        _Animator.SetBool("isAttack1",true);
    }

    public override void Skill02()
    {
        _Animator.SetBool("isAttack2",true);
    }

    public override void Attack()
    {
        base.Attack();
        if(Current_Mana == _Mana) UseMana();
        if(Current_Mana < _Mana) IncreaseMana();
    }

    public override void TakeDamage(float Damage)
    {
        float Currend_Dmg = Damage * (100 / (100 + Current_Def));
        Current_Hp -= Currend_Dmg;

        ShowFloatingText((int)Currend_Dmg, 1f);
        base.TakeDamage(Damage);
    }
}
