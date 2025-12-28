using UnityEngine;

public class Skill : Attack
{
    public Skill(Character c, Character enemy, int enemy_index) : base(c, enemy, enemy_index)
    {
        Use_Skill = SKILL.SKILL01;
    }

    public override void Enter()
    {
        base.Enter();
        Me.Skill02();
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