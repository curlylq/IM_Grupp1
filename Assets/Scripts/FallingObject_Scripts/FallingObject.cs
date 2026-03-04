using System;
using UnityEngine;

public abstract class FallingObject : MonoBehaviour
{
    [SerializeField] protected float fallSpeed = 3f;
    [SerializeField] private float despawnBelowStart = 8f; // meters below where it spawned

    private float startY;

    protected virtual void Start()
    {
        startY = transform.position.y;
    }

    protected virtual void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        if (transform.position.y < startY - despawnBelowStart)
            Destroy(gameObject);
    }

    public abstract void OnCaught(PanController pan);
}
