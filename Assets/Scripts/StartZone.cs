using UnityEngine;
using static Enums;

/// <summary>
/// Det gröna "håll pannan här"-området som spelaren måste hålla
/// pannan i för att starta spelet. Definieras av ett SpawnArea.
/// </summary>
/*public class StartZone : MonoBehaviour
{
    [SerializeField] private SpawnArea area;

    [Header("Starttimer")]
    [SerializeField] private float holdDuration = 2f; // Sekunder spelaren måste hålla

    private float holdTimer = 0f;
    private bool hasTriggeredStart = false;

    private void Update()
    {
        // StartZone är bara aktiv när spelet är i Idle-läge
        if (GameManager.Instance.State != GameState.Idle) return;
        if (hasTriggeredStart) return;

        PanController pan = FindObjectOfType<PanController>();
        if (pan == null) return;

        if (Contains(pan))
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= holdDuration)
            {
                hasTriggeredStart = true;
                GameManager.Instance.StartGame();
            }
        }
        else
        {
            // Återställ timern om spelaren drar bort pannan
            holdTimer = 0f;
        }
    }

    /// <summary>
    /// Returnerar true om pannans X-position är inom zonens område.
    /// </summary>
    public bool Contains(PanController pan)
    {
        if (area == null || pan == null) return false;

        // Convert pan world position into SpawnArea local space
        Vector3 local = area.transform.InverseTransformPoint(pan.transform.position);

        float halfWidth = area.width * 0.5f;
        float halfHeight = area.height * 0.5f;

        // Local X/Y within rectangle
        bool withinX = local.x >= -halfWidth && local.x <= halfWidth;
        bool withinY = local.y >= -halfHeight && local.y <= halfHeight;

        return withinX && withinY;
    }

    /// <summary>
    /// Återställer zonen, t.ex. vid omstart.
    /// </summary>
    public void Reset()
    {
        holdTimer = 0f;
        hasTriggeredStart = false;
    }
}*/