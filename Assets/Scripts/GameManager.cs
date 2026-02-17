using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    

    public static GameManager Instance;

    public GameState state = GameState.WaitingToStart;

    public int lives = 3;
    public int score = 0;

    public float fallSpeedMultiplier = 1.0f;
    public float difficultyIncreaseRate = 0.05f;


    public float safeSpaceDuration = 3.0f; // Duration in seconds for the safe space at the start of the game
    private float safeSpaceTimer; 



    private void Awake() // Singleton pattern för att säkerställa en enda instans av GameManager
    {
        Instance = this;
    }


   

   

    
    private void Update() //Huvudloop som uppdaterar alla komponenter
    {
        if (state == GameState.WaitingToStart)
        {
            if (Player.Instance.IsHoldingStartArea())
                StartGame();
        }

        if (state == GameState.Playing)
        {
            fallSpeedMultiplier += difficultyIncreaseRate * Time.deltaTime;
        }
    }

    void StartGame() // Initierar spelet, visar startområdet
    {
        state = GameState.Playing;
        safeSpaceTimer = safeSpaceDuration;
    }

    public bool IsSafeSpaceActive() //Kontrollerar om safe space är aktivt
    {
        return safeSpaceTimer > 0f;
    }

   public void LoseLife() // Hanterar när spelaren förlorar ett liv
    {
        lives--;
        if (lives <= 0)
        {
            GameOver();
        }
    }

    public void AddScore (int amount) //Lägger till poäng när rätt ingrediens fångas
    {
        score += amount;
    }

    void GameOver() // Hanterar spelets
    {
        state = GameState.GameOver;
    }



}
