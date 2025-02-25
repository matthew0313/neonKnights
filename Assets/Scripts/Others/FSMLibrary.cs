using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FSM_WaitForSeconds<T, ValT> : State<T, ValT> where ValT : FSMVals
{
    readonly Func<float> waitTime;
    readonly Action onFinish;
    public FSM_WaitForSeconds(T origin, Layer<T, ValT> parent, Func<float> waitTime, Action onFinish) : base(origin, parent)
    {
        this.waitTime = waitTime;
        this.onFinish = onFinish;
    }
    float counter = 0.0f;
    public override void OnStateEnter()
    {
        base.OnStateEnter();
        counter = waitTime.Invoke();
    }
    public override void OnStateUpdate()
    {
        base.OnStateUpdate();
        if(counter > 0.0f)
        {
            counter -= Time.deltaTime;
            if (counter <= 0.0f) onFinish.Invoke();
        }
    }
}
public class FSM_Await<T, ValT> : State<T, ValT> where ValT : FSMVals
{
    readonly Func<bool> promise;
    readonly Action onTrue;
    public FSM_Await(T origin, Layer<T, ValT> parent, Func<bool> promise, Action onTrue) : base(origin, parent)
    {
        this.promise = promise;
        this.onTrue = onTrue;
    }
    bool invoked = false;
    public override void OnStateEnter()
    {
        base.OnStateEnter();
        invoked = false;
    }
    public override void OnStateUpdate()
    {
        base.OnStateUpdate();
        if (!invoked && promise.Invoke())
        {
            invoked = true;
            onTrue.Invoke();
        }
    }
}