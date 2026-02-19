using UnityEngine;

public abstract class SpecialObject : FallingObject
{
    [SerializeField] protected float duration = 5f;

    // Publik property så GameManager kan läsa duration
    public float Duration => duration;

    public override void OnCaught(PanController pan)
    {
        // Skickar sig själv till GameManager.
        // GameManager hanterar ApplyEffect + coroutine för RemoveEffect.
        GameManager.Instance.OnObjectCought(this);
        Destroy(gameObject);
    }

    public abstract void ApplyEffect(GameManager gm);
    public abstract void RemoveEffect(GameManager gm);
}
