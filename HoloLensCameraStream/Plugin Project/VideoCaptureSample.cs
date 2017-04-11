//  
// Copyright (c) 2017 Vulcan, Inc. All rights reserved.  
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Graphics.Imaging;
using Windows.Media.Capture.Frames;

namespace HoloLensCameraStream
{
    public class VideoCaptureSample
    {
        /// <summary>
        /// The guid for getting the view transform from the frame sample.
        /// See https://developer.microsoft.com/en-us/windows/mixed-reality/locatable_camera#locating_the_device_camera_in_the_world
        /// </summary>
        static Guid viewTransformGuid = new Guid("4E251FA4-830F-4770-859A-4B8D99AA809B");

        /// <summary>
        /// The guid for getting the projection transform from the frame sample.
        /// See https://developer.microsoft.com/en-us/windows/mixed-reality/locatable_camera#locating_the_device_camera_in_the_world
        /// </summary>
        static Guid projectionTransformGuid = new Guid("47F9FCB5-2A02-4F26-A477-792FDF95886A");

        /// <summary>
        /// How many bytes are in the frame.
        /// There are four bytes per pixel, times the width and height of the bitmap.
        /// </summary>
        public int dataLength
        {
            get
            {
                return 4 * bitmap.PixelHeight * bitmap.PixelWidth;
            }
        }

        /// <summary>
        /// Note: This method has not been written. Help us out on GitHub!
        /// Will be true if the HoloLens knows where it is and is tracking.
        /// Indicates that obtaining the matrices will be successful.
        /// </summary>
        public bool hasLocationData
        {
            get
            {
                //TODO: Return if location data exists.
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// The format of the frames that the bitmap stream is sending.
        /// </summary>
        public CapturePixelFormat pixelFormat { get; private set; }

        //Internal members

        internal SoftwareBitmap bitmap { get; private set; }

        internal bool isBitmapCopied { get; private set; }

        //Private members

        MediaFrameReference frameReference;

        internal VideoCaptureSample(MediaFrameReference frameReference)
        {
            if (frameReference == null)
            {
                throw new ArgumentNullException("frameReference.");
            }

            this.frameReference = frameReference;
            bitmap = frameReference.VideoMediaFrame.SoftwareBitmap;

            //TODO: Get location data
            //var transformMatrix = frameReference.Properties[TransformMatrixGuid] as SomeMatrixOrArray;
            //var projectionMatrix = frameReference.Properties[ProjectionMatrixGuid] as SomeMatrixOrArray;
        }

        /// <summary>
        /// If you need safe, long term control over the image bytes in this frame, they will need to be
        /// copied. You need to supply a byte[] to copy them into. It is best to pre-allocate and reuse
        /// this byte array to minimize unecessarily high memory ceiling or unnecessary garbage collections.
        /// </summary>
        /// <param name="byteBuffer">A byte array with a length the size of VideoCaptureSample.dataLength</param>
        public void CopyRawImageDataIntoBuffer(byte[] byteBuffer)
        {
            //Here is a potential way to get direct access to the buffer:
            //http://stackoverflow.com/questions/25481840/how-to-change-mediacapture-to-byte

            if (byteBuffer == null)
            {
                throw new ArgumentNullException("byteBuffer");
            }

            if (byteBuffer.Length < dataLength)
            {
                throw new IndexOutOfRangeException("Your byteBuffer is not big enough." +
                    " Please use the VideoCaptureSample.dataLength property to allocate a large enough array.");
            }

            bitmap.CopyToBuffer(byteBuffer.AsBuffer());
            isBitmapCopied = true;
        }


        public void CopyRawImageDataIntoBuffer(List<byte> byteBuffer)
        {
            throw new NotSupportedException("This method is not yet supported with a List<byte>. Please provide a byte[] instead.");
        }

        /// <summary>
        /// This returns the transform matrix at the time the photo was captured, if location data if available.
        /// If it's not, that is probably an indication that the HoloLens is not tracking and its location is not known.
        /// It could also mean the VideoCapture stream is not running.
        /// If location data is unavailable then the camera to world matrix will be set to the identity matrix.
        /// </summary>
        /// <param name="matrix">The transform matrix used to convert between coordinate spaces.
        /// The matrix will have to be converted to a Unity matrix before it can be used by methods in the UnityEngine namespace.
        /// See https://forum.unity3d.com/threads/locatable-camera-in-unity.398803/ for details.</param>
        public byte[] TryGetCameraToWorldMatrix()
        {
            //TODO: Figure out how to use a Unity matrix in a plugin.
            if (frameReference.Properties.ContainsKey(viewTransformGuid) == false)
            {
                return GetIdentityMatrixByteArray();
            }

            return frameReference.Properties[viewTransformGuid] as byte[];
        }

        /// <summary>
        /// This returns the transform matrix at the time the photo was captured, if location data if available.
        /// If it's not, that is probably an indication that the HoloLens is not tracking and its location is not known.
        /// It could also mean the VideoCapture stream is not running.
        /// If location data is unavailable then the camera to world matrix will be set to the identity matrix.
        /// </summary>
        /// <param name="matrix">The projection matrix used to match the true camera projection.
        /// The matrix will have to be converted to a Unity matrix before it can be used by methods in the UnityEngine namespace.
        /// See https://forum.unity3d.com/threads/locatable-camera-in-unity.398803/ for details.</param>
        public byte[] TryGetProjectionMatrix()
        {
            if (frameReference.Properties.ContainsKey(projectionTransformGuid) == false)
            {
                return GetIdentityMatrixByteArray();
            }

            return frameReference.Properties[projectionTransformGuid] as byte[];
        }

        /// <summary>
        /// Note: This method hasn't been written yet. Help us out on GitHub!
        /// </summary>
        /// <param name="targetTexture"></param>
        public void UploadImageDataToTexture(object targetTexture)
        {
            //TODO: Figure out how to use a Texture2D in a plugin.
            throw new NotSupportedException("I'm not sure how to use a Texture2D within this plugin.");
        }

        /// <summary>
        /// When done with the VideoCapture class, you will need to dispose it to release unmanaged memory.
        /// </summary>
        public void Dispose()
        {
            bitmap.Dispose();
            frameReference.Dispose();
        }

        private Matrix4x4 ConvertByteArrayToMatrix4x4(byte[] matrixAsBytes)
        {
            if (matrixAsBytes == null)
            {
                throw new ArgumentNullException("matrixAsBytes");
            }

            if (matrixAsBytes.Length != 64)
            {
                throw new Exception("Cannot convert byte[] to Matrix4x4. Size of array should be 64, but it is " + matrixAsBytes.Length);
            }

            var m = matrixAsBytes;
            return new Matrix4x4(
                BitConverter.ToSingle(m, 0),
                BitConverter.ToSingle(m, 4),
                BitConverter.ToSingle(m, 8),
                BitConverter.ToSingle(m, 12),
                BitConverter.ToSingle(m, 16),
                BitConverter.ToSingle(m, 20),
                BitConverter.ToSingle(m, 24),
                BitConverter.ToSingle(m, 28),
                BitConverter.ToSingle(m, 32),
                BitConverter.ToSingle(m, 36),
                BitConverter.ToSingle(m, 40),
                BitConverter.ToSingle(m, 44),
                BitConverter.ToSingle(m, 48),
                BitConverter.ToSingle(m, 52),
                BitConverter.ToSingle(m, 56),
                BitConverter.ToSingle(m, 60));
        }

        static CapturePixelFormat ConvertBitmapPixelFormatToCapturePixelFormat(BitmapPixelFormat format)
        {
            switch (format)
            {
                case BitmapPixelFormat.Bgra8:
                    return CapturePixelFormat.BGRA32;
                case BitmapPixelFormat.Nv12:
                    return CapturePixelFormat.NV12;
                default:
                    return CapturePixelFormat.Unknown;
            }
        }

        /// <summary>
        /// We're not yet returning real matrices from the plugin to Unity. Until we do that, we're passing byte[].
        /// </summary>
        static byte[] GetIdentityMatrixByteArray()
        {
            return new byte[] { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 };
        }
    }
}
