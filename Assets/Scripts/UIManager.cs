using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static System.Net.Mime.MediaTypeNames;

/// <summary>
/// UIManager hanterar all UI-logik i hamburgerspelet.
/// Placera detta script på ett tomt GameObject i scenen (t.ex. "UIManager").
/// Koppla ihop UI-elementen via Inspector.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Texter")]
    public TMP_Text scoreTxt;
    public TMP_Text recipeTxt;
    public TMP_Text timerTxt;

    [Header("Bild")]
    public UnityEngine.UI.Image recipeImg;

    [Header("Skärmar")]
    public GameObject startTxt;
    public GameObject gameOverTxt;

    // =========================================================
    // Unity-livscykel
    // =========================================================

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
       // ShowStartScreen();
    }

    // =========================================================
    // Skärmlägen
    // =========================================================

    /// <summary>Visar startskärmen innan spelet börjar.</summary>
    public void ShowStartScreen()
    {
        if (startTxt != null) startTxt.SetActive(true);
        if (gameOverTxt != null) gameOverTxt.SetActive(false);
        SetGameUIVisible(false);
    }

    /// <summary>Döljer startskärmen och visar spel-UI.</summary>
    public void ShowGameUI()
    {
        if (startTxt != null) startTxt.SetActive(false);
        if (gameOverTxt != null) gameOverTxt.SetActive(false);
        SetGameUIVisible(true);
    }

    /// <summary>Visar game over-skärmen.</summary>
    public void ShowGameOver()
    {
        if (gameOverTxt != null) gameOverTxt.SetActive(true);
        SetGameUIVisible(false);
    }

    // =========================================================
    // Uppdatera UI-element
    // =========================================================

    /// <summary>Uppdaterar poängtexten.</summary>
    public void UpdateScore(int score)
    {
        if (scoreTxt != null)
            scoreTxt.text = "Poäng: " + score;
    }

    /// <summary>Uppdaterar recepttexten och bilden med nästa ingrediens.</summary>
    public void UpdateRecipe(string ingredientName, Sprite ingredientSprite = null)
    {
        if (recipeTxt != null)
            recipeTxt.text = "Fånga: " + ingredientName;

        if (recipeImg != null)
        {
            if (ingredientSprite != null)
            {
                recipeImg.sprite = ingredientSprite;
                recipeImg.gameObject.SetActive(true);
            }
            else
            {
                recipeImg.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Uppdaterar timertexten. Blinkar rött under 10 sekunder.
    /// </summary>
    public void UpdateTimer(float secondsLeft)
    {
        if (timerTxt == null) return;

        int display = Mathf.CeilToInt(secondsLeft);
        timerTxt.text = "Tid: " + display + "s";
        timerTxt.color = secondsLeft <= 10f ? Color.red : Color.white;
    }

    /// <summary>
    /// Visar ett tillfälligt meddelande i recipeTxt, t.ex. "FEL! -5s".
    /// Återgår automatiskt till ingrediensnamnet efter delay sekunder.
    /// </summary>
    public void ShowTemporaryMessage(string message, float delay, string nextIngredientName)
    {
        if (recipeTxt != null)
            recipeTxt.text = message;

        CancelInvoke(nameof(ClearTemporaryMessage));
        _pendingIngredientName = nextIngredientName;
        Invoke(nameof(ClearTemporaryMessage), delay);
    }

    // =========================================================
    // Privata hjälpmetoder
    // =========================================================

    private string _pendingIngredientName = "";

    private void ClearTemporaryMessage()
    {
        if (recipeTxt != null)
            recipeTxt.text = "Fånga: " + _pendingIngredientName;
    }

    private void SetGameUIVisible(bool visible)
    {
        if (scoreTxt != null) scoreTxt.gameObject.SetActive(visible);
        if (recipeTxt != null) recipeTxt.gameObject.SetActive(visible);
        if (recipeImg != null) recipeImg.gameObject.SetActive(visible);
        if (timerTxt != null) timerTxt.gameObject.SetActive(visible);
    }
}