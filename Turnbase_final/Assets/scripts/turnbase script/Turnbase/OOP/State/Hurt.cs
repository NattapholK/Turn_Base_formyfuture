using UnityEngine;

public class Hurt : Control
{
    public Hurt(Character c) : base(c)
    {
        Name = STATE.HURT;
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