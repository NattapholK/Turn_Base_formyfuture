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
        //เดี๋ยวค่อยเขียนเพิ่ม
        NextState = new Idle(Me);
        Stage = EVENT.EXIT;
    }

    public override void Exit()
    {
        base.Exit();
    }
}