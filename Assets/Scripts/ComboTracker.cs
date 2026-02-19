using UnityEngine;

public class ComboTracker : MonoBehaviour
{
    [SerializeField] private int comboCount = 0;

    [Header("Combo Settings")]
    [SerializeField] private float baseMultiplier = 1f;
    [SerializeField] private float multiplierStep = 0.25f;
    [SerializeField] private int maxCombo = 20;

    public int ComboCount => comboCount;


    public void OnCorrectCatch()
    {
        comboCount++;

        if (comboCount > maxCombo)
            comboCount = maxCombo;
    }

    public void OnMistake()
    {
        comboCount = 0;
    }

    public float GetMultiplier()
    {
        return baseMultiplier + (comboCount * multiplierStep);
    }


    public void ResetCombo()
    {
        comboCount = 0;
    }
}
