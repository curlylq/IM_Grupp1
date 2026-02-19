using System.Collections.Generic;

/// <summary>
/// Representerar ett recept med en ordnad lista av steg.
/// Vanlig C#-klass (inte MonoBehaviour) så att konstruktor fungerar normalt.
/// </summary>
[System.Serializable]
public class Recipe
{
    public string Name { get; }
    private readonly List<RecipeStep> steps;
    public IReadOnlyList<RecipeStep> Steps => steps;

    public Recipe(string name, List<RecipeStep> steps)
    {
        this.Name = name;
        this.steps = steps;
    }

    public RecipeStep GetStep(int index)
    {
        if (index < 0 || index >= steps.Count) return null;
        return steps[index];
    }
}