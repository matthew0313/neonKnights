using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class Player_HpComp : HpComp, ICutsceneCallbackReceiver
{
    bool undamagable = false;
    public void OnCutsceneEnd()
    {
        undamagable = false;
    }

    public void OnCutsceneStart()
    {
        undamagable = true;
    }
    public override DamageReceivedInfo GetDamage(DamageInfo damage)
    {
        if (undamagable) return new() { receivedDamage = 0 };
        else return base.GetDamage(damage);
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(Player_HpComp))]
public class Player_HpComp_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Deal Damage")) (target as Player_HpComp).GetDamage(10.0f);
    }
}
#endif