using UnityEngine;
using static Enums;

[System.Serializable]
public class CaughtItem
{
    public IngredientType ingredientType;
    public float weight;
    public float friction;

    public CaughtItem(IngredientType type, float weight, float friction)
    {
        this.ingredientType = type;
        this.weight = weight;
        this.friction = friction;
    }
}
