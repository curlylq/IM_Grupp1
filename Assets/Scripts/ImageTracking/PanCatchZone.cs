using System.Collections.Generic;
using UnityEngine;

public class PanCatchZone : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    public float maxStableSpeed = 0.25f;
    public float maxStableAngularSpeed = 2.0f;

    private readonly Dictionary<Ingredient, float> stableTimers = new();

    private void OnTriggerEnter(Collider other)
    {
        var ing = other.GetComponentInParent<Ingredient>();
        if (ing == null || ing.isCounted) return;

        if (!stableTimers.ContainsKey(ing))
            stableTimers.Add(ing, 0f);
    }

    private void OnTriggerExit(Collider other)
    {
        var ing = other.GetComponentInParent<Ingredient>();
        if (ing == null) return;
        stableTimers.Remove(ing);
    }

    private void FixedUpdate()
    {
        var keys = new List<Ingredient>(stableTimers.Keys);

        foreach (var ing in keys)
        {
            if (ing == null) { stableTimers.Remove(ing); continue; }

            var rb = ing.GetComponent<Rigidbody>();
            bool slowEnough =
                rb.linearVelocity.magnitude <= maxStableSpeed &&
                rb.angularVelocity.magnitude <= maxStableAngularSpeed;

            if (slowEnough)
                stableTimers[ing] += Time.fixedDeltaTime;
            else
                stableTimers[ing] = 0f;

            if (stableTimers[ing] >= ing.stableTimeRequired)
            {
                ing.isCounted = true;
                stableTimers.Remove(ing);
                gameManager.OnIngredientCaught(ing);
            }
        }
    }
}
