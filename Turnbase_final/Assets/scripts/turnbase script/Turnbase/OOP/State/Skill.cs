using UnityEngine;

public class Skill : Control
{
    public Character Enemy;
    public Skill(Character c, Character enemy) : base(c)
    {
        Name = STATE.SKILL;
        Enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Update()
    {
        if(Me is Player p)
        {
            p.UseMana(p._Mana);
            // p.Attack(Enemy);
            NextState = new Idle(Me);
            Stage = EVENT.EXIT;
        }
        else if (Me is Enemy e)
        {
            //เดี๋ยวเขียนเพิ่ม
            NextState = new Idle(Me);
            Stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}