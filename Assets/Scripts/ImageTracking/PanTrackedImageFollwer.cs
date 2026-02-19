using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PanTrackedImageFollower : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private Transform panProxy;

    [Header("If you have multiple markers, set the name to follow (optional)")]
    public string referenceImageName = "";

    [Header("Alignment offsets (tune these to match the real pan)")]
    public Vector3 positionOffsetMeters = Vector3.zero;     // local to marker
    public Vector3 rotationOffsetEuler = Vector3.zero;      // degrees

    [Header("Smoothing")]
    [Range(0f, 30f)] public float positionLerp = 20f;
    [Range(0f, 30f)] public float rotationLerp = 20f;

    private bool hasTarget;
    private Vector3 targetPos;
    private Quaternion targetRot;

    private void OnEnable()
    {
        if (trackedImageManager == null)
            trackedImageManager = FindObjectOfType<ARTrackedImageManager>();

        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var img in args.added) TryUpdateTarget(img);
        foreach (var img in args.updated) TryUpdateTarget(img);
    }

    private void TryUpdateTarget(ARTrackedImage img)
    {
        if (img.trackingState != TrackingState.Tracking) return;

        if (!string.IsNullOrEmpty(referenceImageName) &&
            img.referenceImage.name != referenceImageName) return;

        // Marker pose
        Pose markerPose = new Pose(img.transform.position, img.transform.rotation);

        // Apply offsets so PanProxy lines up with the real pan surface
        Quaternion rotOffset = Quaternion.Euler(rotationOffsetEuler);
        Vector3 pos = markerPose.position + markerPose.rotation * positionOffsetMeters;
        Quaternion rot = markerPose.rotation * rotOffset;

        targetPos = pos;
        targetRot = rot;
        hasTarget = true;
    }

    private void Update()
    {
        if (!hasTarget || panProxy == null) return;

        panProxy.position = Vector3.Lerp(panProxy.position, targetPos, Time.deltaTime * positionLerp);
        panProxy.rotation = Quaternion.Slerp(panProxy.rotation, targetRot, Time.deltaTime * rotationLerp);
    }
}
