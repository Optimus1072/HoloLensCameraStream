namespace HoloLensCameraStream
{
    /// <summary>
    /// A structure for holding a resolution.
    /// </summary>
    public struct Resolution
    {
        /// <summary>
        /// The width property.
        /// </summary>
        public readonly int width;

        /// <summary>
        /// The height property.
        /// </summary>
        public readonly int height;

        public Resolution(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }
}
