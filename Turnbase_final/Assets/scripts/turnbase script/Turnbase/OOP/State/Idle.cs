using UnityEngine;

public class Idle : Control
{
    public Idle(Character c) : base(c)
    {
        Name = STATE.IDLE;
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Update()
    {
        if (Me.MyTurn)
        {
            NextState = new Select(Me);
            Stage = EVENT.EXIT;
        }
        if (Me._Animator.GetBool("isTakingDamage"))
        {
            NextState = new Hurt(Me);
            Stage = EVENT.EXIT;
        }
        if (Me.GetHp() <= 0)
        {
            NextState = new Dead(Me);
            Stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}