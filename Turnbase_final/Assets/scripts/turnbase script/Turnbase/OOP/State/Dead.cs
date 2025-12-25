using UnityEngine;

public class Dead : StateMachine
{
    public Dead(Character c) : base(c)
    {
        Name = STATE.DEAD;
    }

    public override void Enter()
    {
        Me._Animator.SetBool("isDie",true);
        status = STATUS.ACTION;
        base.Enter();
    }
    public override void Update()
    {
        //ตายแล้ว
        if(status == STATUS.ACTION) return;
        Me._Character.SetActive(false);
    }

    public override void Exit()
    {
        base.Exit();
    }
}