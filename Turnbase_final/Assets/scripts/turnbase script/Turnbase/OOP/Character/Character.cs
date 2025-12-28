using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Character : MonoBehaviour
{
    [Header("Main-Setting")]
    public GameObject _Character;
    public AudioSource _SoundSource;
    public GameObject _TurnUI;
    public Slider _HealthSliderObject;
    public AudioClip _HurtSound;
    public Transform _CameraPos;
    public bool floattextset2 = false;

    [Header("Default-Status")]
    public float _HP;
    public float _ATK;
    public float _DEF;
    public float _SPEED;
    public float _TAUNT;

    [HideInInspector] public bool MyTurn = false;
    [HideInInspector] public Animator _Animator;  
    [HideInInspector] public Manager _Manager;
    protected float Current_Hp;
    protected float Current_Atk;
    protected float Current_Def = 0f;
    protected float Current_Speed;
    protected float Current_Taunt = 30f;
    protected StateMachine _currentState;

    protected virtual void SetCurrentStatus()
    {
        Current_Hp = _HP;
        Current_Atk = _ATK;
        Current_Def = _DEF;
        Current_Speed = _SPEED;
        Current_Taunt = _TAUNT;
    }

    public float GetHp()
    {
        return Current_Hp;
    }
    public float GetSpeed()
    {
        return Current_Speed;
    }

    public float GetTaunt()
    {
        return Current_Taunt;
    }

    public void HurtSound()
    {
        _SoundSource.PlayOneShot(_HurtSound);
    }

    void Awake()
    {
        SetCurrentStatus();
        _HealthSliderObject.interactable = false;
        _HealthSliderObject.value = Current_Hp/_HP;
        _currentState = new Idle(this);
        _Animator = _Character.GetComponent<Animator>();
        _Manager = FindAnyObjectByType<Manager>();
    }

    void FixedUpdate()
    {
        _currentState = _currentState.Process();
    }

    public virtual void Skill01(){}
    public virtual void Skill02(){}
    
    public virtual void Attack()
    {
        Debug.Log(_Character + "ไป Attack");
        Attack atk = _currentState as Attack;
        atk.Enemy.TakeDamage(Current_Atk);
    }
    public virtual void TakeDamage(float Damage)
    {
        Debug.Log(_Character + "โดน Takedamage");
        _HealthSliderObject.value = Current_Hp/_HP;
        if(Current_Hp > 0)
        {
            _Animator.SetBool("isTakingDamage", true);
        }
    }

    public void StopAction()
    {
        Debug.Log(_Character + "เลิก Action");
        foreach (AnimatorControllerParameter param in _Animator.parameters)
        {
            _Animator.SetBool(param.name, false);
        }
        _currentState.FinishAction();
        Debug.Log("###########-StopAction-#################");
    }

    public virtual void Heal(float Heal)
    {
        Current_Hp += Heal;
    }

    public void ShowFloatingText(int dmg, float radius)
    {
        Debug.Log("ShowFloatingText ทำงานแล้ว");

        // Random offset ใน world space
        Vector3 randomOffset = new Vector3(
            Random.Range(-radius, radius),
            Random.Range(0.5f, 1.5f),
            0f
        );

        Vector3 worldPos = new Vector3();

        // World position ของ player + offset
        if (floattextset2)
        {
            worldPos = _Character.transform.GetChild(0).gameObject.transform.position + randomOffset;
        }
        else
        {
            worldPos = _Character.transform.position + randomOffset;
        }

        // แปลงไปเป็น screen position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // Instantiate บน Canvas
        var go = Instantiate(_Manager._FloatingTextPrefab, _Manager._UICanvas.transform);
        go.transform.position = screenPos;

        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = dmg.ToString();

        Destroy(go, 2f);
    }

}
