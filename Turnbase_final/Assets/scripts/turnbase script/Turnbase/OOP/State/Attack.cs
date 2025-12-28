using System;
using UnityEngine;

public class Attack : StateMachine
{
    public Character Enemy;
    public int Enemy_Index;
    public Attack(Character c, Character enemy, int enemy_index) : base(c)
    {
        Name = STATE.ATTACK;
        Enemy = enemy;
        Enemy_Index = enemy_index;
    }

    protected override void Enter()
    {
        Debug.Log("Attack ของ "+ Me +" ทำงาน");
        Status = STATUS.ACTION;
        base.Enter();
    }

    protected override void Update()
    {
        if(Status == STATUS.ACTION) return;
        CameraShake.Instance.PresetShake_GeneralAttack();
        Me._Manager.EndTurn();
        NextState = new Idle(Me);                                 
        Stage = EVENT.EXIT;
    }

    protected override void Exit()
    {
        base.Exit();
    }
}