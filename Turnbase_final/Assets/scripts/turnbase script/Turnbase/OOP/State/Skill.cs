using UnityEngine;

public class Skill : Control
{
    public Skill(Character c) : base(c)
    {
        Name = STATE.SKILL;
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Update()
    {
     
    }

    public override void Exit()
    {
        base.Exit();
    }
}