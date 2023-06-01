#nullable enable
using System;
using Cinemachine;
using Unity.Logging;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mochineko.AniSelfie
{
    /// <summary>
    /// An extension for Cinemachine to control zooming with input.
    /// </summary>
    internal sealed class CinemachineInputZoom : CinemachineExtension
    {
        [SerializeField]
        private InputActionReference? actionReference = default;

        [SerializeField]
        private float scale = 1f;

        [SerializeField, Range(1, 179)]
        private float minFOV = 10;

        [SerializeField, Range(1, 179)]
        private float maxFOV = 90;

        private float scrollDelta = 0f;
        private float fov = 0f;

        protected override void Awake()
        {
            base.Awake();
            
            if (actionReference == null)
            {
                throw new NullReferenceException(nameof(actionReference));
            }
            
            actionReference.action.Enable();
        }

        private void Update()
        {
            if (actionReference == null)
            {
                throw new NullReferenceException(nameof(actionReference));
            }

            var scroll = actionReference.action.ReadValue<float>();
            scrollDelta += scroll;
        }

        public override bool RequiresUserInput => true;

        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage,
            ref CameraState state,
            float deltaTime)
        {
            if (stage != CinemachineCore.Stage.Aim)
            {
                return;
            }

            var lens = state.Lens;

            if (!Mathf.Approximately(scrollDelta, 0))
            {
                fov = Mathf.Clamp(
                    value: fov - scrollDelta * scale,
                    min: minFOV - lens.FieldOfView,
                    max: maxFOV - lens.FieldOfView
                );

                scrollDelta = 0;
            }

            lens.FieldOfView += fov;

            state.Lens = lens;
        }
    }
}