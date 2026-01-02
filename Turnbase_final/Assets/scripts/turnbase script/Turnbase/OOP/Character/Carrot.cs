using UnityEngine;

public class Carrot : Player
{
    public override void Skill02()
    {
        base.Skill02();
        Attack atk = _currentState as Attack;
        atk.Enemy.Bleeding();
    }
}   
