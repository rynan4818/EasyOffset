using System;
using EasyOffset.Configuration;
using UnityEngine;

namespace EasyOffset {
    public static class Abomination {
        #region Transforms

        public static Vector3 LeftPosition = Vector3.zero;
        public static Quaternion LeftRotation = Quaternion.identity;
        public static Vector3 RightPosition = Vector3.zero;
        public static Quaternion RightRotation = Quaternion.identity;
        public static event Action<Vector3, Quaternion, Vector3, Quaternion> TransformsUpdatedEvent;

        public static void UpdateTransforms(Vector3 leftPosition, Quaternion leftRotation, Vector3 rightPosition, Quaternion rightRotation) {
            if (PluginConfig.SmoothingEnabled) {
                var t = Time.deltaTime * PluginConfig.SmoothingSpeed;
                LeftPosition = Vector3.Lerp(LeftPosition, leftPosition, t);
                LeftRotation = Quaternion.Lerp(LeftRotation, leftRotation, t);
                RightPosition = Vector3.Lerp(RightPosition, rightPosition, t);
                RightRotation = Quaternion.Lerp(RightRotation, rightRotation, t);
            } else {
                LeftPosition = leftPosition;
                LeftRotation = leftRotation;
                RightPosition = rightPosition;
                RightRotation = rightRotation;
            }

            TransformsUpdatedEvent?.Invoke(LeftPosition, LeftRotation, RightPosition, RightRotation);
        }

        #endregion

        #region GripButtonEvents

        public static event Action<Hand> GripButtonPressedEvent;
        public static event Action<Hand> GripButtonReleasedEvent;

        public static void OnGripButtonPressed(Hand hand) {
            GripButtonPressedEvent?.Invoke(hand);
        }

        public static void OnGripButtonReleased(Hand hand) {
            GripButtonReleasedEvent?.Invoke(hand);
        }

        #endregion
    }
}