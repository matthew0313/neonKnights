using UnityEngine;

public interface ISavable
{
    public void Save(SaveData data);
    public void Load(SaveData data);
}