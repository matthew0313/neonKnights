using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
public class PlayerHPText : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    Player target;
    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    private void OnEnable()
    {
        target.hpComp.onHpChange += OnHpChange;
        OnHpChange();
    }
    private void OnDisable()
    {
        target.hpComp.onHpChange -= OnHpChange;
    }
    void OnHpChange()
    {
        StringBuilder sb = new();
        sb.Append(target.statistics.playerName);
        sb.Append(" | ");
        sb.Append($"{Mathf.Ceil(target.hpComp.hp + target.hpComp.shield)}/{target.hpComp.maxHp + target.hpComp.maxShield}");
        text.text = sb.ToString();
    }
}