#nullable enable
using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Logging;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Mochineko.AniSelfie
{
    /// <summary>
    /// Captures the image of the camera.
    /// </summary>
    internal static class CameraCapture
    {
        /// <summary>
        /// Captures the image of the camera and save.
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="delay"></param>
        public static async UniTask CaptureCameraAndSaveAsync(
            Camera camera,
            CancellationToken cancellationToken,
            TimeSpan delay = default)
        {
            await UniTask.Delay(delay, DelayType.DeltaTime, PlayerLoopTiming.Update, cancellationToken);

            var capturedTexture = Capture(camera);

            // TODO: Encode image on a thread pool.
            var encoded = ImageEncoder.Encode(capturedTexture);

            var path = GetSavePath();

            await File.WriteAllBytesAsync(path, encoded, cancellationToken);

            Log.Info("[AniSelfie] Capture image saved to path:{0}.", path);
        }

        private static Texture2D Capture(Camera camera)
        {
            // Render camera to render texture
            var renderTexture = new RenderTexture(
                camera.pixelWidth,
                camera.pixelHeight,
                depth: 24
            );
            camera.targetTexture = renderTexture;
            camera.Render();

            RenderTexture.active = renderTexture;

            // Copy pixels from render texture to texture 2D
            var screenShot = new Texture2D(
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

        private static string GetSavePath()
        {
            var directory = Application.isEditor
                ? Path.Combine(Application.dataPath, "/../Selfies/")
                : Path.Combine(Application.dataPath, "/Selfies/");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var now = DateTime.Now;
            var fileName =
                $"AniSelfie_{now.Year:0000}{now.Month:00}{now.Day:00}_{now.Hour:00}{now.Minute:00}{now.Second:00}_{now.Millisecond:000}.png";

            return Path.Combine(directory, fileName);
        }
    }
}