using Unity.VisualScripting;
using UnityEngine;

public class Character : MonoBehaviour
{
    public GameObject _Character;
    public Manager _Manager;
    public GameObject _TurnUI;
    public AudioClip _HurtSound;
    public Transform _CameraPos;

    public float _HP;
    public float _ATK;
    public float _DEF;
    public float _SPEED;

    [HideInInspector] public bool MyTurn = false;
    [HideInInspector] public Animator _Animator;  
    [HideInInspector] public AudioSource _SoundSource;
    private Rigidbody _Rb;
    private float Current_Hp;
    private float Current_Atk;
    private float Curren_Def;
    private float Current_Speed;
    private Control _currentState;

    protected virtual void SetCurrentStatus()
    {
        Current_Hp = _HP;
        Current_Atk = _ATK;
        Curren_Def = _DEF;
        Current_Speed = _SPEED;
    }

    public float GetHp()
    {
        return Current_Hp;
    }
    public float GetSpeed()
    {
        return Current_Speed;
    }

    void Awake()
    {
        _currentState = new Idle(this);
        SetCurrentStatus();
    }

    void FixedUpdate()
    {
        _currentState = _currentState.Process();
    }
    
    public virtual void Attack(Character Enemy)
    {
        Enemy.TakeDamage(this._ATK);
    }

    public virtual void TakeDamage(float Damage)
    {
        this._HP -= Damage;
    }

    public virtual void Heal(float Heal)
    {
        this._HP += Heal;
    }

}
