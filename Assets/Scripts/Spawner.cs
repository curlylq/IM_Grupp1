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
        running = true;
        timer = 0f;
        safeZone?.Activate();
    }

    public void StopSpawning()
    {
        running = false;
    }

    private void Update()
    {
        Tick(Time.deltaTime);
    }

    public void Tick(float dt)
    {
        if (!running) return;

        safeZone?.Tick(dt);

        float spawnsPerSecond = difficultyManager != null ? difficultyManager.GetSpawnRate() : 1f;
        float interval = Mathf.Max(0.05f, 1f / Mathf.Max(0.01f, spawnsPerSecond));

        timer += dt;
        while (timer >= interval)
        {
            timer -= interval;
            SpawnOne();
        }
    }

    private void SpawnOne()
    {
        if (spawnArea == null) return;

        Vector3 pos = spawnArea.GetRandomPoint();
        FallingObject prefab = ChoosePrefab();
        if (prefab == null) return;

        FallingObject obj = Instantiate(prefab, pos, Quaternion.identity);

        // Om dina FallingObject behöver init (t.ex. fallSpeed eller gm-referens)
        // obj.Initialize(gameManager, difficultyManager.GetFallSpeed());
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
