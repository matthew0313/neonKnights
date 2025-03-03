using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MEC;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody), typeof(GunEnemy_HpComp))]
[RequireComponent(typeof(Animator))]
public class GunEnemy : Enemy, ISpeedMultiplierAdjustable, IDamageMultiplierAdjustable, IStunable
{
    [SerializeField] Transform firePoint;
    [SerializeField] EnemyBullet bullet;
    [SerializeField] float attackInterval = 3.0f;
    [SerializeField] float bulletDamage, bulletSpeed, bulletRange;
    [SerializeField] float rotateSpeed = 4.0f;
    [SerializeField] float maxDistance = 20.0f, minDistance = 5.0f;
    [SerializeField] float backstepSpeedMultiplier = 0.5f;
    [SerializeField] int shotCount = 1;
    [SerializeField] float spread = 0.0f;
    [SerializeField] int magSize;
    [SerializeField] float reloadTime = 3.0f;
    [SerializeField] Sound fireSound, reloadSound;
    [SerializeField] float soundDistance = 50.0f;

    [Header("Debug")]
    [SerializeField] string FSMPath;
    NavMeshAgent agent;
    Rigidbody rb;
    Animator anim;

    TopLayer topLayer;
    float baseAgentSpeed;
    float m_speedMultiplier = 1.0f;
    public float speedMultiplier
    {
        get => m_speedMultiplier;
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
    int mag = 0;
    private void OnDisable()
    {
        agentCutsceneControl.enabled = true;
    }
    private void OnEnable()
    {
        agentCutsceneControl.enabled = false;
    }

    ControlMachine agentControlMachine;
    ControlElement agentMoveControl;
    ControlElement agentCutsceneControl;
    protected override void Awake()
    {
        base.Awake();
        mag = magSize;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        baseAgentSpeed = agent.speed;

        agentControlMachine = new ControlMachine(() => agent.isStopped = true);
        agentMoveControl = new ControlElement(0, () => agent.isStopped = false, false);
        agentCutsceneControl = new ControlElement(99, () => agent.isStopped = true, false);
        agentControlMachine.AddControls(agentMoveControl, agentCutsceneControl);

        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        topLayer = new TopLayer(this, new GunEnemyFSMVals());
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
    readonly int attackID = Animator.StringToHash("Attack");
    void Fire()
    {
        AudioManager.Instance.PlaySound(fireSound).SetDist(() => transform.position, soundDistance);
        for(int i = 0; i < shotCount; i++)
        {
            EnemyBullet tmp = bullet.Instantiate() as EnemyBullet;
            tmp.transform.position = firePoint.position;
            tmp.transform.rotation = Quaternion.LookRotation((player.transform.position - firePoint.position).normalized) * Quaternion.Euler(UnityEngine.Random.Range(-spread, spread), UnityEngine.Random.Range(-spread, spread), 0.0f);
            tmp.Set(bulletDamage * damageMultiplier, bulletSpeed, bulletRange);
        }
        anim.SetTrigger(attackID);
    }
    readonly int reloadID = Animator.StringToHash("Reload");
    bool reloading = false;
    CoroutineHandle reloadCoroutine;
    void Reload()
    {
        if (reloading) return;
        Debug.Log("Reload");
        reloading = true;
        AudioManager.Instance.PlaySound(reloadSound).SetDist(() => transform.position, soundDistance);
        anim.SetTrigger(reloadID);
        reloadCoroutine = Timing.RunCoroutine(Reloading());
    }
    IEnumerator<float> Reloading()
    {
        yield return Timing.WaitForSeconds(reloadTime);
        mag = magSize;
        reloading = false;
    }
    float GetPlayerDist()
    {
        return Vector3.Distance(transform.position, player.transform.position);
    }
    bool ScanPlayer()
    {
        return !Physics.Raycast(transform.position, (player.transform.position - transform.position).normalized, Vector3.Distance(transform.position, player.transform.position), LayerMask.GetMask("Map"));
    }
    void OnDestroy()
    {
        Timing.KillCoroutines(reloadCoroutine);
    }
    public override void Load(SaveData data)
    {
        agent.enabled = false;
        base.Load(data);
        if(enabled) agent.enabled = true;
    }
    class GunEnemyFSMVals : FSMVals
    {
        public float attackCounter = 0.0f;
    }
    class TopLayer : TopLayer<GunEnemy, GunEnemyFSMVals>
    {
        public TopLayer(GunEnemy origin, GunEnemyFSMVals values) : base(origin, values)
        {
            defaultState = new EnemyIdle<GunEnemy, GunEnemyFSMVals>(origin, this, "Detected");
            AddState("Idle", defaultState);
            AddState("Detected", new Detected(origin, this, "Idle"));
            AddState("Stunned", new FSM_Await<GunEnemy, GunEnemyFSMVals>(origin, this, () => !origin.stunned, () => ChangeState("Idle")));
        }
        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
            if (values.attackCounter > 0.0f) values.attackCounter -= Time.deltaTime;
        }
        class Detected : EnemyDetected<GunEnemy, GunEnemyFSMVals>
        {
            public Detected(GunEnemy origin, Layer<GunEnemy, GunEnemyFSMVals> parent, string idleState) : base(origin, parent, idleState)
            {
                defaultState = new Idle(origin, this);
                AddState("Idle", defaultState);
                AddState("Approach", new Approach(origin, this));
                AddState("Backstep", new Backstep(origin, this));
            }
            readonly int detectedID = Animator.StringToHash("Detected");
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

                if (origin.ScanPlayer())
                {
                    if(values.attackCounter <= 0.0f)
                    {
                        if(origin.mag > 0)
                        {
                            origin.mag--;
                            origin.Fire();
                            values.attackCounter = origin.attackInterval;
                        }
                        else
                        {
                            if (!origin.reloading) origin.Reload();
                        }
                    }
                }
                else
                {
                    if (origin.mag <= origin.magSize / 2 && !origin.reloading) origin.Reload();
                }
            }
            public override void OnStateExit()
            {
                base.OnStateExit();
                origin.anim.SetBool(detectedID, false);
            }
            class Idle : State<GunEnemy, GunEnemyFSMVals>
            {
                public Idle(GunEnemy origin, Layer<GunEnemy, GunEnemyFSMVals> parent) : base(origin, parent)
                {

                }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    if(!origin.ScanPlayer() || Vector3.Distance(origin.transform.position, origin.player.transform.position) > origin.maxDistance)
                    {
                        parentLayer.ChangeState("Approach");
                    }
                    else if(Vector3.Distance(origin.transform.position, origin.player.transform.position) < origin.minDistance)
                    {
                        parentLayer.ChangeState("Backstep");
                    }
                }
            }
            class Approach : State<GunEnemy, GunEnemyFSMVals>
            {
                public Approach(GunEnemy origin, Layer<GunEnemy, GunEnemyFSMVals> parent) : base(origin, parent)
                {

                }
                readonly int movingID = Animator.StringToHash("Moving");
                public override void OnStateEnter()
                {
                    base.OnStateEnter();
                    origin.agent.SetDestination(origin.player.transform.position);
                    origin.agentMoveControl.enabled = true;
                    origin.anim.SetBool(movingID, true);
                }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    origin.agent.SetDestination(origin.player.transform.position);
                    if(origin.ScanPlayer() && origin.GetPlayerDist() <= origin.maxDistance)
                    {
                        parentLayer.ChangeState("Idle");
                    }
                }
                public override void OnStateExit()
                {
                    base.OnStateExit();
                    origin.agentMoveControl.enabled = false;
                    origin.anim.SetBool(movingID, false);
                }
            }
            class Backstep : State<GunEnemy, GunEnemyFSMVals>
            {
                public Backstep(GunEnemy origin, Layer<GunEnemy, GunEnemyFSMVals> parent) : base(origin, parent)
                {

                }
                readonly int movingID = Animator.StringToHash("Moving");
                public override void OnStateEnter()
                {
                    base.OnStateEnter();
                    Vector3 dir = (origin.transform.position - origin.player.transform.position).normalized;
                    origin.agent.SetDestination(origin.transform.position + dir * (origin.minDistance + 1.0f));
                    origin.speedMultiplier *= origin.backstepSpeedMultiplier;
                    origin.agentMoveControl.enabled = true;
                    origin.anim.SetBool(movingID, true);
                }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    Vector3 dir = (origin.transform.position - origin.player.transform.position).normalized;
                    origin.agent.SetDestination(origin.transform.position + dir * (origin.minDistance + 1.0f));
                    if(origin.GetPlayerDist() >= origin.minDistance)
                    {
                        parentLayer.ChangeState("Idle");
                    }
                    else if (!origin.ScanPlayer())
                    {
                        parentLayer.ChangeState("Approach");
                    }
                }
                public override void OnStateExit()
                {
                    base.OnStateExit();
                    origin.speedMultiplier /= origin.backstepSpeedMultiplier;
                    origin.agentMoveControl.enabled = false;
                    origin.anim.SetBool(movingID, false);
                }
            }
        }
    }
}