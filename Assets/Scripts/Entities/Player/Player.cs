using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player_Movement), typeof(Player_Inventory), typeof(Player_Animation))]
public class Player : Entity
{
    internal Player_Movement movement;
    internal Player_Inventory inventory;
    new internal Player_Animation animation;
    private void Awake()
    {
        movement = GetComponent<Player_Movement>();
        inventory = GetComponent<Player_Inventory>();
        animation = GetComponent<Player_Animation>();
        movement.OnAwake();
        inventory.OnAwake();
    }
    private void Start()
    {
        inventory.OnStart();
    }
    private void Update()
    {
        movement.OnUpdate();
        inventory.OnUpdate();
    }
    private void FixedUpdate()
    {
        movement.OnFixedUpdate();
    }
}
