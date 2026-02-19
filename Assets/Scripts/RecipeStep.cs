using UnityEngine;

public class RecipeStep : MonoBehaviour
{

    private int order;
    private Enums.IngredientType IngredientType { get; }


  public RecipeStep(int order, Enums.IngredientType ingredientType)
    {
        this.order = order;
        this.IngredientType = ingredientType;
    }
}
