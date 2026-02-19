using UnityEngine;
using static Enums;

/// <summary>
/// Det gröna "håll pannan här"-området som spelaren måste hålla
/// pannan i för att starta spelet. Definieras av ett SpawnArea.
/// </summary>
public class StartZone : MonoBehaviour
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
        if (area == null) return false;

        float halfWidth = area.width / 2f;
        float halfHeight = area.height / 2f;

        bool withinX = pan.xPosition >= (area.centerX - halfWidth) &&
                       pan.xPosition <= (area.centerX + halfWidth);

        bool withinY = pan.transform.position.y >= (transform.position.y - halfHeight) &&
                       pan.transform.position.y <= (transform.position.y + halfHeight);

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
}