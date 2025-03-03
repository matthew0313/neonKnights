using UnityEngine;

public class SaveData
{
    public int stageIndex = 1;
    public float timesPlayed = 0.0f;
    public DataUnit playerData = new();
    public SerializableDictionary<string, DataUnit> mapElements = new();
    public SerializableDictionary<string, DataUnit> cutsceneTriggers = new();
    public SerializableDictionary<string, DataUnit> enemies = new();
    public SerializableDictionary<int, DroppedItemManagerSave> droppedItems = new();
}
