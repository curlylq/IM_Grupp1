using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] private int score = 0;

    public int Score => score;

    public void AddPoints(int amount)
    {
        if (amount <= 0) return;
        score += amount;
    }

    public void AddRecipeBonus()
    {
        const int RECIPE_BONUS = 100;
        score += RECIPE_BONUS;
    }

    public void ResetScore()
    {
        score = 0;
    }


}
