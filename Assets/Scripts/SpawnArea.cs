using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    [SerializeField] private Vector2 size = new Vector2(0.5f, 0.5f); // width/depth in meters
    [SerializeField] private float height = 0.5f; // meters above the SpawnArea transform

    public Vector3 GetRandomPoint()
    {
        float localX = Random.Range(-size.x * 0.5f, size.x * 0.5f);
        float localZ = Random.Range(-size.y * 0.5f, size.y * 0.5f);

        // Spawn above this transform
        Vector3 localPoint = new Vector3(localX, height, localZ);
        return transform.TransformPoint(localPoint);
    }
}