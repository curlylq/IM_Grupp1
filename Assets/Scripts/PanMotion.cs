using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PanMotion : MonoBehaviour
{
    public Vector3 DeltaPosition { get; private set; }
    public Quaternion DeltaRotation { get; private set; }

    private Vector3 lastPos;
    private Quaternion lastRot;

    private void Awake()
    {
        lastPos = transform.position;
        lastRot = transform.rotation;
    }

    private void FixedUpdate()
    {
        DeltaPosition = transform.position - lastPos;
        DeltaRotation = transform.rotation * Quaternion.Inverse(lastRot);

        lastPos = transform.position;
        lastRot = transform.rotation;
    }
}
