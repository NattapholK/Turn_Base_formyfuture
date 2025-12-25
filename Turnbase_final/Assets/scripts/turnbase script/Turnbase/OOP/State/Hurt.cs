using UnityEngine;

public class Hurt : StateMachine
{
    public Hurt(Character c) : base(c)
    {
        Name = STATE.HURT;
    }

    public override void Enter()
    {
        status = STATUS.ACTION;
        base.Enter();
    }
    public override void Update()
    {
        //เดี๋ยวค่อยเขียนเพิ่ม
        if(status == STATUS.ACTION) return;
        NextState = new Idle(Me);                                 
        Stage = EVENT.EXIT;
    }

    public override void Exit()
    {
        base.Exit();
    }
}