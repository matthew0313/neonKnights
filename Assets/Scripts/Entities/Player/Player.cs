using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player_Movement), typeof(Player_Inventory), typeof(Player_Animation))]
[RequireComponent(typeof(Player_Statistics), typeof(Player_HpComp), typeof(Player_Camera))]
[RequireComponent(typeof(Player_Cooldowns), typeof(Player_Interaction))]
public class Player : Entity, IDamageSource
{
    internal Player_Movement movement;
    internal Player_Inventory inventory;
    internal Player_Animation animation;
    internal Player_Statistics statistics;
    internal Player_HpComp hpComp;
    internal Player_Camera camera;
    internal Player_Cooldowns cooldowns;
    internal Player_Interaction interaction;
    private void Awake()
    {
        movement = GetComponent<Player_Movement>();
        inventory = GetComponent<Player_Inventory>();
        animation = GetComponent<Player_Animation>();
        statistics = GetComponent<Player_Statistics>();
        hpComp = GetComponent<Player_HpComp>();
        camera = GetComponent<Player_Camera>();
        cooldowns = GetComponent<Player_Cooldowns>();
        interaction = GetComponent<Player_Interaction>();
        movement.OnAwake();
        inventory.OnAwake();
        animation.OnAwake();
        camera.OnAwake();
        cooldowns.OnAwake();
    }
    private void Start()
    {
        inventory.OnStart();
        animation.OnStart();
    }
    private void Update()
    {
        inventory.OnUpdate();
        cooldowns.OnUpdate();
        movement.OnUpdate();
        animation.OnUpdate();
        camera.OnUpdate();
        interaction.OnUpdate();
    }
    private void FixedUpdate()
    {
        movement.OnFixedUpdate();
    }
}
