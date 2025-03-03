using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Text;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Hook_StringBuilder", menuName = "Scriptables/Hooks/Composites/String Builder", order = 0)]
public class Hook_StringBuilder : StringHook
{
    [SerializeField] HookStringBuilderElement[] elements;
    public override string value
    {
        get
        {
            StringBuilder sb = new();
            foreach (var element in elements)
            {
                switch (element.type)
                {
                    case HookStringBuilderElementType.Text:
                        sb.Append(element.text);
                        break;
                    case HookStringBuilderElementType.IntegerHook:
                        sb.Append((element.integerHook.value * element.integerMultiplier).ToString());
                        break;
                    case HookStringBuilderElementType.FloatHook:
                        sb.Append((Mathf.Round(element.floatHook.value * element.floatMultiplier * Mathf.Pow(10, element.floatRounding)) / Mathf.Pow(10, element.floatRounding)).ToString());
                        break;
                    case HookStringBuilderElementType.StringHook:
                        sb.Append(element.stringHook.value);
                        break;
                }
            }
            return sb.ToString();
        }
    }

    public override void AssignCallback(Action callback)
    {
        foreach(var i in elements)
        {
            if (i.type == HookStringBuilderElementType.IntegerHook)
            {
                i.integerHook.AssignCallback(callback);
            }
            else if (i.type == HookStringBuilderElementType.FloatHook)
            {
                i.floatHook.AssignCallback(callback);
            }
        }
    }

    public override void RemoveCallback(Action callback)
    {
        foreach(var i in elements)
        {
            if (i.type == HookStringBuilderElementType.IntegerHook)
            {
                i.integerHook.RemoveCallback(callback);
            }
            else if (i.type == HookStringBuilderElementType.FloatHook)
            {
                i.floatHook.RemoveCallback(callback);
            }
        }
    }

    public override void SetValue(string value)
    {
        Debug.LogWarning("Unable to set value of string builder hook");
    }
}
[System.Serializable]
public struct HookStringBuilderElement
{
    public HookStringBuilderElementType type;

    //Text
    public string text;

    //IntegerHook
    public IntegerHook integerHook;
    public int integerMultiplier;

    //FloatHook
    public FloatHook floatHook;
    public float floatMultiplier;
    public int floatRounding;

    //StringHook
    public StringHook stringHook;
}
[System.Serializable]
public enum HookStringBuilderElementType
{
    Text = 0,
    IntegerHook = 1,
    FloatHook = 2,
    StringHook = 3
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(HookStringBuilderElement))]
public class HookStringBuilderElement_Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.height = 20.0f;

        EditorGUI.PropertyField(position, property.FindPropertyRelative("type"));
        position.y += position.height + 2.0f;

        if (property.FindPropertyRelative("type").enumValueIndex == 0)
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("text"));
            position.y += position.height + 2.0f;
        }
        else if (property.FindPropertyRelative("type").enumValueIndex == 1)
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("integerHook"));
            position.y += position.height + 2.0f;

            EditorGUI.PropertyField(position, property.FindPropertyRelative("integerMultiplier"));
            position.y += position.height + 2.0f;
        }
        else if (property.FindPropertyRelative("type").enumValueIndex == 2)
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("floatHook"));
            position.y += position.height + 2.0f;

            EditorGUI.PropertyField(position, property.FindPropertyRelative("floatMultiplier"));
            position.y += position.height + 2.0f;

            EditorGUI.PropertyField(position, property.FindPropertyRelative("floatRounding"));
            position.y += position.height + 2.0f;
        }
        else if(property.FindPropertyRelative("type").enumValueIndex == 3)
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("stringHook"));
            position.y += position.height + 2.0f;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 88.0f;
    }
}
#endif