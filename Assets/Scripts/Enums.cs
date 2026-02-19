using UnityEngine;

public class Enums : MonoBehaviour
{
    public enum GameState
    {
        WaitingToStart,
        Playing,
        GameOver
    }

    public enum IngredientType
    {
        BunBottom,
        Lettuce,
        Meat,
        BunTop,
        Trash
    }

    public enum SpecialEffectType
    {
        SlowFall,
        ExtraInstability
    }

    public enum CatchResult
    {
        Correct,
        Wrong,
        NoActiveRecipe,
        RecipeComplete
    }

}
