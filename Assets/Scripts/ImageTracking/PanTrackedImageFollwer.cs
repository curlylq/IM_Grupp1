using System.Collections.Generic;
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
        // LÄGG TILL - släck LED om tracking tappas
        if (img.trackingState != TrackingState.Tracking)
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

        // LÄGG TILL - tänd LED när imagen hittas
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
}