//  
// Copyright (c) 2017 Vulcan, Inc. All rights reserved.  
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
//

namespace HoloLensCameraStream
{
    /// <summary>
    /// When calling VideoCapture.StartPhotoModeAsync, you must pass in a CameraParameters object
    /// that contains the various settings that the web camera will use.
    /// </summary>
    public struct CameraParameters
    {
        /// <summary>
        /// The pixel format used to capture and record your image data.
        /// </summary>
        public CapturePixelFormat pixelFormat;

        /// <summary>
        /// The height of the image frame. Must be a valid option.
        /// Valid options can be obtained from VideoCapture.SupportedResolutions.
        /// </summary>
        public int cameraResolutionHeight;

        /// <summary>
        /// The width of the image frame. Must be a valid option.
        /// Valid options can be obtained from VideoCapture.SupportedResolutions.
        /// </summary>
        public int cameraResolutionWidth;

        /// <summary>
        /// The frames per second that the video will stream at. Must be supported based on the selected resolution.
        /// </summary>
        public int frameRate;

        public CameraParameters(
            CapturePixelFormat pixelFormat = CapturePixelFormat.BGRA32,
            int cameraResolutionHeight = 720,
            int cameraResolutionWidth = 1280,
            int frameRate = 30)
        {
            this.pixelFormat = pixelFormat;
            this.cameraResolutionHeight = cameraResolutionHeight;
            this.cameraResolutionWidth = cameraResolutionWidth;
            this.frameRate = frameRate;
        }
    }
}
