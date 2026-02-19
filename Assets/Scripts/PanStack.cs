using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

/// <summary>
/// Hanterar stapeln av ingredienser på pannan.
/// Ansvarar för att lägga till/ta bort items och beräkna stabiliteten
/// baserat på pannans lutning och itemsens samlade vikt och friktion.
/// </summary>
public class PanStack : MonoBehaviour
{
    private List<CaughtItem> items = new List<CaughtItem>();


    /// <summary>
    /// Lägg till en fångad ingrediens överst i stapeln.
    /// </summary>
    public void Add(CaughtItem item)
    {
        items.Add(item);
        Debug.Log($"Added {item.ingredientType} to stack. Stack size: {items.Count}");
    }

    /// <summary>
    /// Ta bort det översta itemet (t.ex. vid tappning eller game over).
    /// </summary>
    public void RemoveTop()
    {
        if (items.Count > 0)
        {
            items.RemoveAt(items.Count - 1);
            Debug.Log("Removed top item from stack.");
        }
    }

    // ── Stabilitet ──────────────────────────────────────────────────

    /// <summary>
    /// Beräknar om stapeln är stabil givet pannans lutningsvinkel.
    /// Tar hänsyn till varje items vikt och friktion.
    /// Returnerar true om stapeln håller, false om den riskerar att rasa.
    /// </summary>
    public float ComputeStability(float tiltAngle)
    {
        if (items.Count == 0) return 1f;

        float totalWeight = 0f;
        float totalFriction = 0f;

        foreach (var item in items)
        {
            totalWeight += item.weight;
            totalFriction += item.friction;
        }

        float avgFriction = totalFriction / items.Count;

        // Stabiliteten minskar med lutning och ökar med friktion
        // tiltAngle förväntas vara i grader (0 = plant, 90 = vertikalt)
        float stability = avgFriction - (Mathf.Abs(tiltAngle) / 90f) * totalWeight;

        return Mathf.Clamp01(stability);
    }

    /// <summary>
    /// Returnerar true om stapeln är instabil och items riskerar glida av.
    /// </summary>
    public bool IsUnstable(float tiltAngle)
    {
        return ComputeStability(tiltAngle) <= 0f;
    }

    // ── Hjälpmetoder ─────────────────────────────────────────────────

    public int Count => items.Count;

    public CaughtItem GetTop() => items.Count > 0 ? items[items.Count - 1] : null;

    public void Clear() => items.Clear();
}
