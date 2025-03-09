using System;
using System.Collections;
using UnityEngine;
using MEC;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody), typeof(GravityCharacterController))]
public class Player_Movement : MonoBehaviour, ICutsceneCallbackReceiver, ISavable
{
    Player origin;

    [Header("Movement")]
    [SerializeField] internal float moveSpeed;
    [SerializeField] float baseMovespeedMultiplier = 1.0f;

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
    public float speed => moveSpeed * baseMovespeedMultiplier * speedMultiplier;

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
            m_stamina = Mathf.Min(stamina, maxStamina);
            onStaminaChange?.Invoke();
        }
    }
    public Action onStaminaChange;

    internal bool canMove = true;

    public bool moving { get; private set; } = false;
    public bool running { get; private set; } = false;
    public bool dashing { get; private set; } = false;
    public bool sliding { get; private set; } = false;

    RotateMode rotateMode = RotateMode.Free;

    Rigidbody rb;
    GravityCharacterController controller;

    TopLayer topLayer;

    private Player_Animation animatorController;

    public ControlMachine canMoveControl { get; private set; }
    public void OnAwake()
    {
        origin = GetComponent<Player>();
        origin.inventory.onEquippedItemChange += OnEquippedItemChange;
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<GravityCharacterController>();
        topLayer = new TopLayer(this, new MoveFSMVals());
#if UNITY_EDITOR
        topLayer.onFSMChange += () => { FSMPath = topLayer.GetFSMPath(); };
#endif
        topLayer.OnStateEnter();

        canMoveControl = new ControlMachine(() => canMove = true);
    }
    Vector3 lastLookedDir = Vector3.forward;
    internal Vector3 relativeMoveInput { get; private set; } = Vector3.forward;
    void OnEquippedItemChange(Item item)
    {
        if (item == null) rotateMode = RotateMode.Free;
        else rotateMode = item.rotateMode;
    }
    bool cutscenePaused = false;
    public void OnStart()
    {
        controller.enabled = true;
        
        /*animatorController = GetComponent<Player_Animation>();
        
        if (animatorController == null)
        {
            Debug.LogError("CharacterAnimatorController missing");
        }*/
    }
    public void OnUpdate()
    {
        if (cutscenePaused) return;
        topLayer.OnStateUpdate();
        Rotate();
        
        /*float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector2 inputVector = new Vector2(horizontalInput, verticalInput);

        if (animatorController != null)
        {
            animatorController.SetMovementInput(inputVector.x, inputVector.y);
            animatorController.UpdateInputMagniude(inputVector);
            
            if (horizontalInput != 0 || verticalInput != 0)
            {
                Debug.Log($"PlayerMovements: 입력 값 X={horizontalInput}, Y={verticalInput}");
            }
        }*/
    }
    public void OnFixedUpdate()
    {
        if (cutscenePaused) return;
        topLayer.OnStateFixedUpdate();
    }
    void Rotate()
    {
        Vector2 rot = Vector2.zero;
        if (rotateMode == RotateMode.CameraLocked)
        {
            /*Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float height = transform.position.y - ray.origin.y;
            Vector3 end = ray.origin + ray.direction * (height / ray.direction.y);
            Vector2 pos1 = new Vector2(transform.position.x, transform.position.z);
            Vector2 pos2 = new Vector2(end.x, end.z);
            rot = pos2 - pos1;*/
        }
        //else if (rotateMode == RotateMode.Free)
        //{
           // rot = new Vector2(lastLookedDir.x, lastLookedDir.z);
        //}
        rot = new Vector2(Camera.main.transform.forward.x, Camera.main.transform.forward.z);
        lastLookedDir = new Vector3(rot.x, 0.0f, rot.y);
        rotator.rotation = Quaternion.Slerp(rotator.rotation, Quaternion.Euler(0, -Mathf.Atan2(rot.y, rot.x) * Mathf.Rad2Deg + 90.0f, 0), rotateSpeed * Time.deltaTime);
    }
    CoroutineHandle dashCoroutine;
    public void Dash(Vector3 dir, float distance, float speed, float endLag, Action onFinish)
    {
        dashCoroutine = Timing.RunCoroutine(Dashing(dir, distance, speed, endLag, onFinish));
    }
    IEnumerator<float> Dashing(Vector3 dir, float distance, float speed, float endLag, Action onFinish)
    {
        dir.Normalize();
        float counter = 0.0f;
        while (counter * speed < distance)
        {
            counter += Time.deltaTime;
            controller.Move(dir * speed * Time.deltaTime);
            if (!canMove)
            {
                onFinish?.Invoke();
                yield break;
            }
            yield return Timing.WaitForOneFrame;
        }
        yield return Timing.WaitForSeconds(endLag);
        onFinish?.Invoke();
    }

    void OnDestroy()
    {
        Timing.KillCoroutines(dashCoroutine);
    }

    public void OnCutsceneStart()
    {
        cutscenePaused = true;
        rb.isKinematic = true;
        topLayer.ChangeState("InCutscene");
        Timing.KillCoroutines(dashCoroutine);
        relativeMoveInput = Vector3.zero;
    }

    public void OnCutsceneEnd()
    {
        cutscenePaused = false;
        rb.isKinematic = false;
    }

    public void Save(SaveData data)
    {
        data.playerData.floats["posX"] = transform.position.x;
        data.playerData.floats["posY"] = transform.position.y;
        data.playerData.floats["posZ"] = transform.position.z;
        data.playerData.floats["rotY"] = rotator.eulerAngles.y;
        Debug.Log("MoveSave");
    }

    public void Load(SaveData data)
    {
        float posX, posY, posZ, rotY;
        data.playerData.floats.TryGetValue("posX", out posX);
        data.playerData.floats.TryGetValue("posY", out posY);
        data.playerData.floats.TryGetValue("posZ", out posZ);
        data.playerData.floats.TryGetValue("rotY", out rotY);
        controller.enabled = false;
        transform.position = new Vector3(posX, posY, posZ);
        controller.enabled = true;
        rotator.rotation = Quaternion.Euler(0.0f, rotY, 0.0f);
    }

    class MoveFSMVals : FSMVals
    {
        public Vector3 moveInput = Vector3.zero;
        public Vector3 relativeMoveInput = Vector3.zero;
        public float staminaRegenCounter = 0.0f;
    }
    class TopLayer : TopLayer<Player_Movement, MoveFSMVals>
    {
        public TopLayer(Player_Movement origin, MoveFSMVals values) : base(origin, values)
        {
            defaultState = new NormalMovement(origin, this);
            AddState("NormalMovement", defaultState);
            AddState("InCutscene", new FSM_Await<Player_Movement, MoveFSMVals>(origin, this, () => !origin.cutscenePaused, () => ChangeState("NormalMovement")));
        }
        class NormalMovement : Layer<Player_Movement, MoveFSMVals>
        {
            public NormalMovement(Player_Movement origin, Layer<Player_Movement, MoveFSMVals> parent) : base(origin, parent)
            {
                defaultState = new Idle(origin, this);
                AddState("Idle", defaultState);
                AddState("Moving", new Moving(origin, this));
                AddState("Dashing", new Dashing(origin, this));
                AddState("Sliding", new Sliding(origin, this));
            }
            public override void OnStateUpdate()
            {
                Vector3 camFront = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;
                Vector3 camRight = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z).normalized;
                values.moveInput = camFront * Input.GetAxis("Vertical") + camRight * Input.GetAxis("Horizontal");
                values.relativeMoveInput = origin.rotator.transform.InverseTransformDirection(values.moveInput);
                base.OnStateUpdate();
            }
            class Idle : State<Player_Movement, MoveFSMVals>
            {
                public Idle(Player_Movement origin, Layer<Player_Movement, MoveFSMVals> parent) : base(origin, parent)
                {

                }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    if (origin.stamina < origin.maxStamina)
                    {
                        values.staminaRegenCounter += Time.deltaTime;
                        if (values.staminaRegenCounter >= origin.staminaRegenTick)
                        {
                            values.staminaRegenCounter = 0.0f;
                            origin.stamina = Mathf.Min(origin.maxStamina, origin.stamina + origin.staminaRegen);
                        }
                    }

                    if (values.moveInput.magnitude >= 0.1f) parentLayer.ChangeState("Moving");
                }
            }
            class Moving : Layer<Player_Movement, MoveFSMVals>
            {
                public Moving(Player_Movement origin, Layer<Player_Movement, MoveFSMVals> parent) : base(origin, parent)
                {
                    AddState("Walking", new Walking(origin, this));
                    AddState("Running", new Running(origin, this));
                }
                public override void OnStateEnter()
                {
                    if (InputManager.GetSprintInput() && origin.stamina > 0.0f && values.moveInput.magnitude >= 0.1f)
                    {
                        currentState = states["Running"];
                    }
                    else
                    {
                        currentState = states["Walking"];
                    }
                    currentState.OnStateEnter();
                    origin.moving = true;
                }
                public override void OnStateUpdate()
                {
                    if (!origin.canMove)
                    {
                        parentLayer.ChangeState("Idle");
                        return;
                    }
                    base.OnStateUpdate();

                    origin.relativeMoveInput = values.relativeMoveInput;
                    if (values.moveInput.magnitude < 0.1f) parentLayer.ChangeState("Idle");
                }
                public override void OnStateExit()
                {
                    base.OnStateExit();
                    origin.moving = false;
                }
                class Walking : State<Player_Movement, MoveFSMVals>
                {
                    public Walking(Player_Movement origin, Layer<Player_Movement, MoveFSMVals> parent) : base(origin, parent)
                    {

                    }
                    public override void OnStateUpdate()
                    {
                        base.OnStateUpdate();
                        if (values.moveInput.magnitude != 0) origin.lastLookedDir = values.moveInput.normalized;
                        Vector3 tmp = values.moveInput * origin.speed;
                        origin.controller.Move(tmp * Time.deltaTime);

                        if (InputManager.GetSprintInput() && origin.stamina > 0.0f)
                        {
                            parentLayer.ChangeState("Running");
                            return;
                        }
                        if (InputManager.GetDashInput() && origin.stamina > 0.0f)
                        {
                            origin.stamina = Mathf.Max(0, origin.stamina - 1.0f);
                            parentLayer.parentLayer.ChangeState("Dashing");
                            return;
                        }
                        if (origin.stamina < origin.maxStamina)
                        {
                            values.staminaRegenCounter += Time.deltaTime;
                            if (values.staminaRegenCounter >= origin.staminaRegenTick)
                            {
                                values.staminaRegenCounter = 0.0f;
                                origin.stamina = Mathf.Min(origin.maxStamina, origin.stamina + origin.staminaRegen);
                            }
                        }
                    }
                }
                class Running : State<Player_Movement, MoveFSMVals>
                {
                    public Running(Player_Movement origin, Layer<Player_Movement, MoveFSMVals> parent) : base(origin, parent)
                    {

                    }
                    public override void OnStateEnter()
                    {
                        base.OnStateEnter();
                        values.staminaRegenCounter = 0.0f;
                        origin.running = true;
                    }
                    public override void OnStateUpdate()
                    {
                        base.OnStateUpdate();
                        origin.stamina = Mathf.Max(0.0f, origin.stamina - origin.sprintStaminaCost * Time.deltaTime);

                        if (values.moveInput.magnitude != 0) origin.lastLookedDir = values.moveInput.normalized;
                        Vector3 tmp = values.moveInput * origin.speed * origin.sprintMultiplier;
                        origin.controller.Move(tmp * Time.deltaTime);

                        if (!InputManager.GetSprintInput() || values.moveInput.magnitude < 0.1f || origin.stamina <= 0.0f)
                        {
                            parentLayer.ChangeState("Walking");
                            return;
                        }
                        if (InputManager.GetDashInput())
                        {
                            if (origin.stamina > 1.0f)
                            {
                                origin.stamina = Mathf.Max(0.0f, origin.stamina - 2.0f);
                                parentLayer.parentLayer.ChangeState("Sliding");
                                return;
                            }
                            else
                            {
                                origin.stamina = Mathf.Max(0.0f, origin.stamina - 1.0f);
                                parentLayer.parentLayer.ChangeState("Dashing");
                                return;
                            }
                        }
                    }
                    public override void OnStateExit()
                    {
                        base.OnStateExit();
                        origin.running = false;
                    }
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
                    origin.dashing = true;
                    values.staminaRegenCounter = 0.0f;
                    origin.relativeMoveInput = values.relativeMoveInput;

                    Vector3 camFront = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;
                    Vector3 move = values.moveInput;
                    if (move.magnitude == 0) move = camFront;
                    origin.Dash(move, origin.dashDistance, origin.dashSpeed, origin.dashEndlag, () => parentLayer.ChangeState("Idle"));
                    origin.dashTrail.emitting = true;
                }
                public override void OnStateExit()
                {
                    base.OnStateExit();
                    origin.dashing = false;
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
                    origin.sliding = true;
                    values.staminaRegenCounter = 0.0f;
                    origin.relativeMoveInput = values.relativeMoveInput;

                    Vector3 camFront = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;
                    Vector3 move = values.moveInput;
                    if (move.magnitude == 0) move = camFront;
                    origin.Dash(move, origin.slideDistance, origin.slideSpeed, origin.slideEndlag, () => parentLayer.ChangeState("Idle"));
                    origin.slideTrail.emitting = true;
                    origin.sliding = true;
                }
                public override void OnStateExit()
                {
                    base.OnStateExit();
                    origin.sliding = false;
                    origin.slideTrail.emitting = false;
                }
            }
        }
        
    }
}
public enum RotateMode
{
    Free,
    CameraLocked
}
