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
    protected Control _currentState;

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
        SetCurrentStatus();
        _currentState = new Idle(this);
        _Animator = _Character.GetComponent<Animator>();
        _SoundSource = _Character.GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        _currentState = _currentState.Process();
    }

    public virtual void Skill01(){}
    public virtual void Skill02(){}
    
    public virtual void Attack()
    {
        Attack atk = _currentState as Attack;
        atk.Enemy.TakeDamage(Current_Atk);
        atk.FinishAction();
    }
    protected virtual void TakeDamage(float Damage)
    {
        Current_Hp -= Damage;
        _Animator.SetBool("isTakingDamage", true);
    }

    public virtual void Heal(float Heal)
    {
        Current_Hp += Heal;
    }

}
