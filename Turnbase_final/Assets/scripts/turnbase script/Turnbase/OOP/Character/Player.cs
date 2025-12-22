using UnityEngine;

public class Player : Character
{
    public float _Mana;
    public AudioClip _FirstSkillSound;
    public AudioClip _SecondSkillSound;

    private float Current_Mana;

    protected override void SetCurrentStatus()
    {
        base.SetCurrentStatus();
        Current_Mana = _Mana;
    }
}
