using UnityEngine;

public class Select : Control
{
    public Select(Character c) : base(c)
    {
        Name = STATE.SELECT;
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