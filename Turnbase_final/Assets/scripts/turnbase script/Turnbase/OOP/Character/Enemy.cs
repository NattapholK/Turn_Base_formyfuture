using UnityEngine;

public class Enemy : Character
{
    public int SkillCount = 1;
    [HideInInspector] public Player[] players;

    public override void Skill01()
    {
        Attack atk = _currentState as Attack;
        int target = atk.Enemy_Index;
        _Animator.SetBool("isAttack" + target + "0",true);
    }

    public override void Skill02()
    {
        Attack atk = _currentState as Attack;
        int target = atk.Enemy_Index;
        _Animator.SetBool("isAttack" + target + "1",true);
    }
}
