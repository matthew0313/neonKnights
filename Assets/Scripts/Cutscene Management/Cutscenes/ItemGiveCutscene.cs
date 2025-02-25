using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiveCutscene : Cutscene
{
    [SerializeField] LootGen given;
    Player player;
    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    public override void End()
    {
        base.End();
        foreach(var i in given.GetLoot())
        {
            player.inventory.AddItem_DropRest(i.item, i.count);
        }
    }
}