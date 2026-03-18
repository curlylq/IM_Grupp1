using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Image Heart_1;
    public Image Heart_2;
    public Image Heart_3;

    private int currentHearts;
    public UIManager UIManager;

    void Start()
    {
        currentHearts = 3; // Starta med fullt liv
        UpdateHeartsUI();
    }

    // Metod som kan kallas från andra klasser
    public void TakeDamage(int amount = 1)
    {
        currentHearts -= amount;
        if (currentHearts < 0) currentHearts = 0;
        UpdateHeartsUI();

        if(currentHearts == 0)
        {
            UIManager.ShowGameOver();
            currentHearts = 3;
            Invoke("UpdateHeartsUI", 2);
        }
    }

    void UpdateHeartsUI()
    {
        Heart_1.enabled = currentHearts >= 1;
        Heart_2.enabled = currentHearts >= 2;
        Heart_3.enabled = currentHearts >= 3;
    }
}
