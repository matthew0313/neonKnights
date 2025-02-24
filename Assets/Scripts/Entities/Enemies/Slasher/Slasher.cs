using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent), typeof(Slasher_HpComp))]
public class Slasher : Enemy
{
    [SerializeField] TriggerCallbacks swordHitbox;
    [SerializeField] float slashTriggerDistance;
    [SerializeField] float slashDamage;

    [Header("Debug")]
    [SerializeField] string FSMPath;


    Animator anim;
    NavMeshAgent agent;
    TopLayer topLayer;
    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
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
            tmp.GetDamage(slashDamage);
        }
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
                if(Vector3.Distance(origin.transform.position, origin.player.transform.position) <= origin.slashTriggerDistance)
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
                    origin.agent.isStopped = false;
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
                    origin.agent.isStopped = true;
                    origin.anim.SetBool(movingID, false);
                }
            }
        }
    }
}