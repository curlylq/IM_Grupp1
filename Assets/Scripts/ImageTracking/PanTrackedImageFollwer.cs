using ExtralityLab;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PanTrackedImageFollower : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private Transform panProxy;

    [Header("MQTT")]
    [SerializeField] private MqttClientExampleBidirectional mqttClient;

    [Header("If you have multiple markers, set the name to follow (optional)")]
    public string referenceImageName = "PanMarker";

    [Header("Alignment offsets (tune these to match the real pan)")]
    public Vector3 positionOffsetMeters = Vector3.zero;
    public Vector3 rotationOffsetEuler = Vector3.zero;

    [Header("Smoothing")]
    [Range(0f, 30f)] public float positionLerp = 20f;
    [Range(0f, 30f)] public float rotationLerp = 20f;

    [Header("Debug")]
    [SerializeField] private bool verboseLogs = false;

    private bool hasTarget;
    private bool ledIsOn = false;
    private bool hasCalled = false;

    private Vector3 targetPos;
    private Quaternion targetRot;

    private bool lastAnyTracking = false;
    private Rigidbody panRb;

    private void Awake()
    {
        if (trackedImageManager == null)
            trackedImageManager = Object.FindFirstObjectByType<ARTrackedImageManager>();

        if (panProxy != null)
        {
            panRb = panProxy.GetComponent<Rigidbody>();
            if (panRb == null)
            {
                Debug.LogError("[PanTrackedImageFollower] PanProxy has no Rigidbody. Add one and set it Kinematic.");
            }
            else
            {
                if (!panRb.isKinematic)
                    Debug.LogWarning("[PanTrackedImageFollower] PanProxy Rigidbody should be Kinematic for MovePosition/MoveRotation.");
            }
        }

        if (verboseLogs)
        {
            Debug.Log($"[PanTrackedImageFollower] trackedImageManager: {(trackedImageManager != null)}");
            Debug.Log($"[PanTrackedImageFollower] mqttClient: {(mqttClient != null)}");
            Debug.Log($"[PanTrackedImageFollower] referenceImageName: '{referenceImageName}'");
            Debug.Log($"[PanTrackedImageFollower] panProxy: {(panProxy != null)} rb: {(panRb != null)}");
        }
    }

    private void OnEnable()
    {
        if (trackedImageManager == null)
        {
            Debug.LogError("[PanTrackedImageFollower] trackedImageManager is NULL. Cannot track images.");
            return;
        }

        trackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);

        if (verboseLogs)
            Debug.Log("[PanTrackedImageFollower] Subscribed to trackablesChanged.");
    }

    private void OnDisable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);

        if (verboseLogs)
            Debug.Log("[PanTrackedImageFollower] Unsubscribed from trackablesChanged.");
    }

    private void OnTrackedImagesChanged(UnityEngine.XR.ARFoundation.ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        // Update follow target pose when image is TRACKING
        foreach (var img in args.added) UpdatePoseIfMatch(img);
        foreach (var img in args.updated) UpdatePoseIfMatch(img);

        // Decide LED state based on whether ANY matching image is currently TRACKING
        bool anyTracking = AnyMatchingImageTracking();

        if (anyTracking != lastAnyTracking && verboseLogs)
            Debug.Log($"[PanTrackedImageFollower] anyTracking changed -> {anyTracking}");

        lastAnyTracking = anyTracking;

        // MQTT is optional; don't block tracking if it's missing
        if (mqttClient == null || !mqttClient.IsConnected)
            return;

        // Publish only on state change
        if (anyTracking && !ledIsOn)
        {
            mqttClient.PublishTopicValue("true");
            ledIsOn = true;
        }
        else if (!anyTracking && ledIsOn)
        {
            mqttClient.PublishTopicValue("false");
            ledIsOn = false;
        }
    }

    private bool AnyMatchingImageTracking()
    {
        if (trackedImageManager == null) return false;

        foreach (var tracked in trackedImageManager.trackables)
        {
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

        if (!string.IsNullOrEmpty(referenceImageName) &&
            img.referenceImage.name != referenceImageName)
            return;

        if (img.trackingState != TrackingState.Tracking)
            return;

        // Optional one-time call when first found
        if (!hasCalled && mqttClient != null && mqttClient.IsConnected)
        {
            mqttClient.PublishTopicValue("true");
            hasCalled = true;
        }

        Pose markerPose = new Pose(img.transform.position, img.transform.rotation);
        Quaternion rotOffset = Quaternion.Euler(rotationOffsetEuler);

        targetPos = markerPose.position + markerPose.rotation * positionOffsetMeters;
        targetRot = markerPose.rotation * rotOffset;

        hasTarget = true;

        if (verboseLogs)
            Debug.Log($"[PanTrackedImageFollower] Target updated from '{img.referenceImage.name}' pos:{targetPos} rot:{targetRot.eulerAngles}");
    }

    private void FixedUpdate()
    {
        if (!hasTarget || panProxy == null) return;

        // Smooth toward target in physics time-step
        float dt = Time.fixedDeltaTime;
        Vector3 newPos = Vector3.Lerp(panProxy.position, targetPos, dt * positionLerp);
        Quaternion newRot = Quaternion.Slerp(panProxy.rotation, targetRot, dt * rotationLerp);

        // Move through physics so rigidbodies ride the pan
        if (panRb != null)
        {
            panRb.MovePosition(newPos);
            panRb.MoveRotation(newRot);
        }
        else
        {
            // Fallback: still works visually, but physics will be worse
            panProxy.SetPositionAndRotation(newPos, newRot);
        }
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