using UnityEngine;
using UnityEngine.Events;

public class Control
{
    public UnityEvent OnStageChanged;

    public enum STATE
    {
        IDLE,
        SELECT,
        ATTACK,
        SKILL,
        HURT,
        DEAD
    }

    public enum EVENT
    {
        ENTER,
        UPDATE,
        EXIT
    }
    
    public STATE Name;
    protected EVENT Stage;
    protected Control NextState;

    protected Character Me;

    public Control(Character c)
    {
        Me = c;
        Stage = EVENT.ENTER;
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

    public Control Process()
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
