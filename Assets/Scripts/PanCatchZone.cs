using UnityEngine;

public class PanCatchZone : MonoBehaviour
{
    [Header("Feedback")]
    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioClip correctClip;
    [SerializeField] private AudioClip wrongClip;

    [Header("Wrong ingredient handling")]
    [SerializeField] private bool destroyWrongOnEnter = true;

    private PanMotion panMotion;

    private void Awake()
    {
        // PanMotion should be on PanProxy (parent)
        panMotion = GetComponentInParent<PanMotion>();
        if (panMotion == null)
            Debug.LogError("PanCatchZone: No PanMotion found on parent. Add PanMotion to PanProxy.");
    }

    private void OnTriggerEnter(Collider other)
    {
        var ing = other.GetComponentInParent<FallingObject>();
        if (ing == null) return;

        if (!ing.isCorrect)
        {
            if (sfx && wrongClip) sfx.PlayOneShot(wrongClip);
            if (destroyWrongOnEnter) Destroy(ing.gameObject);
            return;
        }

        ing.SetInCatchZone(true, panMotion);
        if (sfx && correctClip) sfx.PlayOneShot(correctClip);
    }

    private void OnTriggerExit(Collider other)
    {
        var ing = other.GetComponentInParent<FallingObject>();
        if (ing == null) return;

        ing.SetInCatchZone(false, null);
    }
}
