using UnityEngine;

public class SaveData
{
    public SerializableDictionary<string, DataUnit> mapElements = new();
    public SerializableDictionary<string, bool> cutsceneTriggers = new();
}
