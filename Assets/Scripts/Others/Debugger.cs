using UnityEngine;

public class Debugger : MonoBehaviour
{
    [SerializeField] Language startLanguage = Language.English;
    [SerializeField] QuirkData addingQuirk;
    [SerializeField] QuirkShard_ItemData shardItemData;
    Player player;
    private void Awake()
    {
        Settings.language = startLanguage;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    public void AddQuirk()
    {
        if (!Application.isPlaying || addingQuirk == null || player == null) return;
        if(player.inventory.equipped is IQuirkable)
        {
            if (addingQuirk.IsAddable(player.inventory.equipped as IQuirkable) == false) return;
            Quirk quirk = addingQuirk.Create();
            quirk.SetRandom();
            (player.inventory.equipped as IQuirkable).AddQuirk(quirk);
        }
    }
    public void GiveQuirkShard()
    {
        if (!Application.isPlaying || addingQuirk == null || shardItemData == null || player == null || !addingQuirk.isShardable) return;
        QuirkShard_Item tmp = shardItemData.Create() as QuirkShard_Item;
        tmp.SetQuirk(addingQuirk);
        player.inventory.Insert(tmp, 1);
    }
}
