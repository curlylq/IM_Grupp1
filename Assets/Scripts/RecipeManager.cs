using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using static Enums;

/*public class RecipeManager : MonoBehaviour
{
    private Recipe activeRecipe;
    private int stepIndex;

    public void NewRecipe(Recipe recipe)
    {
        activeRecipe = recipe;
        stepIndex = 0;
    }

    public IngredientType? GetExpectedIngredient()
    {
        if (activeRecipe == null) return null;
        var step = activeRecipe.GetStep(stepIndex);
        return step?.ingredientType; // Fix 1: liten bokstav, matchar RecipeStep
    }

    public CatchResult ValidateCatch(Ingredient ingredient)
    {
        if (activeRecipe == null) return CatchResult.NoActiveRecipe;

        var expected = GetExpectedIngredient();
        if (expected == null) return CatchResult.RecipeComplete;

        if (ingredient.type == expected) // Fix 2: liten bokstav, matchar Ingredient.type
        {
            stepIndex++;
            return CatchResult.Correct;
        }

        return CatchResult.WrongOrder; // Fix 3: rätt enum-värde
    }

    public bool IsRecipeComplete()
    {
        if (activeRecipe == null) return false;
        return stepIndex >= activeRecipe.Steps.Count;
    }
}*/
