using static Enums;

/// <summary>
/// Representerar ett steg i ett recept.
/// Vanlig C#-klass (inte MonoBehaviour) så att konstruktor fungerar normalt.
/// </summary>
[System.Serializable]
public class RecipeStep
{
    public int order;
    public IngredientType ingredientType;

    public RecipeStep(int order, IngredientType ingredientType)
    {
        this.order = order;
        this.ingredientType = ingredientType;
    }
}