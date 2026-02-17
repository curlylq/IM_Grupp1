using System;
using UnityEngine;

public class FallingObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public bool isCought = false;
    

    int positionX ()
    {
        return (int)transform.position.x;
    }
  

}
