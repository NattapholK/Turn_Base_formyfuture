using System;
using UnityEngine;

public class Attack : Control
{
    public Character Enemy;
    public int Enemy_Index;
    public enum STATUS
    {
        ACTION,
        NONE
    }
    public enum SKILL
    {
        NORMAL_ATTACK,
        SKILL01
    }
    public SKILL Use_Skill;
    public STATUS status;
    public Attack(Character c, Character enemy, int enemy_index) : base(c)
    {
        Name = STATE.ATTACK;
        status = STATUS.NONE;
        Enemy = enemy;
        Enemy_Index = enemy_index;
    }

    public override void Enter()
    {
        status = STATUS.ACTION;
        base.Enter();
    }

    public override void Update()
    {
        if(status == STATUS.ACTION) return;
        NextState = new Idle(Me);                                 
        Stage = EVENT.EXIT;
    }

    public override void Exit()
    {

        base.Exit();
    }

    public void FinishAction()
    {
        status = STATUS.NONE;
    }
}