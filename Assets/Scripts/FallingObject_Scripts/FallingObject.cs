using System;
using UnityEngine;

/// <summary>
/// Abstrakt basklass för alla fallande objekt i Pan Panic.
/// Hanterar rörelse nedåt och grundläggande fångst-logik.
/// </summary>
public abstract class FallingObject : MonoBehaviour
{
    [SerializeField] protected float fallSpeed = 3f;

    /// <summary>
    /// Anropas varje frame. Flyttar objektet nedåt baserat på fallSpeed.
    /// </summary>
    public virtual void Update(float dt)
    {
        transform.position += Vector3.down * fallSpeed * dt;
    }

    /// <summary>
    /// Anropas när pannan fångar objektet.
    /// Varje subklass implementerar sin egen reaktion.
    /// </summary>
    public abstract void OnCaught(PanController pan);

    /// <summary>
    /// Förstör objektet om det faller utanför skärmen.
    /// </summary>
    protected virtual void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
