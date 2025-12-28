using UnityEngine;

public class Dead : StateMachine
{
    public Dead(Character c) : base(c)
    {
        Name = STATE.DEAD;
    }

    protected override void Enter()
    {
        Me._Animator.SetBool("isDie",true);
        Status = STATUS.ACTION;
        base.Enter();
    }
    protected override void Update()
    {
        //ตายแล้ว
        if(Status == STATUS.ACTION) return;
        Me._Character.SetActive(false);
    }

    protected override void Exit()
    {
        base.Exit();
    }
}