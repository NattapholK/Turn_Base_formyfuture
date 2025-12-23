using UnityEngine;

public class Player : Character
{
    public float _Mana;
    public GameObject _SkillUIObject;
    public AudioClip _FirstSkillSound;
    public AudioClip _SecondSkillSound;
    [HideInInspector] public Enemy[] enemies;

    private float Current_Mana;

    protected override void SetCurrentStatus()
    {
        base.SetCurrentStatus();
        Current_Mana = _Mana;
    }

    public void IncreaseMana(float total)
    {
        Current_Mana += total;
    }

    public void UseMana(float total)
    {
        Current_Mana -= total;
    }

    public float GetMana()
    {
        return Current_Mana;
    }
}
