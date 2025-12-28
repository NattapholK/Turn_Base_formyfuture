using UnityEngine;

public class Cabbage : Player
{
    [Header("Armor")]
    public GameObject _ArmorField;
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
                _ArmorField.SetActive(false);
            }
            Passed_Turn++;
        }
    }

    public override void Skill02() //ล่าศัตรู
    {
        base.Skill02();
        isTaunt = true;
        Passed_Turn = 0;
        Current_Taunt *= 1.5f;
        Current_Def *= 2f;
        _ArmorField.SetActive(true);
    }
}