using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.XR;
using static Enums;

public class GameManager : MonoBehaviour
{

    [Header("State")]
    [SerializeField] private GameState state = GameState.Idle;
    [SerializeField] private float currentSpeedMultiplier = 1f;

    [Header("System (wire in Inspector")]
    [SerializeField] private RecipeManager recipeManager;
    [SerializeField] private ScoreSystem scoreSystem;
    [SerializeField] private ComboTracker comboTracker;
    [SerializeField] private LifeSystem lifeSystem;
    [SerializeField] private DifficultyManager difficultyManager;
    [SerializeField] private Spawner spawner;
    [SerializeField] private PanController panController;
    [SerializeField] private FeedbackManager feedbackManager;
    
    public GameState State => state;
    public float CurrentSpeedMultiplier => currentSpeedMultiplier;


    private void Awake()
    {

        //Rimlig deafault
        state = GameState.Idle;
        currentSpeedMultiplier = 1f;
    }

    private void Update()
    {
        Update(Time.deltaTime);
    }

    // UML: +Update(dt: float)
    public void Update(float dt)
    {
        switch (state)
        {
            case GameState.Idle:
                // Väntar på att spelaren ska starta spelet
                break;

            case GameState.Starting:
                // Enkel "instant start" – kan bytas mot countdown/intro
                state = GameState.Playing;
                StartGame();
                break;

            case GameState.Playing:
                TickPlaying(dt);
                break;

            case GameState.GameOver:
                // Hanterar game over-logiken
                break;

        }
    }

    public void StartGame() // Initierar spelet, visar startområdet
    {
        // Reset state
        comboTracker?.OnCorrectCatch(); // om du vill nolla combo separat, gör en Reset()-metod istället
        currentSpeedMultiplier = 1f;
        state = GameState.Starting;

        // Reset recept / liv / score
        // (Exakt reset beror på hur dina klasser ser ut, men idén är här)
        // lifeSystem.ResetLives();
        // scoreSystem.Reset();
        // recipeManager.NewRecipe();

        // Spawner: säker zon aktiv i början (om du har sådan logik)
        // spawner.ActivateSafeZone();

        feedbackManager?.PlayNewRecipe();
    }

    public void EndGame() // Hanterar spelets
    {
        state = GameState.GameOver;

        // Stoppa spawn / effekter
        // spawner.Stop();

        // Feedback
        feedbackManager?.PlayWrong(); // eller en egen PlayGameOver()
    }

    private void TickPlaying(float dt)
    {
        // 1) Difficulty -> speed multiplier
        if (difficultyManager != null)
            currentSpeedMultiplier = difficultyManager.GetSpawnRate();
        // Obs: i diagrammet finns GetSpawnRate(): float och GetFallSpeed(): float.
        // Om du vill att speed multiplier ska påverka fall, kan du istället använda GetFallSpeed().

        // 2) Spawner uppdateras normalt av sig själv, men om du vill styra här:
        // spawner.SetSpawnRate(difficultyManager.GetSpawnRate());

        // 3) Game over check
        if (lifeSystem != null && lifeSystem.IsDead())
            EndGame();
    }

    public FallingObject OnObjectCought (FallingObject obj)
    {
        if (state != GameState.Playing || obj == null)
            return;

        // Avgör vad som fångats (Ingredient / WrongObject / SpecialObject)
        // UML visar att Ingredient och WrongObject är FallingObject-subklasser.
        if (obj is Ingredient ingredient)
        {
            HandleIngredientCaught(ingredient);
            return;
        }

        if (obj is WrongObject wrong)
        {
            HandleWrongObjectCaught(wrong);
            return;
        }

        if (obj is SpecialObject special)
        {
            // SpecialObject: +ApplyEffect(gm: GameManager) i UML
            special.ApplyEffect(this);
            // samt +RemoveEffect(gm: GameManager) när duration går ut (sköts i SpecialObject)
            return;
        }

        // Okänd typ -> behandla som fel
        HandleGenericWrong();
    }

    private void HandleIngredientCaught(Ingredient ingredient)
    {
        // Receptvalidering
        var result = recipeManager.ValidateCatch(ingredient);

        switch (result)
        {
            case CatchResult.Correct:
                comboTracker?.OnCorrectCatch();
                scoreSystem?.AddPoints(comboTracker != null ? comboTracker.GetMultiplier() : 1f);
                feedbackManager?.PlayWrong(); // byt till "correct"-ljud om du har det
                feedbackManager?.UpdatePanGlow(0f, 1f); // ex: grönt glow (beroende på din impl)
                break;

            case CatchResult.WrongOrder:
                comboTracker?.OnMistake();
                lifeSystem?.LoseLife();
                feedbackManager?.PlayWrong();
                feedbackManager?.UpdatePanGlow(0f, 0f);
                break;

            case CatchResult.NotInRecipe:
                comboTracker?.OnMistake();
                lifeSystem?.LoseLife();
                feedbackManager?.PlayWrong();
                feedbackManager?.UpdatePanGlow(0f, 0f);
                break;
        }

        if (recipeManager.IsRecipeComplete())
        {
            scoreSystem?.AddRecipeBonus();
            difficultyManager?.IncreaseDifficulty();
            feedbackManager?.PlayRecipeComplete();
            feedbackManager?.PlayNewRecipe();

            // Ladda nästa recept (beror på hur du väljer recept)
            // recipeManager.NewRecipe();
        }

        if (lifeSystem != null && lifeSystem.IsDead())
            EndGame();
    }

    private void HandleWrongObjectCaught(WrongObject wrong)
    {
        comboTracker?.OnMistake();
        lifeSystem?.LoseLife();
        feedbackManager?.PlayWrong();
        feedbackManager?.UpdatePanGlow(0f, 0f);

        if (lifeSystem != null && lifeSystem.IsDead())
            EndGame();
    }

    private void HandleGenericWrong()
    {
        comboTracker?.OnMistake();
        lifeSystem?.LoseLife();
        feedbackManager?.PlayWrong();

        if (lifeSystem != null && lifeSystem.IsDead())
            EndGame();
    }




}
