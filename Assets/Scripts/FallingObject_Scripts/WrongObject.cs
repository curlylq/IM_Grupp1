using UnityEngine;

public class WrongObject : FallingObject
{
    public string reason = "Wrong ingredient!";

    public override void OnCaught(PanController pan)
    {
        Debug.Log($"Wrong object caught: {reason}");
        GameManager.Instance.LoseLife();
        Destroy(gameObject);
    }
}