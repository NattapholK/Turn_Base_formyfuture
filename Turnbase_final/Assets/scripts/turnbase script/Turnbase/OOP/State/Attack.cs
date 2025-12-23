using System;
using UnityEngine;

public class Attack : Control
{
    public Character Enemy;
    public enum STATUS
    {
        ACTION,
        NONE
    }
    public STATUS status;
    public Attack(Character c, Character enemy) : base(c)
    {
        Name = STATE.ATTACK;
        status = STATUS.NONE;
        Enemy = enemy;
    }

    public override void Enter()
    {
        if(Me is Player p)
        {
            if(p.GetMana() < p._Mana)
            {
                p.IncreaseMana(p._Mana / 2f);
            }
        }
        else if (Me is Enemy e)
        {
            //เดี๋ยวเขียนเพิ่ม
        }
        Me.Skill01();
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