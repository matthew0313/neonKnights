using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIScanner : MonoBehaviour
{
    static List<RaycastResult> hits = new();
    public static List<RaycastResult> ScanUI(Vector2 screenPos)
    {
        EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = screenPos }, hits);
        return hits;
    }
}