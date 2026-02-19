using System.Collections.Generic;
using UnityEngine;

public class PanCatchZone : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    public float maxStableSpeed = 0.25f;
    public float maxStableAngularSpeed = 2.0f;

    private readonly Dictionary<FallingObject, float> stableTimers = new();

    private void Awake()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var obj = other.GetComponentInParent<FallingObject>();
        if (obj == null) return;

        // If your FallingObject has a "counted" flag, check it here.
        // Otherwise the dictionary prevents duplicates while inside.
        if (!stableTimers.ContainsKey(obj))
            stableTimers.Add(obj, 0f);
    }

    private void OnTriggerExit(Collider other)
    {
        var obj = other.GetComponentInParent<FallingObject>();
        if (obj == null) return;

        stableTimers.Remove(obj);
    }

    private void FixedUpdate()
    {
        var keys = new List<FallingObject>(stableTimers.Keys);

        foreach (var obj in keys)
        {
            if (obj == null) { stableTimers.Remove(obj); continue; }

            var rb = obj.GetComponent<Rigidbody>();
            if (rb == null) { stableTimers.Remove(obj); continue; }

            bool slowEnough =
                rb.velocity.magnitude <= maxStableSpeed &&
                rb.angularVelocity.magnitude <= maxStableAngularSpeed;

            if (slowEnough)
                stableTimers[obj] += Time.fixedDeltaTime;
            else
                stableTimers[obj] = 0f;

            // ✅ Important: stableTimeRequired currently lives on Ingredient in your code.
            // So we read it if it's an Ingredient, otherwise use a default.
            float required = 0.45f;
            if (obj is Ingredient ing)

            if (stableTimers[obj] >= required)
            {
                stableTimers.Remove(obj);

                // ✅ Fire your existing game logic
                gameManager.OnObjectCought(obj);

                // ✅ Let the object react (optional)
                // obj.OnCaught(panController); // if you want to call this too
            }
        }
    }
}
