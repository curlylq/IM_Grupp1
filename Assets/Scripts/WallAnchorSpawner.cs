using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class WallAnchorSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private Transform spawnerRoot;

    [Header("Wall Marker")]
    [SerializeField] private string wallImageName = "WallMarker";

    [Header("Placement")]
    [Tooltip("Meters in front of the wall marker (along its forward). Flip sign if it ends up behind.")]
    [SerializeField] private float forwardOffset = 0.25f;

    [Tooltip("Meters up from marker center.")]
    [SerializeField] private float upOffset = 0.0f;

    [Tooltip("Optional rotation offset.")]
    [SerializeField] private Vector3 rotationOffsetEuler = Vector3.zero;

    [Header("Behavior")]
    [SerializeField] private bool anchorOnce = true;

    [Header("StartTimer")]
    public UIManager UIManager;
    public TimeManager TimeManager;

    private bool anchored;

    private void Awake()
    {
        if (trackedImageManager == null)
            trackedImageManager = Object.FindFirstObjectByType<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackablesChanged.AddListener(OnChanged);
    }

    private void OnDisable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackablesChanged.RemoveListener(OnChanged);
    }

    private void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var img in args.added) TryLock(img);
        foreach (var img in args.updated) TryLock(img);
    }

    private void TryLock(ARTrackedImage img)
    {
        if (anchored && anchorOnce) return;
        if (img.trackingState != TrackingState.Tracking) return;
        if (img.referenceImage.name != wallImageName) return;
        if (spawnerRoot == null) return;

        // Compute pose once
        Quaternion rotOffset = Quaternion.Euler(rotationOffsetEuler);
        Quaternion worldRot = img.transform.rotation * rotOffset;

        Vector3 worldPos =
            img.transform.position +
            img.transform.forward * forwardOffset +
            img.transform.up * upOffset;

        // IMPORTANT: detach from any parent so it doesn't inherit AR/XR transforms
        spawnerRoot.SetParent(null, true);

        spawnerRoot.SetPositionAndRotation(worldPos, worldRot);

        // -------- DEBUG: prove what moved --------
        Debug.Log($"[WallAnchorSpawner] LOCKED '{wallImageName}' at pos {spawnerRoot.position} rot {spawnerRoot.rotation.eulerAngles}");
        Debug.Log($"[WallAnchorSpawner] SpawnerRoot children: {spawnerRoot.childCount}");
        foreach (Transform child in spawnerRoot)
            Debug.Log($"  - child '{child.name}' worldPos {child.position}");

        // -------- START SPAWNING (only after lock) --------
        var simpleSpawner = spawnerRoot.GetComponentInChildren<SimpleSpawner>(true);
        if (simpleSpawner != null)
        {
            
            TimeManager.countDown();
            
            Debug.Log("[WallAnchorSpawner] Started SimpleSpawner.");
        }
        else
        {
            Debug.LogError("[WallAnchorSpawner] No SimpleSpawner found under SpawnerRoot!");
        }

        anchored = true;

        if (anchorOnce)
            enabled = false;
    }
}