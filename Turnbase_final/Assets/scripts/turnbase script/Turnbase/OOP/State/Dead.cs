using UnityEngine;

public class Dead : Control
{
    public Dead(Character c) : base(c)
    {
        Name = STATE.DEAD;
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Update()
    {
        //ตายแล้ว
    }

    public override void Exit()
    {
        base.Exit();
    }
}