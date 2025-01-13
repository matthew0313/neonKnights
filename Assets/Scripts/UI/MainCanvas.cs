using UnityEngine;
using TMPro;

[RequireComponent(typeof(Canvas))]
public class MainCanvas : MonoBehaviour
{
    public static MainCanvas Instance { get; private set; }
    public MainCanvas() => Instance = this;
}