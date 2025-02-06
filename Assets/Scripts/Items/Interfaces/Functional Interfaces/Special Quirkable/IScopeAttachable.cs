using UnityEngine;

public interface IScopeAttachable
{
    public bool scopeAttached { get; set; }
    public float scopeSpreadReduction { get; }
    public int scopeCost { get; }
}