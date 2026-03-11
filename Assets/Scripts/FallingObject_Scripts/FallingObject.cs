using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingObject : MonoBehaviour
{
    [Header("Gameplay")]
    public bool isCorrect = true;

    [Header("Fall Speed")]
    [Min(0f)] public float maxFallSpeed = 1.5f;
    [Min(0f)] public float fallAcceleration = 50f;

    [Header("Carry While Caught")]
    [Range(0f, 1f)]
    [Tooltip("0 = no carry, 1 = perfectly follows pan translation while in catch zone.")]
    [SerializeField] private float carryStrength = 1f;

    [Header("State (read-only)")]
    public bool InCatchZone { get; private set; }

    private Rigidbody rb;
    private PanMotion panMotion; // set when entering catch zone

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void FixedUpdate()
    {
        if (InCatchZone && panMotion != null && carryStrength > 0f)
        {
            // Move with the pan (translation only). Still allows falling off due to gravity/tilt.
            rb.MovePosition(rb.position + panMotion.DeltaPosition * carryStrength);

            // Optional: rotate with pan a bit (comment out if you hate it)
            // rb.MoveRotation(panMotion.DeltaRotation * rb.rotation);

            return; // don't force fall speed while being carried
        }

        // Normal falling
        var v = rb.linearVelocity;
        float targetVy = -maxFallSpeed;
        v.y = Mathf.MoveTowards(v.y, targetVy, fallAcceleration * Time.fixedDeltaTime);
        rb.linearVelocity = v;
    }

    public void SetInCatchZone(bool value, PanMotion motion = null)
    {
        InCatchZone = value;
        panMotion = value ? motion : null;
    }
}