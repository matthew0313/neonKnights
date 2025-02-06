using UnityEngine;

public interface ICurseable
{
    public bool cursed { get; set; }
    public LangText curseEffectDesc { get; }
}