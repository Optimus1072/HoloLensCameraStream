using UnityEngine;
using System;
using HoloLensCameraStream;
using System.Linq;
using System.Collections.Generic;

public class Example1 : MonoBehaviour
{
    VideoCapture videoCapture;
    byte[] latestImageBytes;
    HoloLensCameraStream.Resolution resolution;

    void Start()
    {
        //Call this in Start() to ensure that the CameraStreamManager is already "Awake".
        CameraStreamHelper.Instance.GetVideoCaptureAsync(OnVideoCaptureCreated);
        //You could also do this "shortcut":
        //CameraStreamManager.Instance.GetVideoCaptureAsync(v => videoCapture = v);
    }

    private void OnDestroy()
    {
        if (videoCapture != null)
        {
            videoCapture.FrameSampleAcquired -= OnFrameSampleAcquired;
            videoCapture.Dispose();
        }
    }

    void OnVideoCaptureCreated(VideoCapture videoCapture)
    {
        if (videoCapture == null)
        {
            Debug.LogError("Did not find a video capture object. You may not be using the HoloLens.");
            return;
        }

        resolution = CameraStreamHelper.Instance.GetLowestResolution();
        float frameRate = CameraStreamHelper.Instance.GetHighestFrameRate(resolution);
        this.videoCapture = videoCapture;
        //videoCapture.FrameSampleAcquired += OnFrameSampleAcquired;

        Debug.Log("Media capture started. Starting video mode...");

        CameraParameters cameraParams = new CameraParameters();
        cameraParams.cameraResolutionHeight = resolution.height;
        cameraParams.cameraResolutionWidth = resolution.width;
        cameraParams.frameRate = Mathf.RoundToInt(frameRate);
        cameraParams.pixelFormat = CapturePixelFormat.BGRA32;
        videoCapture.StartVideoModeAsync(cameraParams, OnVideoModeStarted);
    }

    void OnVideoModeStarted(VideoCaptureResult result)
    {
        if (result.success == false)
        {
            Debug.LogWarning("Could not start video mode.");
            return;
        }

        Debug.Log("Video mode started. Capturing samples...");
        videoCapture.RequestNextFrameSample(OnFrameSampleAcquired);
    }

    void OnFrameSampleAcquired(VideoCaptureSample sample)
    {
        if (latestImageBytes == null || latestImageBytes.Length < sample.dataLength)
        {
            latestImageBytes = new byte[sample.dataLength];
        }
        sample.CopyRawImageDataIntoBuffer(latestImageBytes);
        sample.Dispose();
    }
}
