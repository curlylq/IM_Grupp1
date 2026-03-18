using UnityEngine;

public class SimpleSpawner : MonoBehaviour
{
    [SerializeField] private SpawnArea spawnArea;
    [SerializeField] private FallingObject[] ingredientPrefabs;

    [Header("Timing")]
    [SerializeField] private float spawnsPerSecond = 1.5f;
    [SerializeField] private bool autoStart = false;

    private float timer;
    private bool running;

    public UIManager UIManager;

    private void Start()
    {
        if (autoStart) StartSpawning();
    }

    public void StartSpawning()
    {
        running = true;
        timer = 0f;
        //UIManager.ShowStartScreen();
    }

    public void StopSpawning()
    {
        running = false;
    }

    private void Update()
    {
        if (!running || spawnArea == null || ingredientPrefabs == null || ingredientPrefabs.Length == 0) return;

        float interval = 1f / Mathf.Max(0.01f, spawnsPerSecond);
        timer += Time.deltaTime;

        while (timer >= interval)
        {
            timer -= interval;
            SpawnOne();
        }
    }

    private void SpawnOne()
    {
        var prefab = ingredientPrefabs[Random.Range(0, ingredientPrefabs.Length)];
        Vector3 pos = spawnArea.GetRandomPoint();
        Instantiate(prefab, pos, Quaternion.identity);
    }
}
