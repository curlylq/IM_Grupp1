using UnityEngine;

public class PanController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int tiltAngle()
    {
        return (int)transform.rotation.eulerAngles.z;
    }

    int maxTiltAngle()
    {
        return 30;
    }

    int stackCount()
    {
        return transform.childCount;
    }
}
