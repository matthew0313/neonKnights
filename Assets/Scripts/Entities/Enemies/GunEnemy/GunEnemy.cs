using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody), typeof(GunEnemy_HpComp))]
[RequireComponent(typeof(Animator))]
public class GunEnemy : Enemy, ISpeedMultiplierAdjustable, IDamageMultiplierAdjustable
{
    [SerializeField] Transform firePoint;
    [SerializeField] EnemyBullet bullet;
    [SerializeField] float attackInterval = 3.0f;
    [SerializeField] float bulletDamage, bulletSpeed, bulletRange;
    [SerializeField] float rotateSpeed = 4.0f;
    [SerializeField] float maxDistance = 20.0f, minDistance = 5.0f;
    [SerializeField] float backstepSpeed = 2.0f;

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
    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        baseAgentSpeed = agent.speed;

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
        EnemyBullet tmp = bullet.Instantiate() as EnemyBullet;
        tmp.transform.position = firePoint.position;
        tmp.transform.rotation = Quaternion.LookRotation((player.transform.position - firePoint.position).normalized);
        tmp.Set(OnBulletHit, bulletSpeed, bulletRange);
        anim.SetTrigger(attackID);
    }
    protected virtual void OnBulletHit(GameObject hit)
    {
        if (hit.TryGetComponent(out IDamagable tmp)) tmp.GetDamage(bulletDamage * damageMultiplier);
    }
    float GetPlayerDist()
    {
        return Vector3.Distance(transform.position, player.transform.position);
    }
    bool ScanPlayer()
    {
        return !Physics.Raycast(transform.position, (player.transform.position - transform.position).normalized, Vector3.Distance(transform.position, player.transform.position), LayerMask.GetMask("Map"));
    }
    class GunEnemyFSMVals : FSMVals
    {

    }
    class TopLayer : TopLayer<GunEnemy, GunEnemyFSMVals>
    {
        public TopLayer(GunEnemy origin, GunEnemyFSMVals values) : base(origin, values)
        {
            defaultState = new EnemyIdle<GunEnemy, GunEnemyFSMVals>(origin, this, "Detected");
            AddState("Idle", defaultState);
            AddState("Detected", new Detected(origin, this, "Idle"));
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
            float attackCounter = 0.0f;
            readonly int detectedID = Animator.StringToHash("Detected");
            public override void OnStateEnter()
            {
                base.OnStateEnter();
                attackCounter = origin.attackInterval;
                origin.anim.SetBool(detectedID, true);
            }
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();

                Vector3 tmp = origin.player.transform.position - origin.transform.position;
                float deg = Mathf.Atan2(tmp.x, tmp.z) * Mathf.Rad2Deg;
                origin.transform.rotation = Quaternion.Slerp(origin.transform.rotation, Quaternion.Euler(0, deg, 0), origin.rotateSpeed * Time.deltaTime);

                if (attackCounter > 0.0f) attackCounter -= Time.deltaTime;
                if(attackCounter <= 0.0f)
                {
                    if(origin.ScanPlayer())
                    {
                        origin.Fire();
                        attackCounter = origin.attackInterval;
                    }
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
                    origin.agent.isStopped = false;
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
                    origin.agent.isStopped = true;
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
                    origin.anim.SetBool(movingID, true);
                }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    Vector3 tmp = origin.transform.forward * -1.0f * origin.backstepSpeed;
                    origin.rb.linearVelocity = new Vector3(tmp.x, origin.rb.linearVelocity.y, tmp.z);
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
                    origin.rb.linearVelocity = new Vector3(0.0f, origin.rb.linearVelocity.y, 0.0f);
                    origin.anim.SetBool(movingID, false);
                }
            }
        }
    }
}