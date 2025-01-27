using UnityEngine;

public class Debugger : MonoBehaviour
{
    [SerializeField] Language startLanguage = Language.English;
    [SerializeField] QuirkData addingQuirk;
    [SerializeField] ItemData givingItem;
    [SerializeField] int givingCount = 1;
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
    public void GiveItem()
    {
        if (!Application.isPlaying || givingItem == null || player == null) return;
        player.inventory.AddItem_DropRest(givingItem.Create(), givingCount);
    }
}
