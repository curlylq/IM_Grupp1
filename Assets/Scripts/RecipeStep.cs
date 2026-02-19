using UnityEngine;

public class RecipeStep : MonoBehaviour
{

    private int order;
    private IngredientType IngredientType { get; }


  public RecipeStep(int order, IngredientType ingredientType)
    {
        this.order = order;
        this.IngredientType = ingredientType;
    }
}
