using UnityEngine;

public abstract class SpecialObject : FallingObject
{
    [SerializeField] protected float duration = 5f;

    public override void OnCaught(PanController pan)
    {
        ApplyEffect(GameManager.Instance);
        Destroy(gameObject);

        // Starta en coroutine i GameManager för att ta bort effekten
        GameManager.Instance.StartCoroutine(
            RemoveEffectAfterDelay(GameManager.Instance, duration)
        );
    }

    public abstract void ApplyEffect(GameManager gm);
    public abstract void RemoveEffect(GameManager gm);

    private System.Collections.IEnumerator RemoveEffectAfterDelay(GameManager gm, float delay)
    {
        yield return new WaitForSeconds(delay);
        RemoveEffect(gm);
    }
}
