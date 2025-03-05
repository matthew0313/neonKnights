using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody), typeof(Shielder_HpComp), typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Shielder : Enemy, ISpeedMultiplierAdjustable, IDamageMultiplierAdjustable, IStunable
{
    [SerializeField] Transform rotator;

    [Header("RunningTowards")]
    [SerializeField] float rotateSpeed;

    [Header("Bashing")]
    [SerializeField] TriggerCallbacks bashHitbox;
    [SerializeField] float dashStartRange;
    [SerializeField] float bashRotateSpeed, bashRotateTime;
    [SerializeField] float bashRunSpeed, bashRunDistance;
    [SerializeField] float bashCooldown;
    [SerializeField] float bashDamage;
    [SerializeField] Sound bashHitSound;
    [SerializeField] float soundDistance = 30.0f;
    Rigidbody rb;
    NavMeshAgent agent;
    Animator anim;

    [Header("Debug")]
    [SerializeField] string FSMPath;

    float m_speedMultiplier = 1.0f;
    public float speedMultiplier
    {
        get
        {
            return m_speedMultiplier;
        }
        set
        {
            agent.speed = baseAgentSpeed * value;
            m_speedMultiplier = value;
        }
    }
    public float damageMultiplier { get; set; } = 1.0f;
    bool m_stunned = false;
    public bool stunned
    {
        get => m_stunned;
        set
        {
            m_stunned = value;
            if (value) topLayer.ChangeState("Stunned");
        }
    }
    public override void OnCutsceneStart()
    {
        base.OnCutsceneStart();
        agentCutsceneControl.enabled = true;
    }
    public override void OnCutsceneEnd()
    {
        base.OnCutsceneEnd();
        agentCutsceneControl.enabled = false;
    }

    ControlMachine agentControlMachine;
    ControlElement agentChaseControl;
    ControlElement agentCutsceneControl;

    TopLayer topLayer;
    float baseAgentSpeed;
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        baseAgentSpeed = agent.speed;

        agentControlMachine = new ControlMachine(() => { if (agent.isOnNavMesh) agent.isStopped = true; });
        agentChaseControl = new ControlElement(0, () => { if (agent.isOnNavMesh) agent.isStopped = false; }, false);
        agentCutsceneControl = new ControlElement(99, () => { if (agent.isOnNavMesh) agent.isStopped = true; }, false);

        anim = GetComponent<Animator>();
        topLayer = new TopLayer(this, new ShielderFSMVals());
#if UNITY_EDITOR
        topLayer.onFSMChange += () => { FSMPath = topLayer.GetFSMPath(); };
#endif
        topLayer.OnStateEnter();
    }
    void Start()
    {
        agentControlMachine.AddControls(agentChaseControl, agentCutsceneControl);
    }
    protected override void Update()
    {
        base.Update();
        if (inCutscene) return;
        topLayer.OnStateUpdate();
    }
    public override void Load(SaveData data)
    {
        agent.enabled = false;
        base.Load(data);
        if(enabled) agent.enabled = true;
    }
    class ShielderFSMVals : FSMVals
    {

    }
    class TopLayer : TopLayer<Shielder, ShielderFSMVals>
    {
        public TopLayer(Shielder origin, ShielderFSMVals values) : base(origin, values)
        {
            defaultState = new EnemyIdle<Shielder, ShielderFSMVals>(origin, this, "Detected");
            AddState("Idle", defaultState);
            AddState("Detected", new Detected(origin, this));
            AddState("Stunned", new FSM_Await<Shielder, ShielderFSMVals>(origin, this, () => !origin.stunned, () => ChangeState("Idle")));
        }
        class Detected : EnemyDetected<Shielder, ShielderFSMVals>
        {
            public Detected(Shielder origin, Layer<Shielder, ShielderFSMVals> parent) : base(origin, parent, "Idle")
            {
                defaultState = new RunningTowards(origin, this);
                AddState("RunningTowards", defaultState);
                AddState("Bashing", new Bashing(origin, this));
                AddState("Cooldown", new FSM_WaitForSeconds<Shielder, ShielderFSMVals>(origin, this, () => origin.bashCooldown, () => ChangeState("RunningTowards")));
            }
            readonly int detectedID = Animator.StringToHash("Detected");
            public override void OnStateEnter()
            {
                base.OnStateEnter();
                origin.anim.SetBool(detectedID, true);
            }
            public override void OnStateExit()
            {
                base.OnStateExit();
                origin.anim.SetBool(detectedID, false);
            }
            class RunningTowards : State<Shielder, ShielderFSMVals>
            {
                public RunningTowards(Shielder origin, Layer<Shielder, ShielderFSMVals> parent) : base(origin, parent)
                {

                }
                readonly int movingID = Animator.StringToHash("Moving");
                public override void OnStateEnter()
                {
                    base.OnStateEnter();
                    origin.anim.SetBool(movingID, true);
                    origin.agentChaseControl.enabled = true;
                }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    /*
                    Vector3 dir = (origin.player.transform.position - origin.transform.position).normalized;
                    origin.rb.linearVelocity = new Vector3(0.0f, origin.rb.linearVelocity.y, 0.0f) + new Vector3(dir.x, 0.0f, dir.z) * origin.runSpeed * origin.speedMultiplier;

                    float deg = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
                    origin.transform.rotation = Quaternion.Slerp(origin.transform.rotation, Quaternion.Euler(0, deg, 0), origin.bashRotateSpeed * Time.deltaTime);
                    */
                    origin.agent.SetDestination(origin.player.transform.position);
                    Vector3 tmp = origin.player.transform.position - origin.rotator.position;
                    float deg = Mathf.Atan2(tmp.x, tmp.z) * Mathf.Rad2Deg;
                    origin.rotator.rotation = Quaternion.Slerp(origin.rotator.rotation, Quaternion.Euler(0, deg, 0), origin.rotateSpeed * Time.deltaTime);
                    if (Vector3.Distance(origin.player.transform.position, origin.transform.position) < origin.dashStartRange && !Physics.Raycast(origin.transform.position, (origin.player.transform.position - origin.transform.position).normalized, Vector2.Distance(origin.player.transform.position, origin.transform.position), LayerMask.GetMask("Map")))
                    {
                        parentLayer.ChangeState("Bashing");
                    }
                }
                public override void OnStateExit()
                {
                    base.OnStateExit();
                    origin.anim.SetBool(movingID, false);
                    origin.agentChaseControl.enabled = false;
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
                public override void OnStateEnter()
                {
                    base.OnStateEnter();
                    origin.rotator.localRotation = Quaternion.identity;
                }

                class Rotating : State<Shielder, ShielderFSMVals>
                {
                    public Rotating(Shielder origin, Layer<Shielder, ShielderFSMVals> parent) : base(origin, parent)
                    {

                    }
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
                        Vector3 tmp = origin.transform.forward * origin.bashRunSpeed * origin.speedMultiplier;
                        origin.agent.Move(tmp * Time.deltaTime);
                        //origin.rb.linearVelocity = new Vector3(tmp.x, origin.rb.linearVelocity.y, tmp.z);
                        counter += Time.deltaTime * origin.bashRunSpeed * origin.speedMultiplier;
                        if (counter > origin.bashRunDistance)
                        {
                            parentLayer.parentLayer.ChangeState("Cooldown");
                        }
                    }
                    public override void OnStateExit()
                    {
                        base.OnStateExit();
                        origin.rb.linearVelocity = new Vector3(0.0f, origin.rb.linearVelocity.y, 0.0f);
                        origin.anim.SetBool(bashingID, false);
                        origin.bashHitbox.onTriggerEnter -= OnTriggerEnter;
                        origin.bashHitbox.gameObject.SetActive(false);
                    }
                    void OnTriggerEnter(Collider other)
                    {
                        GameObject hit = other.attachedRigidbody == null ? other.gameObject : other.attachedRigidbody.gameObject;
                        if(hit.TryGetComponent(out IDamagable tmp))
                        {
                            AudioManager.Instance.PlaySound(origin.bashHitSound).SetDist(() => origin.transform.position, origin.soundDistance);
                            tmp.GetDamage(new()
                            {
                                amount = origin.bashDamage * origin.damageMultiplier,
                                source = origin
                            });
                            parentLayer.parentLayer.ChangeState("Cooldown");
                        }
                    }
                }
            }
        }
    }
}