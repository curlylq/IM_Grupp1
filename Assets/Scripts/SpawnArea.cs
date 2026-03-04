using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    [SerializeField] private Vector2 size = new Vector2(2f, 1f);

    // How far in front of this transform to spawn (meters).
    // If SpawnArea is on/near the AR Camera, this should be > near clip (0.1).
    [SerializeField] private float forwardDistance = 1.5f;

    public float width => size.x;
    public float height => size.y;

    public Vector3 GetRandomPoint()
    {
        float localX = Random.Range(-size.x * 0.5f, size.x * 0.5f);
        float localY = Random.Range(-size.y * 0.5f, size.y * 0.5f);

        // Spawn in a rectangle in front of this transform.
        Vector3 localPoint = new Vector3(localX, localY, forwardDistance);
        return transform.TransformPoint(localPoint);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

        // Draw the spawn plane at forwardDistance
        Vector3 center = new Vector3(0f, 0f, forwardDistance);
        Gizmos.DrawWireCube(center, new Vector3(size.x, size.y, 0.01f));
    }
#endif
}