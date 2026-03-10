using UnityEngine;

/// <summary>
/// Hanterar spelarens styrning av stekpannan via XR-input (hand/kontroller).
/// Läser horisontell position och lutningsvinkel, och rapporterar
/// till GameManager när objekt fångas.
/// </summary>
/*public class PanController : MonoBehaviour
{
    [Header("Rörelse")]
    [SerializeField] private float xMin = -4f;
    [SerializeField] private float xMax = 4f;

    [Header("Lutning")]
    [SerializeField] private float maxTiltAngle = 45f;

    [Header("Referenser")]
    [SerializeField] private StartZone startZone;
    [SerializeField] private PanStack panStack;

    // Publika egenskaper som andra klasser kan läsa
    public float xPosition { get; private set; }
    public float tiltAngle { get; private set; }

    private void Update()
    {
        ReadXRInput();

        // Kolla stabilitet varje frame och tappa item om det tippar
        if (panStack.IsUnstable(tiltAngle))
        {
            panStack.RemoveTop();
            GameManager.Instance.LoseLife();
        }
    }

    /// <summary>
    /// Läser XR-handens position och rotation och översätter till
    /// pannans xPosition och tiltAngle.
    /// </summary>
    public void ReadXRInput()
    {
        // Hämta handtransformens världsposition och rotation
        // I ett XR-projekt kopplar ni detta mot er XR-rigg,
        // t.ex. via XR Interaction Toolkit eller OpenXR
        Transform handTransform = transform; // Ersätt med er XR-hand-referens

        // Horisontell position — kläms inom banans gränser
        xPosition = Mathf.Clamp(handTransform.position.x, xMin, xMax);

        // Lutning baseras på handtransformens Z-rotation (roll)
        float rawTilt = handTransform.rotation.eulerAngles.z;

        // eulerAngles returnerar 0–360, konvertera till -180 till 180
        if (rawTilt > 180f) rawTilt -= 360f;

        tiltAngle = Mathf.Clamp(rawTilt, -maxTiltAngle, maxTiltAngle);

        // Applicera på transform
        transform.position = new Vector3(xPosition, transform.position.y, transform.position.z);
        transform.rotation = Quaternion.Euler(0f, 0f, tiltAngle);
    }

    /// <summary>
    /// Returnerar true om pannan befinner sig inom startzonens område.
    /// Används för att trigga spelstart enligt "hold pan here"-mekaniken.
    /// </summary>
    public bool IsInStartZone()
    {
        return startZone != null && startZone.Contains(this);
    }

    /// <summary>
    /// Anropas av ett FallingObject när det kolliderar med pannan.
    /// Skickar vidare till GameManager för spellogik.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        FallingObject fallingObj = other.GetComponent<FallingObject>();
        if (fallingObj != null)
        {
            GameManager.Instance.OnObjectCought(fallingObj);
            fallingObj.OnCaught(this);
        }
    }
}*/