using UnityEngine;

public class LifeSystem : MonoBehaviour
{
    [SerializeField] private int maxLives = 3;
    [SerializeField] private int lives;

    public int Lives => lives;
    public int MaxLives => maxLives;

    private void Awake()
    {
        ResetLives();
    }


    public void LoseLife()
    {
        if (lives <= 0)
            return;

        lives--;
    }

    public bool IsDead()
    {
        return lives <= 0;
    }

    public void ResetLives()
    {
        lives = maxLives;
    }

}
