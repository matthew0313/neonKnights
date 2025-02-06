using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(Rigidbody))]
public class Player_Movement : MonoBehaviour
{
    Player origin;

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
    [SerializeField] Transform m_rotator;
    [SerializeField] internal float rotateSpeed = 1.0f;
    public Transform rotator => m_rotator;

    [Header("Debug")]
    [SerializeField] string FSMPath;

    public float speedMultiplier => origin.statistics.speedMultiplier;
    public float speed => moveSpeed * speedMultiplier;

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
        origin = GetComponent<Player>();
        origin.inventory.onEquippedItemChange += OnEquippedItemChange;
        rb = GetComponent<Rigidbody>();
        topLayer = new TopLayer(this, new MoveFSMVals());
#if UNITY_EDITOR
        topLayer.onFSMChange += () => { FSMPath = topLayer.GetFSMPath(); };
#endif
        topLayer.OnStateEnter();
    }
    Vector3 lastLookedDir = Vector3.forward;
    internal Vector3 relativeMoveInput { get; private set; } = Vector3.forward;
    void OnEquippedItemChange(Item item)
    {
        if (item == null) rotateMode = RotateMode.Free;
        else rotateMode = item.rotateMode;
    }
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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float height = transform.position.y - ray.origin.y;
            Vector3 end = ray.origin + ray.direction * (height / ray.direction.y);
            Vector2 pos1 = new Vector2(transform.position.x, transform.position.z);
            Vector2 pos2 = new Vector2(end.x, end.z);
            rot = pos2 - pos1;
            lastLookedDir = new Vector3(rot.x, 0.0f, rot.y);
        }
        else if (rotateMode == RotateMode.Free)
        {
            rot = new Vector2(lastLookedDir.x, lastLookedDir.z);
        }
        rotator.rotation = Quaternion.Slerp(rotator.rotation, Quaternion.Euler(0, -Mathf.Atan2(rot.y, rot.x) * Mathf.Rad2Deg + 90.0f, 0), rotateSpeed * Time.deltaTime);
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
            Vector3 tmp = dir * speed;
            rb.linearVelocity = new Vector3(tmp.x, rb.linearVelocity.y, tmp.z);
            //rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(endLag);
        dashing = false;
    }

    class MoveFSMVals : FSMVals
    {
        public Vector3 moveInput;
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
        public override void OnStateUpdate()
        {
            Vector3 camFront = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;
            Vector3 camRight = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z).normalized;
            values.moveInput = camFront * Input.GetAxis("Vertical") + camRight * Input.GetAxis("Horizontal");
            origin.relativeMoveInput = origin.rotator.transform.InverseTransformDirection(values.moveInput);
            base.OnStateUpdate();
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
                if (!origin.canMove) return;
                else
                {
                    if (values.moveInput.magnitude != 0) origin.lastLookedDir = values.moveInput.normalized;
                    Vector3 tmp = values.moveInput * origin.speed;
                    origin.rb.linearVelocity = new Vector3(tmp.x, origin.rb.linearVelocity.y, tmp.z);
                }
                if (InputManager.GetSprintInput() && origin.stamina > 0.0f && values.moveInput.magnitude >= 0.1f)
                {
                    parentLayer.ChangeState("Running");
                    return;
                }
                if(InputManager.GetDashInput() && origin.stamina > 0.0f && values.moveInput.magnitude > 0)
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
                if (!origin.canMove) return;
                //if(values.moveInput.magnitude != 0) origin.lastLookedDir = values.moveInput.normalized;
                //origin.rb.MovePosition(origin.rb.position + values.moveInput * origin.speed * Time.fixedDeltaTime);
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
                if (origin.canMove)
                {
                    if (values.moveInput.magnitude != 0) origin.lastLookedDir = values.moveInput.normalized;
                    Vector3 tmp = values.moveInput * origin.speed * origin.sprintMultiplier;
                    origin.rb.linearVelocity = new Vector3(tmp.x, origin.rb.linearVelocity.y, tmp.z);
                }
                if (!origin.canMove || !InputManager.GetSprintInput() || values.moveInput.magnitude < 0.1f || origin.stamina <= 0.0f)
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
                if (!origin.canMove) return;
                //if (values.moveInput.magnitude != 0) origin.lastLookedDir = values.moveInput.normalized;
                //origin.rb.MovePosition(origin.rb.position + values.moveInput * origin.speed * origin.sprintMultiplier * Time.fixedDeltaTime);
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
                Vector3 camFront = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;
                Vector3 move = values.moveInput;
                if (move.magnitude == 0) move = camFront;
                origin.Dash(move, origin.dashDistance, origin.dashSpeed, origin.dashEndlag);
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
                Vector3 camFront = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;
                Vector3 move = values.moveInput;
                if (move.magnitude == 0) move = camFront;
                origin.Dash(move, origin.slideDistance, origin.slideSpeed, origin.slideEndlag);
                origin.slideTrail.emitting = true;
                origin.sliding = true;
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
                origin.sliding = false;
            }
        }
    }
}
public enum RotateMode
{
    Free,
    MouseLocked
}
