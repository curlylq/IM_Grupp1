using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
   string recipeName = "Pancakes";
    int currentStep = 0;
    List<IngredientType> recipeSteps;

    void LoadRecipe() //Laddar ett nytt recept
    { 
    
    }

    bool ValidateIngredient (Ingredient ingredient) //Kontrollerar om ingrediens är rätt
    { 
        return ingredient.type == recipeSteps[currentStep];
    }

    void NextStep() //Går vidare till nästa ingrediens
    { 
        currentStep++;
    }

    bool IsRecipeComplete() //Returnerar true när receptet är klart
    { 
        return currentStep >= recipeSteps.Count;
    }

    IngredientType GetCurrentIngredient() //Visar vad som förväntas härnäst
    { 
        return recipeSteps[currentStep];
    }

}
