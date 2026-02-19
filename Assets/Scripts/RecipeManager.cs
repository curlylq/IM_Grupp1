using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using static Enums;

public class RecipeManager : MonoBehaviour
{

    private Recipe activeRecipe;
    private int stepIndex;
    
    

    public void NewRecipe( Recipe recipe) //Laddar ett nytt recept
    { 
        activeRecipe = recipe;
        stepIndex = 0;
    }

    public IngredientType? GetExpectedIngredient() //Visar vad som förväntas härnäst
    {
        if (activeRecipe == null) return null;

        var step = activeRecipe.GetStep(stepIndex);
        return step?.IngredientType;
    }

    public CatchResult ValidateCatch (Ingredient ingredient) //Kontrollerar om ingrediens är rätt
    { 
        if (activeRecipe == null) return CatchResult.NoActiveRecipe;

        var expected = GetExpectedIngredient();

        if (expected == null) return CatchResult.RecipeComplete;

        if (ingredient.Type == expected) 
        {
            stepIndex++;
            return CatchResult.Correct;
        }
        return CatchResult.Wrong;

    }

 

    public bool IsRecipeComplete() //Returnerar true när receptet är klart
    { 
        if (activeRecipe == null) return false;

        return stepIndex >= activeRecipe.Steps.Count;
    }

 

}
