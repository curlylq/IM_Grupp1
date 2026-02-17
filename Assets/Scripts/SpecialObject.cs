using UnityEngine;

public class SpecialObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    string effectType()
    {
        return "Speed Boost";
    }   

    int duration()
    {
        return 5; // Duration in seconds
    }

    bool isBuff()
    {
        return true; // Indicates if the effect is a buff (true) or a debuff (false)
    }

}
