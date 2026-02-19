using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private int level = 1;

    [Header("Base Values")]
    [SerializeField] private float baseFallSpeed = 1.0f;
    [SerializeField] private float baseSpawnRate = 1.0f; // tolkning: spawns per sekund

    [Header("Scaling Per Level")]
    [SerializeField] private float fallSpeedPerLevel = 0.15f;
    [SerializeField] private float spawnRatePerLevel = 0.10f;

    [Header("Caps")]
    [SerializeField] private float maxFallSpeed = 5.0f;
    [SerializeField] private float maxSpawnRate = 3.0f;

    public int Level => level;

    // UML-fältens "aktuella" värden (beräknas från level)
    public float GetFallSpeed()
    {
        float value = baseFallSpeed + (level - 1) * fallSpeedPerLevel;
        return Mathf.Min(value, maxFallSpeed);
    }

    public float GetSpawnRate()
    {
        float value = baseSpawnRate + (level - 1) * spawnRatePerLevel;
        return Mathf.Min(value, maxSpawnRate);
    }

    public void IncreaseDifficulty()
    {
        level++;
    }

    // Valfritt men praktiskt vid nytt spel
    public void ResetDifficulty()
    {
        level = 1;
    }
}
