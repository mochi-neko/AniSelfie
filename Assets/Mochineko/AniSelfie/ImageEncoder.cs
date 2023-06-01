#nullable enable
using UnityEngine;

namespace Mochineko.AniSelfie
{
    /// <summary>
    /// An image encoder with PNG format.
    /// </summary>
    internal static class ImageEncoder
    {
        /// <summary>
        /// Encodes the image to PNG format.
        /// </summary>
        /// <param name="texture">The image to encode.</param>
        /// <returns>The encoded image.</returns>
        public static byte[] Encode(Texture2D texture)
        {
            return texture.EncodeToPNG();
        }
        
        // TODO: Encode PNG on a thread pool asynchronously.
    }
}