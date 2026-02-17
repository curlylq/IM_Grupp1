using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    string recipeName()
    {
        return "Pancakes";
    }

    int currentStep()
    {
        return 1;
    }
     string[] recipeSteps()
     {
         return new string[] {"Add flour", "Add eggs", "Add milk", "Mix ingredients", "Cook on pan"};
    }
}
