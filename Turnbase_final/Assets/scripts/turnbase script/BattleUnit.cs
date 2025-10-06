using UnityEngine;

public class BattleUnit
{
    public int speed;
    public Animator animator;
    public GameObject uiObj;
    public Transform targetPos;
    public RectTransform proFileUI;
    public bool isBoss; // true = Boss, false = Player
    public int index;
    public bool isdied;
}