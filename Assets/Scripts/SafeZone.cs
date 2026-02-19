using UnityEngine;

public class SafeZone : MonoBehaviour
{
    [SerializeField] private float durationSeconds = 3f;

    private float timeLeft;

    public bool IsActive => timeLeft > 0f;

    public void Activate()
    {
        timeLeft = durationSeconds;
    }

    public void Deactivate()
    {
        timeLeft = 0f;
    }

    public void Tick(float dt)
    {
        if (timeLeft <= 0f) return;
        timeLeft -= dt;
        if (timeLeft < 0f) timeLeft = 0f;
    }
}
