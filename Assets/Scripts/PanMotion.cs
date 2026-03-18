using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PanMotion : MonoBehaviour
{
    [SerializeField] private float smoothSpeed = 15f;

    public Vector3 DeltaPosition { get; private set; }
    public Quaternion DeltaRotation { get; private set; }

    private Vector3 lastPos;
    private Quaternion lastRot;

    private Vector3 smoothPos;
    private Quaternion smoothRot;

    private void Awake()
    {
        smoothPos = transform.position;
        smoothRot = transform.rotation;

        lastPos = transform.position;
        lastRot = transform.rotation;
    }

    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        smoothPos = Vector3.Lerp(smoothPos, transform.position, dt * smoothSpeed);
        smoothRot = Quaternion.Slerp(smoothRot, transform.rotation, dt * smoothSpeed);

        DeltaPosition = smoothPos - lastPos;
        DeltaRotation = smoothRot * Quaternion.Inverse(lastRot);

        lastPos = smoothPos;
        lastRot = smoothRot;
    }
}