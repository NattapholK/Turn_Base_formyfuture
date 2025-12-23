using UnityEngine;

public class Enemy : Character
{
    public int SkillCount = 1;
    [HideInInspector] public Player[] players;
}
