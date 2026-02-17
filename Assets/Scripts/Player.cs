using UnityEngine;

public class Player : MonoBehaviour
{

    int lives = 3;

  

    void loseLife() // Tar bort ett liv när fel ingrediens fångas
    {
        lives--;
    }

    bool isAlive() // Returnerar om spelaren har liv kvar
    {
        return lives > 0;
    }

    void ResetLives() //Återställer till 3 liv vid ny omgång
    {
        lives = 3;
    }
}
