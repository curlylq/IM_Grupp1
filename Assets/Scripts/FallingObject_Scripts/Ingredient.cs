using UnityEngine;
using static Enums;

public class Ingredient : FallingObject
{
    public IngredientType type;
    public float weight = 1f;

    public override void OnCaught(PanController pan)
    {
        GameManager.Instance.TryAddIngredient(this);
        Destroy(gameObject);
    }
}
