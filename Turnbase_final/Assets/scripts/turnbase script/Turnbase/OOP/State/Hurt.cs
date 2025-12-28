using UnityEngine;

public class Hurt : StateMachine
{
    public Hurt(Character c) : base(c)
    {
        Name = STATE.HURT;
    }

    protected override void Enter()
    {
        Status = STATUS.ACTION;
        base.Enter();
    }
    protected override void Update()
    {
        //เดี๋ยวค่อยเขียนเพิ่ม
        if(Status == STATUS.ACTION) return;
        NextState = new Idle(Me);                                 
        Stage = EVENT.EXIT;
    }

    protected override void Exit()
    {
        base.Exit();
    }
}