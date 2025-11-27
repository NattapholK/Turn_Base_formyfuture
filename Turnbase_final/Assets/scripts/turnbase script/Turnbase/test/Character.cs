using UnityEngine;

public class Character : MonoBehaviour
{
    public Rigidbody _Rb;
    public Animator _Animator;
    public float _HP;
    public float _ATK;
    public float _DEF;
    public float _SPEED;
    
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
