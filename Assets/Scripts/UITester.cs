using UnityEngine;

public class UITester : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            UIManager.Instance?.ShowStartScreen();

        if (Input.GetKeyDown(KeyCode.Alpha2))
            UIManager.Instance?.ShowGameUI();

        if (Input.GetKeyDown(KeyCode.Alpha3))
            UIManager.Instance?.ShowGameOver();

        if (Input.GetKeyDown(KeyCode.Alpha4))
            UIManager.Instance?.UpdateScore(999);

        if (Input.GetKeyDown(KeyCode.Alpha5))
            UIManager.Instance?.UpdateTimerUI(8f); // testar röd timer

        if (Input.GetKeyDown(KeyCode.Alpha6))
            UIManager.Instance?.UpdateRecipe("Köttbullar");

        if (Input.GetKeyDown(KeyCode.Alpha7))
            UIManager.Instance?.ShowTemporaryMessage("FEL! -1 liv", 2f, "Sallad");
    }
}