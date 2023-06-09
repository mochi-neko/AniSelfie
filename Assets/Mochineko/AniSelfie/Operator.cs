#nullable enable
using System;
using System.IO;
using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Mocopi.Receiver;
using UniRx;
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

        [SerializeField]
        private MocopiSimpleReceiver? mocopiReceiver = default;

        [SerializeField]
        private int mocopiPort = 12351;

        [SerializeField]
        private CinemachineVirtualCamera? orbitalCamera = default;

        [SerializeField]
        private CinemachineFollowZoom? followZoom = default;
        
        private InputSettings? inputSettings;
        
        private async void Start()
        {
            if (mocopiReceiver == null)
            {
                throw new NullReferenceException(nameof(mocopiReceiver));
            }

            if (orbitalCamera == null)
            {
                throw new NullReferenceException(nameof(orbitalCamera));
            }
            
            if (followZoom == null)
            {
                throw new NullReferenceException(nameof(followZoom));
            }

            CancellationToken cancellationToken = this.GetCancellationTokenOnDestroy();

            if (!File.Exists(vrmModelFilePath))
            {
                Log.Error("[AniSelfie] VRM model file not found at: {0}", vrmModelFilePath);
                return;
            }

            var bytes = await File.ReadAllBytesAsync(vrmModelFilePath, cancellationToken);
            if (bytes == null)
            {
                Log.Error("[AniSelfie] Failed to read VRM model file at: {0}", vrmModelFilePath);
                return;
            }

            var vrm = await Vrm10.LoadBytesAsync(
                bytes: bytes,
                canLoadVrm0X: true,
                controlRigGenerationOption: ControlRigGenerationOption.None,
                showMeshes: true,
                awaitCaller: new RuntimeOnlyAwaitCaller(),
                ct: cancellationToken
            );

            Log.Info("[AniSelfie] VRM model loaded from: {0}.", vrmModelFilePath);

            var animator = vrm.transform.GetComponent<Animator>();
            if (animator == null)
            {
                throw new NullReferenceException(nameof(animator));
            }

            var avatar = animator.gameObject.AddComponent<MocopiAvatar>();
            mocopiReceiver
                .AvatarSettings
                .Add(
                    new MocopiSimpleReceiver.MocopiSimpleReceiverAvatarSettings(
                        avatar,
                        mocopiPort
                    )
                );

            Log.Info("[AniSelfie] Start receiving mocopi data on port: {0}.", mocopiPort);

            mocopiReceiver.StartReceiving();

            Log.Info("[AniSelfie] Setup virtual cameras.");

            var head = vrm.Humanoid.Head.transform;
            orbitalCamera.Follow = head;
            orbitalCamera.LookAt = head;

            inputSettings = new InputSettings();
            
            inputSettings
                .Player
                .Capture
                .OnPerformedAsObservable()
                .Subscribe(_ => CaptureImage())
                .AddTo(this);
            
            inputSettings
                .Player
                .CaptureWithDelay
                .OnPerformedAsObservable()
                .Subscribe(_ => CaptureImageWithDelay())
                .AddTo(this);

            inputSettings.Enable();

            Log.Info("[AniSelfie] AniSelfie started.");
        }

        private void Update()
        {
            if (followZoom == null)
            {
                throw new NullReferenceException(nameof(followZoom));
            }
            
            if (inputSettings == null)
            {
                return;
            }
            
            followZoom.m_Width += inputSettings
                .Player
                .Zoom
                .ReadValue<float>();
        }

        private void OnDestroy()
        {
            if (mocopiReceiver == null)
            {
                throw new NullReferenceException(nameof(mocopiReceiver));
            }

            mocopiReceiver.StopReceiving();
            
            inputSettings?.Dispose();
        }
        
        private void CaptureImage()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                throw new NullReferenceException(nameof(Camera.main));
            }
            
            Log.Info("[AniSelfie] Capture image.");

            CameraCapture.CaptureCameraAndSaveAsync(
                    camera,
                    this.GetCancellationTokenOnDestroy())
                .Forget();
        }
        
        private void CaptureImageWithDelay()
        {
            var camera = Camera.main;
            if (camera == null)
            {
                throw new NullReferenceException(nameof(Camera.main));
            }
            
            Log.Info("[AniSelfie] Capture image with delay.");

            CameraCapture.CaptureCameraAndSaveAsync(
                    camera,
                    this.GetCancellationTokenOnDestroy(),
                    delay: TimeSpan.FromSeconds(5f))
                .Forget();
        }
    }
}