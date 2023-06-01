#nullable enable
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UniVRM10;
using VRMShaders;
using Unity.Logging;

namespace Mochineko.AniSelfie
{
    /// <summary>
    /// The operator of AniSelfie.
    /// </summary>
    internal sealed class Operator : MonoBehaviour
    {
        [SerializeField]
        private string vrmModelFilePath = string.Empty;

        private async void Start()
        {
            if (!File.Exists(vrmModelFilePath))
            {
                Log.Error($"VRM model file not found at: {vrmModelFilePath}");
                return;
            }

            var bytes = await File.ReadAllBytesAsync(vrmModelFilePath);
            if (bytes == null)
            {
                Log.Error($"Failed to read VRM model file at: {vrmModelFilePath}");
                return;
            }

            var vrm = await LoadVRMModelAsync(
                binary: bytes,
                cancellationToken: this.GetCancellationTokenOnDestroy()
            );

            Log.Info("VRM model loaded.");
        }

        private static async UniTask<Vrm10Instance> LoadVRMModelAsync(
            byte[] binary,
            CancellationToken cancellationToken)
            => await Vrm10.LoadBytesAsync(
                bytes: binary,
                canLoadVrm0X: true,
                controlRigGenerationOption: ControlRigGenerationOption.None,
                showMeshes: true,
                awaitCaller: new RuntimeOnlyAwaitCaller(),
                ct: cancellationToken
            );
    }
}