using MEC;
using System;
using System.Text;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] TMP_Text statsText;
    [SerializeField] float interval = 0.05f, longInterval = 0.25f;

    [SerializeField] Button autoSaveLoadButton;
    CoroutineHandle typewriting;

    public static SaveData saveData;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void init()
    {
        saveData = null;
    }

    private void Start()
    {
        autoSaveLoadButton.gameObject.SetActive(GlobalManager.Instance.saves[0] != null);
        statsText.text = string.Empty;
        if (saveData != null)
        {
            typewriting = Timing.RunCoroutine(CoroutineLibrary.Typewrite(GetStatsString(saveData), interval, longInterval,
            (content) => statsText.text = content));
        }
    }
    string GetStatsString(SaveData data)
    {
        StringBuilder sb = new();
        sb.Append(new LangText()
        {
            en = $"Stage {data.stageIndex}\n",
            kr = $"스테이지 {data.stageIndex}\n"
        }.text);
        sb.Append(new LangText()
        {
            en = $"Time Played: {Mathf.Round(data.timesPlayed)}s\n",
            kr = $"플레이 시간: {Mathf.Round(data.timesPlayed)}s\n"
        }.text);
        if(data.playerData.strings.TryGetValue("inventory", out string val))
        {
            #region loadingInventory
            InventorySaveData inventoryData = JsonUtility.FromJson<InventorySaveData>(val);
            List<Item> items = new();
            foreach(var i in inventoryData.slotSaves)
            {
                if (i.item == null) continue;
                Item tmp = i.item.Create();
                tmp.Load(i.data);
                items.Add(tmp);
            }
            if(data.playerData.strings.TryGetValue("chestplateSlot", out string val2))
            {
                InventorySlotSaveData slotData = JsonUtility.FromJson<InventorySlotSaveData>(val2);
                if(slotData.item != null)
                {
                    Item tmp = slotData.item.Create();
                    tmp.Load(slotData.data);
                    items.Add(tmp);
                }
            }
            if (data.playerData.strings.TryGetValue("accessorySlot1", out string val3))
            {
                InventorySlotSaveData slotData = JsonUtility.FromJson<InventorySlotSaveData>(val3);
                if (slotData.item != null)
                {
                    Item tmp = slotData.item.Create();
                    tmp.Load(slotData.data);
                    items.Add(tmp);
                }
            }
            if (data.playerData.strings.TryGetValue("accessorySlot2", out string val4))
            {
                InventorySlotSaveData slotData = JsonUtility.FromJson<InventorySlotSaveData>(val4);
                if (slotData.item != null)
                {
                    Item tmp = slotData.item.Create();
                    tmp.Load(slotData.data);
                    items.Add(tmp);
                }
            }
            #endregion

            int weaponsCollected = 0, equipmentsCollected = 0, totalQuirks = 0;
            foreach(var i in items)
            {
                if (i is WeaponItem) weaponsCollected++;
                if (i is EquipmentItem) equipmentsCollected++;
                if (i is IQuirkable quirkable) totalQuirks += quirkable.quirks.Count;
            }
            sb.Append(new LangText()
            {
                en = $"Weapons Collected: {weaponsCollected}x\n" +
                $"Equipments Collected: {equipmentsCollected}x\n" +
                $"Total Quirks: {totalQuirks}x\n",
                kr = $"수집한 무기 개수: {weaponsCollected}x\n" +
                $"수집한 장비 개수: {equipmentsCollected}x\n" +
                $"총 쿼크 개수: {totalQuirks}x\n"

            }.text);
        }
        return sb.ToString();
    }
    private void OnDestroy()
    {
        Timing.KillCoroutines(typewriting);
    }
}
