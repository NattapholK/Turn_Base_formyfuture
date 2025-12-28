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
        HURT,
        DEAD
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
    protected STATUS Status;
    protected EVENT Stage;
    protected StateMachine NextState;

    protected Character Me;

    public StateMachine(Character c)
    {
        Me = c;
        Status = STATUS.NONE;
        Stage = EVENT.ENTER;
    }

    public void FinishAction()
    {
        Status = STATUS.NONE;
    }

    //what to do when enter this stage
    protected virtual void Enter()
    {
        Stage = EVENT.UPDATE;
    }
    protected virtual void Update()
    {
        Stage = EVENT.UPDATE;
    }

    protected virtual void Exit()
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
