using UnityEngine;

public class PanController : MonoBehaviour
{

    float tiltAngle = 15f;
    float maxTilt= 30f; // Maximum tilt angle in degrees

    int stackCount = 0;
    Vector2 position;


    void UpdateTilt(float input) //Uppdaterar lutning baserat på spelarinput
    {
        tiltAngle += input;
        tiltAngle = Mathf.Clamp(tiltAngle, -maxTilt, maxTilt);
        transform.rotation = Quaternion.Euler(tiltAngle, 0, 0);
    }

    bool CheckBalance() //Kontrollerar om ingredienser riskerar ramla av
    {
        return stackCount < 5; // Example condition for balance
    }
     void OnTriggerEnter(Collider other)
     {
         if (other.CompareTag("FallingObject"))
         {
             stackCount++;
             Destroy(other.gameObject);
         }
    }

    void CatchIngredient(object fallingObject) //Hanterar när en ingrediens fångas
    {
        stackCount++;
        // Additional logic to handle the caught ingredient
    }

    void DropStack() //Tömmer stacken om balansen förloras
    {
        stackCount = 0;
        // Additional logic to handle dropping the stack
    }

    bool IsInSafeZone() //Kontrollerar om pannan är i safe space vid start
    {
        return position.x > -5 && position.x < 5; // Example safe zone condition
    }

}
