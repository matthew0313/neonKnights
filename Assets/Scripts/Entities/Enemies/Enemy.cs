using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

[RequireComponent(typeof(Enemy_HpComp))]
public abstract class Enemy : Entity, IDamageSource, ICutsceneCallbackReceiver
{
    [SerializeField] protected Animator anim;
    internal HpComp hpComp { get; private set; }

    [Header("Detection")]
    [SerializeField] float detectRange = 20.0f;
    [SerializeField] float detectionRate = 10.0f, detectionLowerRate = 5.0f;
    [SerializeField] float detectionRequired = 10.0f;
    [SerializeField] float maxDetection = 100.0f;
    [SerializeField] protected float detection = 0.0f;

    internal Player player;

    public void OnCutsceneEnd()
    {
        enabled = true;
    }

    public void OnCutsceneStart()
    {
        enabled = false;
    }

    protected virtual void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        hpComp = GetComponent<Enemy_HpComp>();
    }
    protected virtual void Update()
    {
        if(Vector3.Distance(transform.position, player.transform.position) <= detectRange)
        {
            detection = Mathf.Min(detection + detectionRate * Time.deltaTime, maxDetection);
        }
        else
        {
            detection = Mathf.Max(detection - detectionLowerRate * Time.deltaTime, 0.0f);
        }
    }
    public void AddDetection(float amount)
    {
        detection = Mathf.Min(detection + amount, maxDetection);
    }
    protected class EnemyIdle<T, ValT> : State<T, ValT> where ValT : FSMVals where T : Enemy
    {
        readonly string detectedState;
        public EnemyIdle(T origin, Layer<T, ValT> parent, string detectedState) : base(origin, parent)
        {
            this.detectedState = detectedState;
        }
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            if(origin.detection >= origin.detectionRequired)
            {
                parentLayer.ChangeState(detectedState);
            }
        }
    }
    protected class EnemyDetected<T, ValT> : Layer<T, ValT> where ValT : FSMVals where T : Enemy
    {
        readonly string idleState;
        public EnemyDetected(T origin, Layer<T, ValT> parent, string idleState) : base(origin, parent)
        {
            this.idleState = idleState;
        }
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            if(origin.detection < origin.detectionRequired)
            {
                parentLayer.ChangeState(idleState);
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
