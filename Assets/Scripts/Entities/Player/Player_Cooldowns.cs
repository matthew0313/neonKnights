using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player_Cooldowns : MonoBehaviour, ISavable
{
    List<CooldownSource> cooldowns = new(), removeQueue = new(), addQueue = new();
    public void OnAwake()
    {
#if UNITY_EDITOR
        foreach(var i in FindObjectsByType<CooldownSource>(FindObjectsSortMode.None)) i.cooldownLeft = 0;
#endif
    }
    public void OnUpdate()
    {
        foreach(var i in cooldowns)
        {
            i.cooldownLeft = Mathf.Max(0.0f, i.cooldownLeft -= Time.deltaTime);
            if (!i.isOnCooldown)
            {
                removeQueue.Add(i);
            }
        }
        if(addQueue.Count > 0)
        {
            foreach (var i in addQueue) cooldowns.Add(i);
            addQueue.Clear();
        }
        if(removeQueue.Count > 0)
        {
            foreach (var i in removeQueue) cooldowns.Remove(i);
            removeQueue.Clear();
        }
    }
    public void AddCooldown(CooldownSource cooldown)
    {
        addQueue.Add(cooldown);
    }
    public bool ContainsCooldown(CooldownSource cooldown) => cooldowns.Contains(cooldown);

    public void Save(SaveData data)
    {
        CooldownSave tmp = new();
        foreach (var i in cooldowns)
        {
            tmp.cooldowns.Add(new CooldownSaveElement()
            {
                source = i,
                cooldownLeft = i.cooldownLeft,
                startCooldown = i.startCooldown
            });
        }
        data.playerData.strings["cooldowns"] = JsonUtility.ToJson(tmp);
    }

    public void Load(SaveData data)
    {
        if(data.playerData.strings.TryGetValue("cooldowns", out string tmp))
        {
            CooldownSave save = JsonUtility.FromJson<CooldownSave>(tmp);
            foreach (var i in save.cooldowns)
            {
                i.source.SetCooldown(i.startCooldown);
                i.source.cooldownLeft = i.cooldownLeft;
                addQueue.Add(i.source);
            }
        }
    }
}
[System.Serializable]
public class CooldownSave
{
    public List<CooldownSaveElement> cooldowns = new();
}
[System.Serializable]
public class CooldownSaveElement
{
    public CooldownSource source;
    public float cooldownLeft, startCooldown;
}