using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField] RectTransform left, right, up, down;
    [SerializeField] float defaultOffset = 50.0f;
    [SerializeField] float aimOffset = 25.0f;
    Player player;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    private void LateUpdate()
    {
        SetAim(player.camera.zoomAmount);
    }
    public void SetAim(float aimAmount)
    {
        float val = Mathf.Lerp(defaultOffset, aimOffset, aimAmount);
        left.anchoredPosition = new Vector2(-val, 0);
        right.anchoredPosition = new Vector2(val, 0);
        up.anchoredPosition = new Vector2(0, val);
        down.anchoredPosition = new Vector2(0, -val);
    }
}
