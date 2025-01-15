using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player_Movement), typeof(Player_Inventory), typeof(Player_Animation))]
[RequireComponent(typeof(Player_Statistics), typeof(Player_HpComp), typeof(Player_Camera))]
public class Player : Entity
{
    internal Player_Movement movement;
    internal Player_Inventory inventory;
    internal Player_Animation animation;
    internal Player_Statistics statistics;
    internal Player_HpComp hpComp;
    internal Player_Camera camera;
    private void Awake()
    {
        movement = GetComponent<Player_Movement>();
        inventory = GetComponent<Player_Inventory>();
        animation = GetComponent<Player_Animation>();
        statistics = GetComponent<Player_Statistics>();
        hpComp = GetComponent<Player_HpComp>();
        camera = GetComponent<Player_Camera>();
        movement.OnAwake();
        inventory.OnAwake();
        animation.OnAwake();
        camera.OnAwake();
    }
    private void Start()
    {
        inventory.OnStart();
    }
    private void Update()
    {
        movement.OnUpdate();
        inventory.OnUpdate();
        animation.OnUpdate();
        camera.OnUpdate();
    }
    private void FixedUpdate()
    {
        movement.OnFixedUpdate();
    }
}
