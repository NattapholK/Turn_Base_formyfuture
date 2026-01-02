using UnityEngine;

public class Idle : StateMachine
{
    public Idle(Character c) : base(c)
    {
        Name = STATE.IDLE;
    }

    protected override void Enter()
    {
        base.Enter();
    }
    protected override void Update()
    {
        if (Me.MyTurn)
        {
            if(Me.isBleeding) Me.Bleeding();
            NextState = new Select(Me);
            Stage = EVENT.EXIT;
        }

        if (Me.Current_Hp <= 0)
        {
            NextState = new Dead(Me);
            Stage = EVENT.EXIT;
        }
        else if (Me._Animator.GetBool("isTakingDamage"))
        {
            NextState = new Hurt(Me);
            Stage = EVENT.EXIT;
        }
    }

    protected override void Exit()
    {
        base.Exit();
    }
}