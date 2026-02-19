using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    [SerializeField] private Vector2 size = new Vector2(2f, 1f);
    [SerializeField] private float z = 0f;

    // Publika properties så StartZone (och andra) kan läsa dimensioner
    public float width => size.x;
    public float height => size.y;
    public float centerX => transform.position.x;
    public float centerY => transform.position.y;

    public Vector3 GetRandomPoint()
    {
        Vector3 center = transform.position;
        float x = Random.Range(center.x - size.x * 0.5f, center.x + size.x * 0.5f);
        float y = Random.Range(center.y - size.y * 0.5f, center.y + size.y * 0.5f);
        return new Vector3(x, y, z);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(size.x, size.y, 0.01f));
    }
#endif
}