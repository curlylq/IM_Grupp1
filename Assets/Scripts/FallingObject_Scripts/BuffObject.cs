using UnityEngine;

public class BuffObject : SpecialObject
{
    [SerializeField] private float slowMultiplier = 0.5f;

    public override void ApplyEffect(GameManager gm)
    {
        gm.SetFallSpeedMultiplier(slowMultiplier);
        Debug.Log("Buff applied: Slow fall speed");
    }

    public override void RemoveEffect(GameManager gm)
    {
        gm.SetFallSpeedMultiplier(1f);
        Debug.Log("Buff removed: Normal fall speed restored");
    }
}
