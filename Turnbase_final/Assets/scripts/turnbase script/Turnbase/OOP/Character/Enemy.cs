using UnityEngine;

public class Enemy : Character
{
    [Header("Boss prof.")]
    public int SkillCount = 1;
    [HideInInspector] public Player[] players;

    public override void Skill01()
    {
        Debug.Log("Skill01 ทำงาน" + _Character);
        Attack atk = _currentState as Attack;
        int target = atk.Enemy_Index + 1;
        _Animator.SetBool("isAttack" + target + "0",true);
    }

    public override void Skill02()
    {
        Debug.Log("Skill02 ทำงาน" + _Character);
        Attack atk = _currentState as Attack;
        int target = atk.Enemy_Index  + 1;
        _Animator.SetBool("isAttack" + target + "1",true);
    }

    public override void TakeDamage(float Damage)
    {
        float Currend_Dmg = Damage * (100 / (100 + Current_Def));
        Current_Hp -= Currend_Dmg;

        StartCoroutine(_Manager.setDmgTextUI((int)Currend_Dmg));
        ShowFloatingText((int)Currend_Dmg, 4.2f);
        base.TakeDamage(Damage);
    }

    public void ResetRotationAfterAttack() //Animation Event ที่ใช้เฉพาะกับ animation หนูเท่านั้น เซ็ตแล้ว ไม่มีปัญหา
    {
        if (CameraManager.Instance != null && CameraManager.Instance.bossRotateTowardsTarget != null)
        {
            CameraManager.Instance.bossRotateTowardsTarget.ResetTargetAndLerpRotationToZero();
        }
    }
}
