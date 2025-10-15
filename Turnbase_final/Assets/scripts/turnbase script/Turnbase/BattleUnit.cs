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
    public int unitIndex; //ไว้เก็บ index ของ unit จะได้รู้ว่าเป็นตัวละครที่เท่าไหร่ ใช้กับกล้อง | ชั่วคราว
}