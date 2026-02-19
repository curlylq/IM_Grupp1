using UnityEngine;

public class NerfObject : SpecialObject
{
    [SerializeField] private float speedMultiplier = 1.75f;

    public override void ApplyEffect(GameManager gm)
    {
        gm.SetFallSpeedMultiplier(speedMultiplier);
        Debug.Log("Nerf applied: Increased fall speed");
    }

    public override void RemoveEffect(GameManager gm)
    {
        gm.SetFallSpeedMultiplier(1f);
        Debug.Log("Nerf removed: Normal speed restored");
    }
}
