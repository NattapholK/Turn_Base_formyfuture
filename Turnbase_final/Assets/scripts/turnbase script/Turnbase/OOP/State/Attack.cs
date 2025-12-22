using UnityEngine;

public class Attack : Control
{
    public Attack(Character enemy) : base(enemy)
    {
        Name = STATE.ATTACK;
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Update()
    {
        // Agent.SetDestination(Me.transform.position);
        // Me.transform.LookAt(Me._player);
        
        
        // if (!Me.playerInChaseRange && !Me.playerInAttackRange)
        // {
        //     NextState = new Patrol(Me, Agent);
        //     Stage = EVENT.EXIT;
        // }
        // if (Me.playerInChaseRange && !Me.playerInAttackRange)
        // {
        //     NextState = new Chase(Me, Agent);
        //     Stage = EVENT.EXIT;
        // }
    }

    public override void Exit()
    {
        base.Exit();
    }
}