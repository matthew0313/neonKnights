using UnityEngine;

public class Debugger : MonoBehaviour
{
    [SerializeField] Language startLanguage = Language.English;
    [SerializeField] QuirkData addingQuirk;
    Player player;
    private void Awake()
    {
        LangText.language = startLanguage;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    public void AddQuirk()
    {
        if (!Application.isPlaying || addingQuirk == null || player == null) return;
        if(player.inventory.equipped is WeaponItem)
        {
            Quirk quirk = addingQuirk.Create();
            quirk.SetRandom();
            (player.inventory.equipped as WeaponItem).AddQuirk(quirk);
        }
    }
}
