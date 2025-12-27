using System;
using UnityEngine;

public class Attack : StateMachine
{
    public Character Enemy;
    public int Enemy_Index;
    public enum SKILL
    {
        NORMAL_ATTACK,
        SKILL01
    }
    public SKILL Use_Skill;
    public Attack(Character c, Character enemy, int enemy_index) : base(c)
    {
        Name = STATE.ATTACK;
        Enemy = enemy;
        Enemy_Index = enemy_index;
    }

    public override void Enter()
    {
        Debug.Log("Attack ของ "+ Me +" ทำงาน");
        status = STATUS.ACTION;
        base.Enter();
    }

    public override void Update()
    {
        if(status == STATUS.ACTION) return;
        CameraShake.Instance.PresetShake_GeneralAttack();
        NextState = new End(Me);                                 
        Stage = EVENT.EXIT;
    }

    public override void Exit()
    {
        base.Exit();
    }
}