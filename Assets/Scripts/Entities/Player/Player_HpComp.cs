using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
public class Player_HpComp : HpComp
{

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