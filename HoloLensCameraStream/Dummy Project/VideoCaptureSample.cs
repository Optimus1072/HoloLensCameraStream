using System;
using System.Collections.Generic;

namespace HoloLensCameraStream
{
    public class VideoCaptureSample
    {
        public int dataLength { get; private set; }

        public bool hasLocationData { get; private set; }

        public CapturePixelFormat pixelFormat { get; private set; }

        public void CopyRawImageDataIntoBuffer(byte[] byteBuffer)
        {
            throw new NotImplementedException();
        }
        
        public void CopyRawImageDataIntoBuffer(List<byte> byteBuffer)
        {
            throw new NotImplementedException();
        }

        public byte[] TryGetCameraToWorldMatrix()
        {
            throw new NotImplementedException();
        }

        public byte[] TryGetProjectionMatrix()
        {
            throw new NotImplementedException();
        }

        public void UploadImageDataToTexture(object targetTexture)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
