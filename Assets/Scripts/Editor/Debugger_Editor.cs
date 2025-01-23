using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(Debugger))]
public class Debugger_Editor : Editor
{
    public VisualTreeAsset treeAsset;
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement tmp = treeAsset.CloneTree();
        tmp.Q<Button>("AddQuirkButton").clicked += (target as Debugger).AddQuirk;
        tmp.Q<Button>("GiveQuirkShardButton").clicked += (target as Debugger).GiveQuirkShard;
        return tmp;
    }
}
