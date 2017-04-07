using UnityEngine;
using UnityEngine.UI;

public class VideoPanel : MonoBehaviour
{
    public RawImage rawImage;

    public void SetResolution(int width, int height)
    {
        var texture = new Texture2D(width, height, TextureFormat.BGRA32, false);
        rawImage.texture = texture;
    }

    public void SetBytes(byte[] image)
    {
        var texture = rawImage.texture as Texture2D;
        texture.LoadRawTextureData(image); //TODO: Should be able to do this: texture.LoadRawTextureData(pointerToImage, 1280 * 720 * 4);
        texture.Apply();
    }
}
