using UnityEngine;

public class Cabbage : Player
{
    int Passed_Turn = 0;
    bool isTaunt = false;
    public override void Attack()
    {
        base.Attack();
        if (isTaunt)
        {
            if(Passed_Turn == 3)
            {
                isTaunt = false;
            }
            Passed_Turn++;
        }

    }
    public override void Skill02()
    {
        base.Skill02();
        isTaunt = true;
        Passed_Turn = 0;
        Current_Taunt *= 1.5f;
    }
}