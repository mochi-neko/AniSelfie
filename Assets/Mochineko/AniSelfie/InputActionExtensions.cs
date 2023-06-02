#nullable enable
using System;
using UniRx;
using UnityEngine.InputSystem;

namespace Mochineko.AniSelfie
{
    internal static class InputActionExtensions
    {
        public static IObservable<InputAction.CallbackContext> OnStartedAsObservable(
            this InputAction self)
            => Observable
                .FromEvent<Action<InputAction.CallbackContext>, InputAction.CallbackContext>(
                    conversion: action => action.Invoke,
                    addHandler: action => self.started += action,
                    removeHandler: action => self.started -= action
                );
        
        public static IObservable<InputAction.CallbackContext> OnPerformedAsObservable(
            this InputAction self)
            => Observable
                .FromEvent<Action<InputAction.CallbackContext>, InputAction.CallbackContext>(
                    conversion: action => action.Invoke,
                    addHandler: action => self.performed += action,
                    removeHandler: action => self.performed -= action
                );
        
        public static IObservable<InputAction.CallbackContext> OnCancelledAsObservable(
            this InputAction self)
            => Observable
                .FromEvent<Action<InputAction.CallbackContext>, InputAction.CallbackContext>(
                    conversion: action => action.Invoke,
                    addHandler: action => self.canceled += action,
                    removeHandler: action => self.canceled -= action
                );
    }
}