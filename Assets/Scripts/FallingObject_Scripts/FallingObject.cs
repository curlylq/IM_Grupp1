using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingObject : MonoBehaviour
{
    [Header("Gameplay")]
    public bool isCorrect = true;

    [Header("Stacking (fallback)")]
    public float stackHeight = 0.03f;

    [Header("Fall Speed Control")]
    [Tooltip("Max downward speed in meters/second while falling.")]
    [Min(0f)]
    [SerializeField] private float maxFallSpeed = 1.5f;

    [Tooltip("How quickly velocity approaches maxFallSpeed. Higher = snappier, lower = floatier.")]
    [Min(0f)]
    [SerializeField] private float fallAcceleration = 20f;

    private Rigidbody rb;
    private bool caught;

    public bool IsCaught { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;   // still uses gravity for 'feel' and interactions
        rb.isKinematic = false;
    }

    private void FixedUpdate()
    {
        if (caught) return;

        // Enforce a controlled downward speed (NOT floaty gravity ramp)
        float targetVy = -maxFallSpeed;

        // Smoothly move current vy toward targetVy
        float vy = rb.linearVelocity.y;
        float newVy = Mathf.MoveTowards(vy, targetVy, fallAcceleration * Time.fixedDeltaTime);

        // Only override if we're falling / should be falling
        // If you want it to ALWAYS force downward motion, remove the condition.
        if (vy > targetVy)
        {
            Vector3 v = rb.linearVelocity;
            v.y = newVy;
            rb.linearVelocity = v;
        }
    }

    public void Catch(Transform snapParent, Vector3 localSnapPos)
    {
        if (IsCaught) return;

        caught = true;
        IsCaught = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.useGravity = false;

        transform.SetParent(snapParent, false);
        transform.localPosition = localSnapPos;
        transform.localRotation = Quaternion.identity;
    }
}
