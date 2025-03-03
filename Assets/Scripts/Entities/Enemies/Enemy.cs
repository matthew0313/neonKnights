using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public abstract class Enemy : Entity, IDamageSource, ICutsceneCallbackReceiver, ISavable
{
    [Header("Saving")]
    [SerializeField] protected SaveID id;

    [Header("Detection")]
    [SerializeField] float detectRange = 20.0f;
    [SerializeField] float detectionRate = 10.0f, detectionLowerRate = 5.0f;
    [SerializeField] float detectionRequired = 10.0f;
    [SerializeField] float maxDetection = 100.0f;
    [SerializeField] protected float detection = 0.0f;

    protected Player player;

    public virtual void OnCutsceneEnd()
    {
        enabled = true;
    }

    public virtual void OnCutsceneStart()
    {
        enabled = false;
    }

    protected virtual void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    protected virtual void Update()
    {
        if(Vector3.Distance(transform.position, player.transform.position) <= detectRange && !Physics.Raycast(transform.position, (player.transform.position - transform.position).normalized, Vector3.Distance(player.transform.position, transform.position), LayerMask.GetMask("Map")))
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
    public virtual void Save(SaveData data)
    {
        data.enemies[id.value] = new();
        data.enemies[id.value].bools["enabled"] = enabled;
        data.enemies[id.value].floats["detection"] = detection;
        data.enemies[id.value].floats["posX"] = transform.position.x;
        data.enemies[id.value].floats["posY"] = transform.position.y;
        data.enemies[id.value].floats["posZ"] = transform.position.z;
        data.enemies[id.value].floats["rotY"] = transform.rotation.eulerAngles.y;
        if(TryGetComponent(out Enemy_HpComp hp))
        {
            data.enemies[id.value].strings["HpComp"] = JsonUtility.ToJson(hp.Save());
            data.enemies[id.value].bools["looted"] = hp.looted;
        }
    }
    public virtual void Load(SaveData data)
    {
        if (data.enemies.TryGetValue(id.value, out DataUnit tmp))
        {
            enabled = tmp.bools["enabled"];
            detection = tmp.floats["detection"];
            transform.position = new Vector3(tmp.floats["posX"], tmp.floats["posY"], tmp.floats["posZ"]);
            transform.rotation = Quaternion.Euler(0.0f, tmp.floats["rotY"], 0.0f);
            if (TryGetComponent(out Enemy_HpComp hp))
            {
                hp.Load(JsonUtility.FromJson<HpCompSaveData>(tmp.strings["HpComp"]));
                hp.looted = tmp.bools["looted"];
            }
        }
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
}
