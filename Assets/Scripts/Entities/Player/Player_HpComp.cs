using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class Player_HpComp : HpComp, ICutsceneCallbackReceiver, ISavable
{
    [SerializeField] GameOverCutscene gameOverCutscene;
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
        DamageReceivedInfo info;
        if (undamagable) info = new() { receivedDamage = 0 };
        else info = base.GetDamage(damage);

        if(dead)
        {
            CutsceneManager.Instance.StartCutscene(gameOverCutscene);
        }

        return info;
    }

    public void Save(SaveData data)
    {
        data.playerData.strings["HpComp"] = JsonUtility.ToJson(Save());
    }

    public void Load(SaveData data)
    {
        if(data.playerData.strings.TryGetValue("HpComp", out string json))
        {
            Load(JsonUtility.FromJson<HpCompSaveData>(json));
        }
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