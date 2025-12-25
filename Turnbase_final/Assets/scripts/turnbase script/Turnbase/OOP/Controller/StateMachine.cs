using UnityEngine;
using UnityEngine.Events;

public class StateMachine
{
    public UnityEvent OnStageChanged;

    public enum STATE
    {
        IDLE,
        SELECT,
        ATTACK,
        SKILL,
        HURT,
        DEAD,
        END
    }

    public enum EVENT
    {
        ENTER,
        UPDATE,
        EXIT
    }

    public enum STATUS
    {
        ACTION,
        NONE
    }
    
    public STATE Name;
    protected STATUS status;
    protected EVENT Stage;
    protected StateMachine NextState;

    protected Character Me;

    public StateMachine(Character c)
    {
        Me = c;
        status = STATUS.NONE;
        Stage = EVENT.ENTER;
    }

    public void FinishAction()
    {
        status = STATUS.NONE;
    }

    //what to do when enter this stage
    public virtual void Enter()
    {
        Stage = EVENT.UPDATE;
    }
    public virtual void Update()
    {
        Stage = EVENT.UPDATE;
    }

    public virtual void Exit()
    {
        Stage = EVENT.UPDATE;
    }

    public StateMachine Process()
    {
        if (Stage == EVENT.ENTER)
        {
            Enter();
        }
        if (Stage == EVENT.UPDATE)
        {
            Update();
        }
        if(Stage == EVENT.EXIT)
        {
            Exit();
            OnStageChanged?.Invoke();
            return NextState;
        }
        return this;
    }
}
