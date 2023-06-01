#nullable enable
using UnityEngine;

namespace Mochineko.AniSelfie
{
    /// <summary>
    /// Captures the image of the camera.
    /// </summary>
    internal static class CameraCapture
    {
        /// <summary>
        /// Captures the image of the camera.
        /// </summary>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static Texture2D Capture(Camera camera)
        {
            
            // Render camera to render texture
            RenderTexture renderTexture = new RenderTexture(
                camera.pixelWidth,
                camera.pixelHeight,
                depth: 24
            );
            camera.targetTexture = renderTexture;
            camera.Render();

            RenderTexture.active = renderTexture;

            // Copy pixels from render texture to texture 2D
            Texture2D screenShot = new Texture2D(
                renderTexture.width,
                renderTexture.height,
                TextureFormat.RGBA32,
                mipChain: false
            );
            screenShot.ReadPixels(
                source: new Rect(
                    x: 0,
                    y: 0,
                    renderTexture.width,
                    renderTexture.height
                ),
                destX: 0,
                destY: 0
            );
            screenShot.Apply();

            // Clean up
            camera.targetTexture = null;
            RenderTexture.active = null;
            Object.Destroy(renderTexture);

            return screenShot;
        }
    }
}