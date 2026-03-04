using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ExtralityLab;

public class PanTrackedImageFollower : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private Transform panProxy;

    [Header("MQTT")]
    [SerializeField] private MqttClientExampleBidirectional mqttClient;

    [Header("If you have multiple markers, set the name to follow (optional)")]
    public string referenceImageName = "";

    [Header("Alignment offsets (tune these to match the real pan)")]
    public Vector3 positionOffsetMeters = Vector3.zero;
    public Vector3 rotationOffsetEuler = Vector3.zero;

    [Header("Smoothing")]
    [Range(0f, 30f)] public float positionLerp = 20f;
    [Range(0f, 30f)] public float rotationLerp = 20f;

    private bool hasTarget;
    private bool ledIsOn = false;
    private bool hasCalled = false;
    private Vector3 targetPos;
    private Quaternion targetRot;

    // Helps reduce log spam by logging only when something changes
    private bool lastAnyTracking = false;

    private void Awake()
    {
        Debug.Log("[PanTrackedImageFollower] Awake()");

        if (trackedImageManager == null)
        {
            trackedImageManager = FindObjectOfType<ARTrackedImageManager>();
            Debug.Log($"[PanTrackedImageFollower] trackedImageManager auto-found: {(trackedImageManager != null)}");
        }

        Debug.Log($"[PanTrackedImageFollower] mqttClient assigned: {(mqttClient != null)}");
        Debug.Log($"[PanTrackedImageFollower] referenceImageName filter: '{referenceImageName}'");
        Debug.Log($"[PanTrackedImageFollower] panProxy assigned: {(panProxy != null)}");
    }

    private void OnEnable()
    {
        Debug.Log("[PanTrackedImageFollower] OnEnable()");
        if (trackedImageManager == null)
        {
            Debug.LogError("[PanTrackedImageFollower] trackedImageManager is NULL. Cannot track images.");
            return;
        }

        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        Debug.Log("[PanTrackedImageFollower] Subscribed to trackedImagesChanged.");
    }

    private void OnDisable()
    {
        Debug.Log("[PanTrackedImageFollower] OnDisable()");
        if (trackedImageManager != null)
        {
            trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
            Debug.Log("[PanTrackedImageFollower] Unsubscribed from trackedImagesChanged.");
        }
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        Debug.Log($"[PanTrackedImageFollower] trackedImagesChanged | added:{args.added.Count} updated:{args.updated.Count} removed:{args.removed.Count}");

        // Log added/updated/removed with details
        foreach (var img in args.added)
            LogImage("ADDED", img);

        foreach (var img in args.updated)
            LogImage("UPDATED", img);

        foreach (var img in args.removed)
            LogImage("REMOVED", img);

        // Update follow target pose when image is TRACKING
        foreach (var img in args.added) UpdatePoseIfMatch(img);
        foreach (var img in args.updated) UpdatePoseIfMatch(img);

        // Decide LED state based on whether ANY matching image is currently TRACKING
        bool anyTracking = AnyMatchingImageTracking();

        if (anyTracking != lastAnyTracking)
        {
            Debug.Log($"[PanTrackedImageFollower] anyTracking changed -> {anyTracking}");
            lastAnyTracking = anyTracking;
        }

        // Validate mqttClient reference
        if (mqttClient == null)
        {
            Debug.LogError("[PanTrackedImageFollower] mqttClient is NULL (Inspector not assigned?). LED publish cannot happen.");
            return;
        }

        // Validate MQTT connection
        if (!mqttClient.IsConnected)
        {
            Debug.LogWarning("[PanTrackedImageFollower] MQTT not connected yet. Skipping publish this frame.");
            // Important: Do NOT flip ledIsOn here, because we did not publish.
            return;
        }

        // Publish only on state change
        if (anyTracking && !ledIsOn)
        {
            Debug.Log("[PanTrackedImageFollower] AR -> MQTT publish TRUE");
            mqttClient.PublishTopicValue("true");
            ledIsOn = true;
        }
        else if (!anyTracking && ledIsOn)
        {
            Debug.Log("[PanTrackedImageFollower] AR -> MQTT publish FALSE");
            mqttClient.PublishTopicValue("false");
            ledIsOn = false;
        }
    }

    private bool AnyMatchingImageTracking()
    {
        if (trackedImageManager == null) return false;

        foreach (var tracked in trackedImageManager.trackables)
        {
            // Filter by name if set
            if (!string.IsNullOrEmpty(referenceImageName) &&
                tracked.referenceImage.name != referenceImageName)
                continue;

            if (tracked.trackingState == TrackingState.Tracking)
                return true;
        }
        return false;
    }

    private void UpdatePoseIfMatch(ARTrackedImage img)
    {
        if (img == null) return;

        // Filter by name if set
        if (!string.IsNullOrEmpty(referenceImageName) &&
            img.referenceImage.name != referenceImageName)
        {
            Debug.Log($"[PanTrackedImageFollower] Ignoring image '{img.referenceImage.name}' due to filter '{referenceImageName}'");
            return;
        }

        // Only update target pose when actually TRACKING
        if (img.trackingState != TrackingState.Tracking)
        {
            Debug.Log($"[PanTrackedImageFollower] Image '{img.referenceImage.name}' is not TRACKING (state: {img.trackingState}), not updating pose.");
            return;
        }

        //Markera att vi hittat den genom ett call till MQTT

        if (!hasCalled)
        {
            mqttClient.PublishTopicValue("true");
            hasCalled = true;
        };


        Pose markerPose = new Pose(img.transform.position, img.transform.rotation);
        Quaternion rotOffset = Quaternion.Euler(rotationOffsetEuler);

        targetPos = markerPose.position + markerPose.rotation * positionOffsetMeters;
        targetRot = markerPose.rotation * rotOffset;

        hasTarget = true;

        Debug.Log($"[PanTrackedImageFollower] Updated pose from '{img.referenceImage.name}' | targetPos:{targetPos} targetRotEuler:{targetRot.eulerAngles}");
    }

    private void LogImage(string label, ARTrackedImage img)
    {
        if (img == null)
        {
            Debug.Log($"[PanTrackedImageFollower] {label} image: NULL");
            return;
        }

        Debug.Log($"[PanTrackedImageFollower] {label} image '{img.referenceImage.name}' | state:{img.trackingState} " +
                  $"pos:{img.transform.position} rot:{img.transform.rotation.eulerAngles}");
    }

    private void Update()
    {
        if (!hasTarget || panProxy == null) return;

        panProxy.position = Vector3.Lerp(panProxy.position, targetPos, Time.deltaTime * positionLerp);
        panProxy.rotation = Quaternion.Slerp(panProxy.rotation, targetRot, Time.deltaTime * rotationLerp);
    }
}



/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ExtralityLab; // LÄGG TILL


public class PanTrackedImageFollower : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private Transform panProxy;

    [Header("MQTT")] // LÄGG TILL
    [SerializeField] private MqttClientExampleBidirectional mqttClient; // LÄGG TILL

    [Header("If you have multiple markers, set the name to follow (optional)")]
    public string referenceImageName = "";
    [Header("Alignment offsets (tune these to match the real pan)")]
    public Vector3 positionOffsetMeters = Vector3.zero;
    public Vector3 rotationOffsetEuler = Vector3.zero;
    [Header("Smoothing")]
    [Range(0f, 30f)] public float positionLerp = 20f;
    [Range(0f, 30f)] public float rotationLerp = 20f;

    private bool hasTarget;
    private bool ledIsOn = false; // LÄGG TILL
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

        // LÄGG TILL - släck LED om imagen försvinner
        foreach (var img in args.removed)
        {
            if (ledIsOn)
            {
                mqttClient?.PublishTopicValue("false");
                ledIsOn = false;
            }
        }
    }

    private void TryUpdateTarget(ARTrackedImage img)
    {
        if (img.trackingState == TrackingState.None)
        {
            if (ledIsOn)
            {
                mqttClient?.PublishTopicValue("false");
                ledIsOn = false;
            }
            return;
        }

        if (!string.IsNullOrEmpty(referenceImageName) &&
            img.referenceImage.name != referenceImageName) return;

        if (!ledIsOn)
        {
            mqttClient?.PublishTopicValue("true");
            ledIsOn = true;
        }

        Pose markerPose = new Pose(img.transform.position, img.transform.rotation);
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
}*/