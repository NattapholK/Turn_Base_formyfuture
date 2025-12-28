using UnityEngine;

public class NormalAttack : Attack
{
    public NormalAttack(Character c, Character enemy, int enemy_index) : base(c, enemy, enemy_index)
    {
        Use_Skill = SKILL.NORMAL_ATTACK;
    }

    public override void Enter()
    {
        base.Enter();
        Me.Skill01();
    }
    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}