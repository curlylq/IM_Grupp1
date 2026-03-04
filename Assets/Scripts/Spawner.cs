using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private DifficultyManager difficultyManager;
    [SerializeField] private SpawnArea spawnArea;
    [SerializeField] private SafeZone safeZone;
    [SerializeField] private GameManager gameManager; // om FallingObject behöver veta gm

    [Header("Prefabs")]
    [SerializeField] private FallingObject[] ingredientPrefabs;
    [SerializeField] private FallingObject[] wrongObjectPrefabs;
    [SerializeField] private FallingObject[] specialObjectPrefabs;

    [Header("Chances (0..1)")]
    [SerializeField] private float wrongChance = 0.25f;
    [SerializeField] private float specialChance = 0.05f;

    private float timer;
    private bool running;

    public void StartSpawning()
    {
        Debug.Log("StartSpawning called");
        running = true;
        timer = 0f;
        safeZone?.Activate();
    }

    public void StopSpawning()
    {
        Debug.Log("StopSpawning called");
        running = false;
    }

    private void Update()
    {
        Tick(Time.deltaTime);
    }

    public void Tick(float dt)
    {
        if (!running) return;

        float spawnsPerSecond = difficultyManager != null ? difficultyManager.GetSpawnRate() : 1f;

        if (float.IsNaN(spawnsPerSecond) || float.IsInfinity(spawnsPerSecond))
            Debug.LogError($"Spawn rate is invalid: {spawnsPerSecond}");

        float interval = Mathf.Max(0.05f, 1f / Mathf.Max(0.01f, spawnsPerSecond));

        timer += dt;

        int spawnedThisFrame = 0;
        while (timer >= interval)
        {
            timer -= interval;
            SpawnOne();
            spawnedThisFrame++;
            if (spawnedThisFrame > 50) break; // prevent runaway spam
        }

        if (spawnedThisFrame > 0)
            Debug.Log($"Spawner running. Spawned {spawnedThisFrame}. Rate={spawnsPerSecond}, interval={interval}");
    }

    private void SpawnOne()
    {
        if (spawnArea == null) { Debug.LogError("No spawnArea"); return; }

        Vector3 pos = spawnArea.GetRandomPoint();
        FallingObject prefab = ChoosePrefab();
        if (prefab == null) { Debug.LogError("No prefab chosen"); return; }

        var obj = Instantiate(prefab, pos, Quaternion.identity);
        Debug.Log($"Spawned {prefab.name} at {pos}");
    }

    private FallingObject ChoosePrefab()
    {
        // SafeZone aktiv => bara ingredienser (enkel tolkning)
        if (safeZone != null && safeZone.IsActive)
            return PickRandom(ingredientPrefabs);

        // annars välj enligt sannolikheter
        float r = Random.value;

        if (specialObjectPrefabs != null && specialObjectPrefabs.Length > 0 && r < specialChance)
            return PickRandom(specialObjectPrefabs);

        if (wrongObjectPrefabs != null && wrongObjectPrefabs.Length > 0 && r < specialChance + wrongChance)
            return PickRandom(wrongObjectPrefabs);

        return PickRandom(ingredientPrefabs);
    }

    private FallingObject PickRandom(FallingObject[] list)
    {
        if (list == null || list.Length == 0) return null;
        return list[Random.Range(0, list.Length)];
    }
}
