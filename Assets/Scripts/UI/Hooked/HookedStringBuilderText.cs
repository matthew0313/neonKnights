using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HookedStringBuilderText : MonoBehaviour
{
    [SerializeField] HookStringBuilderElement[] elements;

    [Header("Display")]
    [SerializeField] TMP_Text text;
    private void OnEnable()
    {
        foreach(var i in elements)
        {
            if (i.type == HookStringBuilderElementType.IntegerHook)
            {
                i.integerHook.AssignCallback(OnValueChange);
            }
            else if (i.type == HookStringBuilderElementType.FloatHook)
            {
                i.floatHook.AssignCallback(OnValueChange);
            }
        }
        OnValueChange();
    }
    private void OnDisable()
    {
        foreach (var i in elements)
        {
            if (i.type == HookStringBuilderElementType.IntegerHook)
            {
                i.integerHook.RemoveCallback(OnValueChange);
            }
            else if (i.type == HookStringBuilderElementType.FloatHook)
            {
                i.floatHook.RemoveCallback(OnValueChange);
            }
        }
    }
    void OnValueChange()
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
            }
        }
        text.text = sb.ToString();
    }
}