using UnityEngine;

public class Enums : MonoBehaviour
{
    public enum GameState
    {
        Idle,
        Starting,
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
        WrongOrder,
        NotInRecipe,
        NoActiveRecipe,
        RecipeComplete
    }

}
