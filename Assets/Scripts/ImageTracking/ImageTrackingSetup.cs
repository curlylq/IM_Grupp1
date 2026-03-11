using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageTrackingSetup : MonoBehaviour
{
    [SerializeField] ARTrackedImageManager trackedImageManager;

    void Awake()
    {
        trackedImageManager.requestedMaxNumberOfMovingImages = 2;
    }

    void OnEnable()
    {
        trackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
    }

    void OnDisable()
    {
        trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
    }

    void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var image in args.updated)
        {
            UnityEngine.Debug.Log($"Tracking state: {image.trackingState}");
        }
    }
}