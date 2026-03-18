using ExtralityLab;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PanTrackedImageFollower : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private Transform panProxy;

    [Header("MQTT")]
    [SerializeField] private MqttClientExampleBidirectional mqttClient;

    [Header("Marker")]
    [SerializeField] private string referenceImageName = "PanMarker";

    [Header("Alignment offsets")]
    [SerializeField] private Vector3 positionOffsetMeters = Vector3.zero;
    [SerializeField] private Vector3 rotationOffsetEuler = Vector3.zero;

    [Header("Follow smoothing")]
    [Range(1f, 40f)][SerializeField] private float positionLerp = 18f;
    [Range(1f, 40f)][SerializeField] private float rotationLerp = 18f;

    [Header("Lost tracking handling")]
    [SerializeField] private float lostTrackingGraceTime = 0.2f;
    [SerializeField] private float maxPredictionTime = 0.35f;
    [SerializeField] private float predictionDamping = 6f;
    [SerializeField] private float teleportDistance = 0.35f;

    [Header("Debug")]
    [SerializeField] private bool verboseLogs = false;

    private Rigidbody panRb;

    private bool hasPose;
    private bool currentlyTracked;
    private bool ledIsOn;

    private Vector3 desiredPos;
    private Quaternion desiredRot;

    private Vector3 predictedVelocity;
    private Vector3 lastTrackedPos;
    private Quaternion lastTrackedRot;
    private bool hasPreviousTrackedPose;

    private float lastSeenTime;
    private float lostTimer;

    private void Awake()
    {
        if (trackedImageManager == null)
            trackedImageManager = FindFirstObjectByType<ARTrackedImageManager>();

        if (panProxy != null)
        {
            panRb = panProxy.GetComponent<Rigidbody>();
            if (panRb == null)
            {
                Debug.LogError("[PanTrackedImageFollower] PanProxy needs a Rigidbody.");
            }
            else if (!panRb.isKinematic)
            {
                Debug.LogWarning("[PanTrackedImageFollower] PanProxy Rigidbody should be Kinematic.");
            }
        }
    }

    private void OnEnable()
    {
        if (trackedImageManager == null)
        {
            Debug.LogError("[PanTrackedImageFollower] Missing ARTrackedImageManager.");
            enabled = false;
            return;
        }

        trackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
    }

    private void OnDisable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
    }

    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        bool foundTrackedPoseThisEvent = false;

        foreach (var img in args.added)
        {
            if (TryConsumeImagePose(img))
                foundTrackedPoseThisEvent = true;
        }

        foreach (var img in args.updated)
        {
            if (TryConsumeImagePose(img))
                foundTrackedPoseThisEvent = true;
        }

        currentlyTracked = AnyMatchingImageTracked();

        UpdateLed(currentlyTracked);

        if (!currentlyTracked && verboseLogs)
            Debug.Log("[PanTrackedImageFollower] Marker not currently tracked.");
    }

    private bool TryConsumeImagePose(ARTrackedImage img)
    {
        if (img == null)
            return false;

        if (!string.IsNullOrEmpty(referenceImageName) &&
            img.referenceImage.name != referenceImageName)
            return false;

        // Testa gärna != None först. Vissa devices ger användbar pose även i Limited.
        if (img.trackingState == TrackingState.None)
            return false;

        Pose rawPose = new Pose(img.transform.position, img.transform.rotation);
        Quaternion rotOffset = Quaternion.Euler(rotationOffsetEuler);

        Vector3 newPos = rawPose.position + rawPose.rotation * positionOffsetMeters;
        Quaternion newRot = rawPose.rotation * rotOffset;

        float now = Time.time;

        if (hasPreviousTrackedPose)
        {
            float dt = Mathf.Max(0.0001f, now - lastSeenTime);
            predictedVelocity = (newPos - lastTrackedPos) / dt;
        }

        lastTrackedPos = newPos;
        lastTrackedRot = newRot;
        lastSeenTime = now;
        lostTimer = 0f;
        hasPreviousTrackedPose = true;
        hasPose = true;

        desiredPos = newPos;
        desiredRot = newRot;

        if (verboseLogs)
            Debug.Log($"[PanTrackedImageFollower] Tracking pose from {img.referenceImage.name} at {newPos}");

        return true;
    }

    private bool AnyMatchingImageTracked()
    {
        if (trackedImageManager == null)
            return false;

        foreach (var tracked in trackedImageManager.trackables)
        {
            if (!string.IsNullOrEmpty(referenceImageName) &&
                tracked.referenceImage.name != referenceImageName)
                continue;

            if (tracked.trackingState != TrackingState.None)
                return true;
        }

        return false;
    }

    private void Update()
    {
        if (!hasPose)
            return;

        if (!currentlyTracked)
        {
            lostTimer += Time.deltaTime;

            // Håll senaste pose en kort stund
            if (lostTimer <= lostTrackingGraceTime)
            {
                desiredPos = lastTrackedPos;
                desiredRot = lastTrackedRot;
                return;
            }

            // Enkel prediktion en kort stund istället för att frysa direkt
            float predictTime = Mathf.Min(lostTimer - lostTrackingGraceTime, maxPredictionTime);
            if (predictTime > 0f)
            {
                float damping = Mathf.Exp(-predictionDamping * predictTime);
                Vector3 v = predictedVelocity * damping;

                desiredPos = lastTrackedPos + v * predictTime;
                desiredRot = lastTrackedRot;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!hasPose || panProxy == null)
            return;

        float dt = Time.fixedDeltaTime;

        // Om tracking hoppar långt vid återfångst, teleportera istället för att släpa efter
        float dist = Vector3.Distance(panProxy.position, desiredPos);
        if (dist > teleportDistance)
        {
            if (panRb != null)
            {
                panRb.position = desiredPos;
                panRb.rotation = desiredRot;
            }
            else
            {
                panProxy.SetPositionAndRotation(desiredPos, desiredRot);
            }
            return;
        }

        Vector3 newPos = Vector3.Lerp(panProxy.position, desiredPos, dt * positionLerp);
        Quaternion newRot = Quaternion.Slerp(panProxy.rotation, desiredRot, dt * rotationLerp);

        if (panRb != null)
        {
            panRb.MovePosition(newPos);
            panRb.MoveRotation(newRot);
        }
        else
        {
            panProxy.SetPositionAndRotation(newPos, newRot);
        }
    }

    private void UpdateLed(bool isTrackingNow)
    {
        if (mqttClient == null)
            return;

        if (isTrackingNow && !ledIsOn)
        {
            mqttClient.PublishTopicValue("true");
            ledIsOn = true;
        }
        else if (!isTrackingNow && ledIsOn)
        {
            mqttClient.PublishTopicValue("false");
            ledIsOn = false;
        }
    }
}