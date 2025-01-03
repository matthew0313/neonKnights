using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(Rigidbody))]
public class Player_Movement : MonoBehaviour, ISpeedAdjustable
{
    [Header("Movement")]
    [SerializeField] internal float moveSpeed;

    [Header("Walk")]
    [SerializeField] internal float staminaRegen = 1.0f;
    [SerializeField] internal float staminaRegenTick = 1.0f;

    [Header("Sprint")]
    [SerializeField] internal float sprintMultiplier = 1.5f;
    [SerializeField] float m_maxStamina = 2.0f;
    [SerializeField] internal float sprintStaminaCost = 0.5f;

    [Header("Dash")]
    [SerializeField] internal float dashStaminaCost = 1.0f;
    [SerializeField] internal float dashDistance = 3.0f;
    [SerializeField] internal float dashSpeed = 15.0f;
    [SerializeField] internal float dashEndlag = 0.1f;
    [SerializeField] TrailRenderer dashTrail;

    [Header("Slide")]
    [SerializeField] internal float slideStaminaCost = 2.0f;
    [SerializeField] internal float slideDistance = 10.0f;
    [SerializeField] internal float slideSpeed = 20.0f;
    [SerializeField] internal float slideEndlag = 0.2f;
    [SerializeField] TrailRenderer slideTrail;

    [Header("Rotate")]
    [SerializeField] internal float rotateSpeed = 1.0f;

    [Header("Debug")]
    [SerializeField] string FSMPath;

    public float speed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    float m_stamina = 2.0f;
    internal float stamina
    {
        get
        {
            return m_stamina;
        }
        set
        {
            m_stamina = value;
            onStaminaChange?.Invoke();
        }
    }
    internal float maxStamina
    {
        get
        {
            return m_maxStamina;
        }
        set
        {
            m_maxStamina = value;
            onStaminaChange?.Invoke();
        }
    }
    public Action onStaminaChange;

    internal bool canMove = true;
    internal bool sprinting = false;
    internal bool dashing = false;
    internal bool sliding = false;
    internal RotateMode rotateMode = RotateMode.Free;
    Rigidbody rb;
    TopLayer topLayer;
    public void OnAwake()
    {
        rb = GetComponent<Rigidbody>();
        topLayer = new TopLayer(this, new MoveFSMVals());
#if UNITY_EDITOR
        topLayer.onFSMChange += () => { FSMPath = topLayer.GetFSMPath(); };
#endif
        topLayer.OnStateEnter();
    }
    Vector3 lastMovedDir = Vector3.forward;
    public void OnUpdate()
    {
        topLayer.OnStateUpdate();
        Rotate();
    }
    public void OnFixedUpdate()
    {
        topLayer.OnStateFixedUpdate();
    }
    void Rotate()
    {
        Vector2 rot = Vector2.zero;
        if (rotateMode == RotateMode.MouseLocked)
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.GetMouseInput());
            float height = transform.position.y - ray.origin.y;
            Vector3 end = ray.origin + ray.direction * (height / ray.direction.y);
            Vector2 pos1 = new Vector2(transform.position.x, transform.position.z);
            Vector2 pos2 = new Vector2(end.x, end.z);
            rot = pos2 - pos1;
            lastMovedDir = new Vector3(rot.x, 0.0f, rot.y);
        }
        else if (rotateMode == RotateMode.Free)
        {
            rot = new Vector2(lastMovedDir.x, lastMovedDir.z);
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, -Mathf.Atan2(rot.y, rot.x) * Mathf.Rad2Deg + 90.0f, 0), rotateSpeed * Time.deltaTime);
    }
    public void Dash(Vector3 dir, float distance, float speed, float endLag)
    {
        StartCoroutine(Dashing(dir, distance, speed, endLag));
    }
    IEnumerator Dashing(Vector3 dir, float distance, float speed, float endLag)
    {
        dashing = true;
        dir.Normalize();
        float counter = 0.0f;
        while (counter < distance)
        {
            counter += Time.deltaTime * speed;
            rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(endLag);
        dashing = false;
    }

    class MoveFSMVals : FSMVals
    {

    }
    class TopLayer : TopLayer<Player_Movement, MoveFSMVals>
    {
        public TopLayer(Player_Movement origin, MoveFSMVals values) : base(origin, values)
        {
            defaultState = new Walking(origin, this);
            AddState("Walking", defaultState);
            AddState("Running", new Running(origin, this));
            AddState("Dashing", new Dashing(origin, this));
            AddState("Sliding", new Sliding(origin, this));
        }
        class Walking : State<Player_Movement, MoveFSMVals>
        {
            public Walking(Player_Movement origin, Layer<Player_Movement, MoveFSMVals> parent) : base(origin, parent)
            {

            }
            float staminaRegenCounter = 0.0f;
            public override void OnStateEnter()
            {
                base.OnStateEnter();
                staminaRegenCounter = 0.0f;
            }
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();
                if (!origin.canMove || origin.dashing) return;
                if (InputManager.GetSprintInput() && origin.stamina > 0.0f)
                {
                    parentLayer.ChangeState("Running");
                    return;
                }
                if(InputManager.GetDashInput() && origin.stamina > 0.0f)
                {
                    origin.stamina = Mathf.Max(0, origin.stamina - 1.0f);
                    parentLayer.ChangeState("Dashing");
                    return;
                }
                if(origin.stamina < origin.maxStamina)
                {
                    staminaRegenCounter += Time.deltaTime;
                    if (staminaRegenCounter >= origin.staminaRegenTick)
                    {
                        staminaRegenCounter = 0.0f;
                        origin.stamina = Mathf.Min(origin.maxStamina, origin.stamina + origin.staminaRegen);
                    }
                }
            }
            public override void OnStateFixedUpdate()
            {
                base.OnStateFixedUpdate();
                if (!origin.canMove || origin.dashing) return;
                Vector3 camFront = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;
                Vector3 camRight = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z).normalized;
                Vector3 move = camFront * InputManager.GetMovementInput().y + camRight * InputManager.GetMovementInput().x;
                if(move.magnitude != 0) origin.lastMovedDir = move.normalized;
                origin.rb.MovePosition(origin.rb.position + move * origin.moveSpeed * Time.fixedDeltaTime);
            }
        }
        class Running : State<Player_Movement, MoveFSMVals>
        {
            public Running(Player_Movement origin, Layer<Player_Movement, MoveFSMVals> parent) : base(origin, parent)
            {

            }
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();
                origin.stamina = Mathf.Max(0.0f, origin.stamina - origin.sprintStaminaCost * Time.deltaTime);
                if(!origin.canMove || origin.dashing || !InputManager.GetSprintInput() || origin.stamina <= 0.0f)
                {
                    parentLayer.ChangeState("Walking");
                    return;
                }
                if (InputManager.GetDashInput())
                {
                    if(origin.stamina > 1.0f)
                    {
                        origin.stamina = Mathf.Max(0.0f, origin.stamina - 2.0f);
                        parentLayer.ChangeState("Sliding");
                        return;
                    }
                    else
                    {
                        origin.stamina = Mathf.Max(0.0f, origin.stamina - 1.0f);
                        parentLayer.ChangeState("Dashing");
                        return;
                    }
                }
            }
            public override void OnStateFixedUpdate()
            {
                base.OnStateFixedUpdate();
                if (!origin.canMove || origin.dashing) return;
                Vector3 camFront = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;
                Vector3 camRight = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z).normalized;
                Vector3 move = camFront * InputManager.GetMovementInput().y + camRight * InputManager.GetMovementInput().x;
                if(move.magnitude != 0) origin.lastMovedDir = move.normalized;
                origin.rb.MovePosition(origin.rb.position + move * origin.moveSpeed * origin.sprintMultiplier * Time.fixedDeltaTime);
            }
        }
        class Dashing : State<Player_Movement, MoveFSMVals>
        {
            public Dashing(Player_Movement origin, Layer<Player_Movement, MoveFSMVals> parent) : base(origin, parent)
            {

            }
            public override void OnStateEnter()
            {
                base.OnStateEnter();
                origin.Dash(origin.lastMovedDir, origin.dashDistance, origin.dashSpeed, origin.dashEndlag);
                origin.dashTrail.emitting = true;
            }
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();
                if (origin.dashing == false) parentLayer.ChangeState("Walking");
            }
            public override void OnStateExit()
            {
                base.OnStateExit();
                origin.dashTrail.emitting = false;
            }
        }
        class Sliding : State<Player_Movement, MoveFSMVals>
        {
            public Sliding(Player_Movement origin, Layer<Player_Movement, MoveFSMVals> parent) : base(origin, parent)
            {

            }
            public override void OnStateEnter()
            {
                base.OnStateEnter();
                origin.Dash(origin.lastMovedDir, origin.slideDistance, origin.slideSpeed, origin.slideEndlag);
                origin.slideTrail.emitting = true;
            }
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();
                if (origin.dashing == false) parentLayer.ChangeState("Walking");
            }
            public override void OnStateExit()
            {
                base.OnStateExit();
                origin.slideTrail.emitting = false;
            }
        }
    }
}
public enum RotateMode
{
    Free,
    MouseLocked
}
