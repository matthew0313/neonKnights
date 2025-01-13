using UnityEngine;

public class Debugger : MonoBehaviour
{
    [SerializeField] Language startLanguage = Language.English;
    [SerializeField] QuirkData addingQuirk;
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
    public void ForceAddQuirk()
    {
        if (!Application.isPlaying || addingQuirk == null || player == null) return;
        if (player.inventory.equipped is IQuirkable)
        {
            Quirk quirk = addingQuirk.Create();
            quirk.SetRandom();
            (player.inventory.equipped as IQuirkable).AddQuirk(quirk);
        }
    }
}
