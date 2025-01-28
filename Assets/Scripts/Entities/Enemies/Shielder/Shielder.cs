using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Shielder_HpComp))]
public class Shielder : Enemy, ISpeedMultiplierAdjustable, IDamageMultiplierAdjustable, IStunable
{
    Shielder_HpComp hpComp;
    [SerializeField] Animator anim;
    [SerializeField] float detectRange;

    [Header("RunningTowards")]
    [SerializeField] float runSpeed;
    [SerializeField] float rotateSpeed;

    [Header("Bashing")]
    [SerializeField] TriggerCallbacks bashHitbox;
    [SerializeField] float dashStartRange;
    [SerializeField] float bashRotateSpeed, bashRotateTime;
    [SerializeField] float bashRunSpeed, bashRunDistance;
    [SerializeField] float bashCooldown;
    Rigidbody rb;

    [Header("Debug")]
    [SerializeField] string FSMPath;

    public float speedMultiplier { get; set; } = 1.0f;
    public float damageMultiplier { get; set; } = 1.0f;
    public bool stunned { get; set; } = false;

    Shielder_TopLayer topLayer;
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        hpComp = GetComponent<Shielder_HpComp>();
        topLayer = new Shielder_TopLayer(this, new ShielderFSMVals());
#if UNITY_EDITOR
        topLayer.onFSMChange += () => { FSMPath = topLayer.GetFSMPath(); };
#endif
        topLayer.OnStateEnter();
    }
    private void Update()
    {
        topLayer.OnStateUpdate();
    }
    private void FixedUpdate()
    {
        topLayer.OnStateFixedUpdate();
    }
    class ShielderFSMVals : FSMVals
    {

    }
    class Shielder_TopLayer : TopLayer<Shielder, ShielderFSMVals>
    {
        public Shielder_TopLayer(Shielder origin, ShielderFSMVals values) : base(origin, values)
        {
            defaultState = new EnemyIdle<Shielder, ShielderFSMVals>(origin, this, () => origin.detectRange, () => ChangeState("Detected"));
            AddState("Idle", defaultState);
            AddState("Detected", new Detected(origin, this));
            AddState("Stunned", new EnemyAwait<Shielder, ShielderFSMVals>(origin, this, () => !origin.stunned, () => ChangeState("Idle")));
        }
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            if (origin.stunned) ChangeState("Stunned");
        }
        class Detected : Layer<Shielder, ShielderFSMVals>
        {
            public Detected(Shielder origin, Layer<Shielder, ShielderFSMVals> parent) : base(origin, parent)
            {
                defaultState = new RunningTowards(origin, this);
                AddState("RunningTowards", defaultState);
                AddState("Bashing", new Bashing(origin, this));
                AddState("Cooldown", new EnemyWait<Shielder, ShielderFSMVals>(origin, this, () => origin.bashCooldown, () => ChangeState("RunningTowards")));
            }
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();
                if (Vector3.Distance(origin.player.transform.position, origin.transform.position) > origin.detectRange) parentLayer.ChangeState("Idle");
            }
            class RunningTowards : State<Shielder, ShielderFSMVals>
            {
                public RunningTowards(Shielder origin, Layer<Shielder, ShielderFSMVals> parent) : base(origin, parent)
                {

                }
                readonly int runningID = Animator.StringToHash("Running");
                public override void OnStateEnter()
                {
                    base.OnStateEnter();
                    origin.anim.SetBool(runningID, true);
                }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();

                    Vector3 dir = (origin.player.transform.position - origin.transform.position).normalized;
                    origin.rb.linearVelocity = new Vector3(0.0f, origin.rb.linearVelocity.y, 0.0f) + new Vector3(dir.x, 0.0f, dir.z) * origin.runSpeed * origin.speedMultiplier;

                    float deg = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                    origin.transform.rotation = Quaternion.Slerp(origin.transform.rotation, Quaternion.Euler(0, deg, 0), origin.bashRotateSpeed * Time.deltaTime);

                    if (Vector3.Distance(origin.player.transform.position, origin.transform.position) < origin.dashStartRange)
                    {
                        parentLayer.ChangeState("Bashing");
                    }
                }
                public override void OnStateExit()
                {
                    base.OnStateExit();
                    origin.anim.SetBool(runningID, false);
                }
            }
            class Bashing : Layer<Shielder, ShielderFSMVals>
            {
                public Bashing(Shielder origin, Layer<Shielder, ShielderFSMVals> parent) : base(origin, parent)
                {
                    defaultState = new Rotating(origin, this);
                    AddState("Rotating", defaultState);
                    AddState("Running", new Running(origin, this));
                }

                class Rotating : State<Shielder, ShielderFSMVals>
                {
                    public Rotating(Shielder origin, Layer<Shielder, ShielderFSMVals> parent) : base(origin, parent)
                    {

                    }
                    readonly int chargingID = Animator.StringToHash("Charging");
                    float counter = 0.0f;
                    public override void OnStateEnter()
                    {
                        base.OnStateEnter();
                        counter = 0.0f;
                    }
                    public override void OnStateUpdate()
                    {
                        base.OnStateUpdate();
                        Vector3 tmp = origin.player.transform.position - origin.transform.position;
                        float deg = Mathf.Atan2(tmp.x, tmp.z) * Mathf.Rad2Deg;
                        origin.transform.rotation = Quaternion.Slerp(origin.transform.rotation, Quaternion.Euler(0, deg, 0), origin.bashRotateSpeed * Time.deltaTime);
                        counter += Time.deltaTime;
                        if (counter > origin.bashRotateTime)
                        {
                            parentLayer.ChangeState("Running");
                        }
                    }
                }
                class Running : State<Shielder, ShielderFSMVals>
                {
                    public Running(Shielder origin, Layer<Shielder, ShielderFSMVals> parent) : base(origin, parent)
                    {

                    }
                    readonly int bashingID = Animator.StringToHash("Bashing");
                    float counter = 0.0f;
                    public override void OnStateEnter()
                    {
                        base.OnStateEnter();
                        origin.anim.SetBool(bashingID, true);
                        origin.bashHitbox.onTriggerEnter += OnTriggerEnter;
                        origin.bashHitbox.gameObject.SetActive(true);
                        counter = 0.0f;
                    }
                    public override void OnStateUpdate()
                    {
                        base.OnStateUpdate();
                        origin.rb.linearVelocity = origin.transform.forward * origin.bashRunSpeed * origin.speedMultiplier;
                        counter += Time.deltaTime * origin.bashRunSpeed * origin.speedMultiplier;
                        if (counter > origin.bashRunDistance)
                        {
                            origin.rb.linearVelocity = new Vector3(0.0f, origin.rb.linearVelocity.y, 0.0f);
                            parentLayer.parentLayer.ChangeState("Cooldown");
                        }
                    }
                    public override void OnStateExit()
                    {
                        base.OnStateExit();
                        origin.anim.SetBool(bashingID, false);
                        origin.bashHitbox.onTriggerEnter -= OnTriggerEnter;
                        origin.bashHitbox.gameObject.SetActive(false);
                    }
                    void OnTriggerEnter(Collider other)
                    {
                        GameObject hit = other.attachedRigidbody == null ? other.gameObject : other.attachedRigidbody.gameObject;
                        if(hit.TryGetComponent(out IDamagable tmp))
                        {
                            origin.rb.linearVelocity = new Vector3(0.0f, origin.rb.linearVelocity.y, 0.0f);
                            tmp.GetDamage(10.0f);
                            parentLayer.parentLayer.ChangeState("Cooldown");
                        }
                    }
                }
            }
        }
    }
}