using UnityEngine;

public interface ISavable
{
    public int executionOrder => 0;
    public void Save(SaveData data);
    public void Load(SaveData data);
}