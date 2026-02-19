using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class Recipe : MonoBehaviour
{
    public string Name { get; }
    private readonly List<RecipeStep> steps;

    public IReadOnlyList<RecipeStep> Steps => steps;

    public Recipe(string name, List<RecipeStep> steps)
    {
        Name = name;
        this.steps = steps;
    }

    public RecipeStep GetStep(int index)
    {
        if (index < 0 || index >= steps.Count) return null;


        return steps[index];
    }    



}
