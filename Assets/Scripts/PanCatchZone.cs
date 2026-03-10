using UnityEngine;

public class PanCatchZone : MonoBehaviour
{
    [Header("Stack Anchors")]
    [SerializeField] private Transform stackRoot;   // ideally scale (1,1,1)
    [SerializeField] private Transform snapCenter;  // center of burger stack
    [SerializeField] private float extraGap = 0.002f;

    [Header("Thickness Source")]
    [SerializeField] private bool useStackBoundsCollider = true;
    [SerializeField] private float fallbackThickness = 0.03f;

    [Header("Feedback")]
    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioClip correctClip;
    [SerializeField] private AudioClip wrongClip;

    private float currentHeight = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (stackRoot == null || snapCenter == null)
        {
            Debug.LogError("PanCatchZone: stackRoot or snapCenter is not assigned.");
            return;
        }

        var ing = other.GetComponentInParent<FallingObject>();
        if (ing == null) return;
        if (ing.IsCaught) return;

        if (!ing.isCorrect)
        {
            if (sfx && wrongClip) sfx.PlayOneShot(wrongClip);
            Destroy(ing.gameObject);
            return;
        }

        float thickness = Mathf.Max(0.001f, GetThicknessAlongStackUp(ing));

        // Stack in WORLD along stackRoot.up, then convert to local snap pos
        Vector3 baseWorld = snapCenter.position;
        Vector3 worldPos = baseWorld + stackRoot.up * (currentHeight + thickness * 0.5f);
        Vector3 localPos = stackRoot.InverseTransformPoint(worldPos);

        ing.Catch(stackRoot, localPos);

        currentHeight += thickness + extraGap;

        if (sfx && correctClip) sfx.PlayOneShot(correctClip);
    }

    private float GetThicknessAlongStackUp(FallingObject ing)
    {
        if (useStackBoundsCollider)
        {
            Transform sb = ing.transform.Find("StackBounds");
            if (sb != null)
            {
                var box = sb.GetComponent<BoxCollider>();
                if (box != null)
                {
                    Bounds b = box.bounds;
                    Vector3 up = stackRoot.up;

                    // Approx thickness of an oriented box along 'up'
                    float t =
                        Mathf.Abs(Vector3.Dot(Vector3.right, up)) * b.size.x +
                        Mathf.Abs(Vector3.Dot(Vector3.up, up)) * b.size.y +
                        Mathf.Abs(Vector3.Dot(Vector3.forward, up)) * b.size.z;

                    return t;
                }
            }
        }

        if (ing.stackHeight > 0f) return ing.stackHeight;

        var r = ing.GetComponentInChildren<Renderer>();
        if (r != null) return r.bounds.size.y;

        return fallbackThickness;
    }

    public void ResetStack() => currentHeight = 0f;
}
