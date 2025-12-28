using UnityEngine;

public class Skill : Attack
{
    public Skill(Character c, Character enemy, int enemy_index) : base(c, enemy, enemy_index){}

    protected override void Enter()
    {
        base.Enter();
        Me.Skill02();
    }
    protected override void Update()
    {
        base.Update();
    }

    protected override void Exit()
    {
        base.Exit();
    }
}