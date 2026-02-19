using System;
using UnityEngine;

public abstract class FallingObject : MonoBehaviour
{
    [SerializeField] protected float fallSpeed = 3f;

    // Unity anropar denna automatiskt — ingen parameter
    protected virtual void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }

    public abstract void OnCaught(PanController pan);

    protected virtual void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
