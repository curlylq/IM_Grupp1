using System.Collections;
using UnityEngine;
using static Enums;

/*public class GameManager : MonoBehaviour
{
    // ── Singleton ────────────────────────────────────────────────────
    public static GameManager Instance { get; private set; }

    // ── State ────────────────────────────────────────────────────────
    [Header("State")]
    [SerializeField] private GameState state = GameState.Idle;
    [SerializeField] private float currentSpeedMultiplier = 1f;

    // ── System-referenser (koppla i Inspector) ───────────────────────
    [Header("Systems (wire in Inspector)")]
    [SerializeField] private RecipeManager recipeManager;
    [SerializeField] private ScoreSystem scoreSystem;
    [SerializeField] private ComboTracker comboTracker;
    [SerializeField] private LifeSystem lifeSystem;
    [SerializeField] private DifficultyManager difficultyManager;
    [SerializeField] private Spawner spawner;
    [SerializeField] private PanController panController;
    // [SerializeField] private FeedbackManager feedbackManager;

    // ── Publika egenskaper ───────────────────────────────────────────
    public GameState State => state;
    public float CurrentSpeedMultiplier => currentSpeedMultiplier;

    // ────────────────────────────────────────────────────────────────
    // Unity-livscykel
    // ────────────────────────────────────────────────────────────────

    [SerializeField] private bool autoStartInEditor = true;

    private void Start()
    {
        UIManager.Instance?.ShowStartScreen(); // visa startskärm /kevin
#if UNITY_EDITOR
        if (autoStartInEditor)
            StartGame();
#endif
    }



    private void Awake()
    {
        // Singleton-setup: förstör duplicat om det finns
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        state = GameState.Idle;
        currentSpeedMultiplier = 1f;
    }

    private void Update()
    {
        Update(Time.deltaTime);
    }

    // ────────────────────────────────────────────────────────────────
    // Spelstyrning
    // ────────────────────────────────────────────────────────────────

    public void Update(float dt)
    {
        switch (state)
        {
            case GameState.Idle:
                // Väntar på att spelaren håller pannan i startzon
                break;

            case GameState.Starting:
                // Kan bytas mot countdown/intro-animation
                state = GameState.Playing;
                spawner?.StartSpawning();   
                break;

            case GameState.Playing:
                TickPlaying(dt);
                break;

            case GameState.GameOver:
                // Hanteras av EndGame()
                break;
        }
    }

    /// <summary>
    /// Anropas av StartZone när spelaren hållit pannan tillräckligt länge.
    /// </summary>
    public void StartGame()
    {
        currentSpeedMultiplier = 1f;
        state = GameState.Starting;

        // Återställ subsystem
        // lifeSystem?.ResetLives();
        // scoreSystem?.Reset();
        // recipeManager?.NewRecipe();
        // spawner?.ActivateSafeZone();
        // feedbackManager?.PlayNewRecipe();

        UIManager.Instance?.ShowGameUI(); // visa spel-UI /kevin
        UIManager.Instance?.UpdateScore(0); // poängdisplay /kevin
    }

    /// <summary>
    /// Avslutar spelet och sätter state till GameOver.
    /// </summary>
    public void EndGame()
    {
        state = GameState.GameOver;

        // spawner?.Stop();
        // feedbackManager?.PlayGameOver();

        UIManager.Instance?.ShowGameOverScreen(); // game over :( /kevin
    }

    private void TickPlaying(float dt)
    {
        // Uppdatera svårighetsgrad och hastighet
        if (difficultyManager != null)
            currentSpeedMultiplier = difficultyManager.GetSpawnRate();

        // Kolla om spelaren är död
        if (lifeSystem != null && lifeSystem.IsDead())
            EndGame();
    }

    // ────────────────────────────────────────────────────────────────
    // Fångst-hantering
    // ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Huvudingång för alla fallande objekt när de fångas av pannan.
    /// Sorterar objekttyp och delegerar till rätt handler.
    /// </summary>
    public void OnObjectCought(FallingObject obj)
    {
        if (state != GameState.Playing || obj == null)
            return;

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
            // Applicera effekten och schemalägg borttagning via GameManager
            // så att coroutinen inte avbryts när objektet förstörs
            special.ApplyEffect(this);
            StartCoroutine(RemoveSpecialEffectAfterDelay(special, special.Duration));
            return;
        }

        HandleGenericWrong();
    }

    private IEnumerator RemoveSpecialEffectAfterDelay(SpecialObject special, float delay)
    {
        yield return new WaitForSeconds(delay);
        special.RemoveEffect(this);
    }

    // ────────────────────────────────────────────────────────────────
    // Privata handlers
    // ────────────────────────────────────────────────────────────────

    private void HandleIngredientCaught(Ingredient ingredient)
    {
        var result = recipeManager.ValidateCatch(ingredient);

        switch (result)
        {
            case CatchResult.Correct:
                comboTracker?.OnCorrectCatch();
                int points = Mathf.RoundToInt(comboTracker != null ? comboTracker.GetMultiplier() : 1f);
                scoreSystem?.AddPoints(points);
                // feedbackManager?.PlayCorrect();
                UIManager.Instance?.UpdateScore(scoreSystem); // uppdatera poängdisplay /kevin
                break;

            case CatchResult.WrongOrder:
            case CatchResult.NotInRecipe:
                comboTracker?.OnMistake();
                lifeSystem?.LoseLife();
                // feedbackManager?.PlayWrong();
                string nextName = recipeManager.GetExpectedIngredient()?.toString() ?? "";
                UIManager.Instance?.ShowTemporaryMessage("FEL -1 liv", 1.5f, nextName); // fel ingrediens UI /kevin
                break;
        }

        if (recipeManager.IsRecipeComplete())
        {
            scoreSystem?.AddRecipeBonus();
            difficultyManager?.IncreaseDifficulty();
            // feedbackManager?.PlayRecipeComplete();
            // recipeManager?.NewRecipe();
        }

        if (lifeSystem != null && lifeSystem.IsDead())
            EndGame();
    }

    private void HandleWrongObjectCaught(WrongObject wrong)
    {
        comboTracker?.OnMistake();
        lifeSystem?.LoseLife();
        // feedbackManager?.PlayWrong();

        UIManager.Instance?.ShowTemporaryMessage("FEL -1 liv", 1.5f, recipeManager.GetExpectedIngredient()?.ToString() ?? ""); // fel objekt fångat /kevin

        if (lifeSystem != null && lifeSystem.IsDead())
            EndGame();
    }

    private void HandleGenericWrong()
    {
        comboTracker?.OnMistake();
        lifeSystem?.LoseLife();
        // feedbackManager?.PlayWrong();

        if (lifeSystem != null && lifeSystem.IsDead())
            EndGame();
    }

    // ────────────────────────────────────────────────────────────────
    // Publika hjälpmetoder (anropas av subsystem)
    // ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Sätter hastighetsmultiplikatorn. Anropas av BuffObject och NerfObject.
    /// </summary>
    public void SetFallSpeedMultiplier(float multiplier)
    {
        currentSpeedMultiplier = multiplier;
    }

    /// <summary>
    /// Drar ett liv. Anropas av PanStack när stapeln tippar.
    /// </summary>
    public void LoseLife()
    {
        lifeSystem?.LoseLife();

        if (lifeSystem != null && lifeSystem.IsDead())
            EndGame();
    }
}*/