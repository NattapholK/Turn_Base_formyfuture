using UnityEngine;

public class Skill : Attack
{
    public Skill(Character c, Character enemy, int enemy_index) : base(c, enemy, enemy_index)
    {
        Use_Skill = SKILL.SKILL01;
    }

    public override void Enter()
    {
        base.Enter();
        if(Me is Player p)
        {
            p.UseMana(p._Mana);
        }
        else if (Me is Enemy e)
        {
            //เดี๋ยวเขียนเพิ่ม
        }
        Me.Skill02();
    }
    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}