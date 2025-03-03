using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;
using UnityEngine.UIElements;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent), typeof(Slasher_HpComp))]
public class Slasher : Enemy, ISpeedMultiplierAdjustable, IDamageMultiplierAdjustable, IStunable
{
    [SerializeField] TriggerCallbacks swordHitbox;
    [SerializeField] float slashTriggerDistance;
    [SerializeField] float slashDamage;
    [SerializeField] float rotateSpeed = 3.0f;
    [SerializeField] Sound slashSound, hitSound;
    [SerializeField] float soundDistance = 20.0f;

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

    Animator anim;
    NavMeshAgent agent;
    TopLayer topLayer;
    Slasher_HpComp hp;
    float baseAgentSpeed = 0.0f;
    private void OnDisable()
    {
        agentCutsceneControl.enabled = true;
    }
    private void OnEnable()
    {
        agentCutsceneControl.enabled = false;
    }

    ControlMachine agentControlMachine;
    ControlElement agentChaseControl;
    ControlElement agentCutsceneControl;
    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        baseAgentSpeed = agent.speed;

        agentControlMachine = new ControlMachine(() => agent.isStopped = true);
        agentChaseControl = new ControlElement(0, () => agent.isStopped = false, false);
        agentCutsceneControl = new ControlElement(99, () => agent.isStopped = true, false);
        agentControlMachine.AddControls(agentChaseControl, agentCutsceneControl);

        anim = GetComponent<Animator>();
        foreach (var i in anim.GetBehaviours<MeleeSlashBehaviour>())
        {
            i.onSlashStart += (int comboIndex) => OnSlashStart(comboIndex);
            i.onSlashEnd += (int comboIndex) => OnSlashEnd(comboIndex);
        }
        swordHitbox.onTriggerStay += OnSwordTriggerStay;
        topLayer = new TopLayer(this, new SlasherFSMVals());
#if UNITY_EDITOR
        topLayer.onFSMChange += () => { FSMPath = topLayer.GetFSMPath(); };
#endif
        topLayer.OnStateEnter();
    }
    protected override void Update()
    {
        base.Update();
        topLayer.OnStateUpdate();
    }
    EnemySlash currentSlash;
    List<Collider> checkedColliders = new();
    void OnSlashStart(int comboIndex)
    {
        AudioManager.Instance.PlaySound(slashSound).SetDist(() => transform.position, soundDistance);
        currentSlash = new();
        checkedColliders.Clear();
    }
    void OnSlashEnd(int comboIndex)
    {
        if(currentSlash != null && currentSlash.comboIndex == comboIndex)
        {
            currentSlash = null;
        }
    }
    void OnSwordTriggerStay(Collider other)
    {
        if (currentSlash == null) return;

        if (checkedColliders.Contains(other)) return;
        checkedColliders.Add(other);

        if(other.TryGetComponent(out IDamagable tmp))
        {
            AudioManager.Instance.PlaySound(hitSound).SetDist(() => transform.position, soundDistance);
            tmp.GetDamage(slashDamage * damageMultiplier);
        }
    }
    public override void Load(SaveData data)
    {
        agent.enabled = false;
        base.Load(data);
        if(enabled) agent.enabled = true;
    }
    class EnemySlash
    {
        public int comboIndex;
    }

    class SlasherFSMVals : FSMVals
    {

    }
    class TopLayer : TopLayer<Slasher, SlasherFSMVals>
    {
        public TopLayer(Slasher origin, SlasherFSMVals values) : base(origin, values)
        {
            defaultState = new EnemyIdle<Slasher, SlasherFSMVals>(origin, this, "Detected");
            AddState("Idle", defaultState);
            AddState("Detected", new Detected(origin, this, "Idle"));
            AddState("Stunned", new FSM_Await<Slasher, SlasherFSMVals>(origin, this, () => !origin.stunned, () => ChangeState("Idle")));
        }
        class Detected : EnemyDetected<Slasher, SlasherFSMVals>
        {
            public Detected(Slasher origin, Layer<Slasher, SlasherFSMVals> parent, string idleState) : base(origin, parent, idleState)
            {
                defaultState = new Chasing(origin, this);
                AddState("Chasing", defaultState);
            }
            readonly int detectedID = Animator.StringToHash("Detected");
            readonly int slashID = Animator.StringToHash("Slash");
            public override void OnStateEnter()
            {
                base.OnStateEnter();
                origin.anim.SetBool(detectedID, true);
            }
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();
                Vector3 tmp = origin.player.transform.position - origin.transform.position;
                float deg = Mathf.Atan2(tmp.x, tmp.z) * Mathf.Rad2Deg;
                origin.transform.rotation = Quaternion.Slerp(origin.transform.rotation, Quaternion.Euler(0, deg, 0), origin.rotateSpeed * Time.deltaTime);
                if (Vector3.Distance(origin.transform.position, origin.player.transform.position) <= origin.slashTriggerDistance)
                {
                    origin.anim.SetTrigger(slashID);
                }
            }
            public override void OnStateExit()
            {
                base.OnStateExit();
                origin.anim.SetBool(detectedID, false);
            }
            class Chasing : State<Slasher, SlasherFSMVals>
            {
                public Chasing(Slasher origin, Layer<Slasher, SlasherFSMVals> parent) : base(origin, parent)
                {

                }
                readonly int movingID = Animator.StringToHash("Moving");
                public override void OnStateEnter()
                {
                    base.OnStateEnter();
                    origin.agent.SetDestination(origin.player.transform.position);
                    origin.agentChaseControl.enabled = true;
                    origin.anim.SetBool(movingID, true);
                }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    origin.agent.SetDestination(origin.player.transform.position);
                }
                public override void OnStateExit()
                {
                    base.OnStateExit();
                    origin.agentChaseControl.enabled = false;
                    origin.anim.SetBool(movingID, false);
                }
            }
        }
    }
}