using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Entity
{
    internal Player player;
    protected virtual void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    protected class EnemyIdle<T, ValT> : State<T, ValT> where ValT : FSMVals where T : Enemy
    {
        readonly Func<float> detectRangeGetter;
        readonly Action onDetect;
        public EnemyIdle(T origin, Layer<T, ValT> parent, Func<float> detectRangeGetter, Action onDetect) : base(origin, parent)
        {
            this.detectRangeGetter = detectRangeGetter;
            this.onDetect = onDetect;
        }
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            if (Vector3.Distance(origin.transform.position, origin.player.transform.position) <= detectRangeGetter.Invoke() && origin.player.statistics.canBeDetected)
            {
                onDetect?.Invoke();
            }
        }
    }
    protected class EnemyWait<T, ValT> : State<T, ValT> where ValT : FSMVals where T : Enemy
    {
        readonly Func<float> waitTimeGetter;
        readonly Action afterWait;
        public EnemyWait(T origin, Layer<T, ValT> parent, Func<float> waitTimeGetter, Action afterWait) : base(origin, parent)
        {
            this.waitTimeGetter = waitTimeGetter;
            this.afterWait = afterWait;
        }
        float counter = 0.0f;
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            counter = waitTimeGetter.Invoke();
        }
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            counter -= Time.deltaTime;
            if(counter <= 0.0f)
            {
                afterWait?.Invoke();
            }
        }
        public override void OnStateExit()
        {
            base.OnStateExit();
            counter = 0.0f;
        }
    }
    protected class EnemyAwait<T, ValT> : State<T, ValT> where ValT : FSMVals where T : Enemy
    {
        readonly Func<bool> promise;
        readonly Action onTrue;
        public EnemyAwait(T origin, Layer<T, ValT> parent, Func<bool> promise, Action onTrue) : base(origin, parent)
        {
            this.promise = promise;
            this.onTrue = onTrue;
        }
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            if (promise.Invoke()) onTrue?.Invoke();
        }
    }
}
