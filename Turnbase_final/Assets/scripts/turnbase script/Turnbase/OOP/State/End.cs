using UnityEngine;

public class End : StateMachine
{
    public End(Character c) : base(c)
    {
        Name = STATE.END;
    }

    public override void Enter()
    {
        Debug.Log("Endturn ทำงาน");
        Me._Manager.EndTurn();
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