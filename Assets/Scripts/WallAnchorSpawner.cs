using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class WallAnchorSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ARTrackedImageManager trackedImageManager;

    [Tooltip("Root object containing SpawnArea + SimpleSpawner. This will be moved to the wall anchor.")]
    [SerializeField] private Transform spawnerRoot;

    [Header("Wall Marker")]
    [Tooltip("Reference image name for the wall marker (must match XR Reference Image Library entry).")]
    [SerializeField] private string wallImageName = "WallMarker";

    [Header("Anchor Behavior")]
    [Tooltip("If true, we anchor once and stop updating the spawner position (most stable).")]
    [SerializeField] private bool anchorOnce = true;

    [Tooltip("Optional offsets relative to the wall image pose.")]
    [SerializeField] private Vector3 positionOffsetMeters = Vector3.zero;
    [SerializeField] private Vector3 rotationOffsetEuler = Vector3.zero;

    private ARAnchor anchor;
    private bool anchored;

    private void Awake()
    {
        if (trackedImageManager == null)
            trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnChanged;
    }

    private void OnChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var img in args.added) TryAnchor(img);
        foreach (var img in args.updated) TryAnchor(img);
    }

    private void TryAnchor(ARTrackedImage img)
    {
        if (img.trackingState != TrackingState.Tracking) return;
        if (img.referenceImage.name != wallImageName) return;
        if (spawnerRoot == null) return;

        // If we already anchored and only want to do it once, ignore later updates
        if (anchored && anchorOnce) return;

        // Create anchor on first time
        if (anchor == null)
        {
            anchor = img.gameObject.AddComponent<ARAnchor>();
        }

        // Apply offsets
        Quaternion rotOffset = Quaternion.Euler(rotationOffsetEuler);
        Pose p = new Pose(img.transform.position, img.transform.rotation);

        Vector3 worldPos = p.position + p.rotation * positionOffsetMeters;
        Quaternion worldRot = p.rotation * rotOffset;

        // Move anchor to desired pose (anchor follows tracked image)
        anchor.transform.SetPositionAndRotation(worldPos, worldRot);

        // Parent spawner under the anchor so it becomes stable in world space
        spawnerRoot.SetParent(anchor.transform, worldPositionStays: true);

        anchored = true;
    }
}