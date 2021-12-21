using System;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace EasyOffset {
    [UsedImplicitly]
    public class GizmosManager : IInitializable, IDisposable, ILateTickable {
        #region Init/Dispose

        public GizmosController LeftHandGizmosController;
        public GizmosController RightHandGizmosController;

        public void Initialize() {
            LeftHandGizmosController = InstantiateGizmosController();
            RightHandGizmosController = InstantiateGizmosController();

            PluginConfig.AdjustmentModeChangedEvent += OnAdjustmentModeChanged;
            PluginConfig.ControllerTypeChangedEvent += OnControllerTypeChanged;
            Abomination.TransformsUpdatedEvent += OnControllerTransformsChanged;

            UpdateVisibility();
            OnControllerTypeChanged(PluginConfig.DisplayControllerType);
        }

        public void Dispose() {
            // if (LeftHandGizmosController != null && LeftHandGizmosController.gameObject != null) {
                // Object.Destroy(LeftHandGizmosController.gameObject);
            // }

            // if (RightHandGizmosController != null && RightHandGizmosController.gameObject != null) {
                // Object.Destroy(RightHandGizmosController.gameObject);
            // }

            PluginConfig.AdjustmentModeChangedEvent -= OnAdjustmentModeChanged;
            PluginConfig.ControllerTypeChangedEvent -= OnControllerTypeChanged;
            Abomination.TransformsUpdatedEvent -= OnControllerTransformsChanged;
        }

        #endregion

        #region LateTick

        public void LateTick() {
            LeftHandGizmosController.SetPivotPosition(PluginConfig.LeftHandPivotPosition);
            LeftHandGizmosController.SetSaberDirection(PluginConfig.LeftHandSaberDirection);

            RightHandGizmosController.SetPivotPosition(PluginConfig.RightHandPivotPosition);
            RightHandGizmosController.SetSaberDirection(PluginConfig.RightHandSaberDirection);

            var mainCamera = Camera.main;
            if (mainCamera == null) return;
            var cameraWorldPosition = mainCamera.transform.position;
            RightHandGizmosController.SetCameraPosition(cameraWorldPosition);
            LeftHandGizmosController.SetCameraPosition(cameraWorldPosition);
        }

        #endregion

        #region Events

        private void OnControllerTransformsChanged(ReeTransform leftHandTransform, ReeTransform rightHandTransform) {
            var leftPos = leftHandTransform.Position;
            var leftRot = leftHandTransform.Rotation;
            var rightPos = rightHandTransform.Position;
            var rightRot = rightHandTransform.Rotation;
            TransformUtils.ApplyRoomOffset(ref leftPos, ref leftRot);
            TransformUtils.ApplyRoomOffset(ref rightPos, ref rightRot);
            LeftHandGizmosController.SetControllerTransform(leftPos, leftRot);
            RightHandGizmosController.SetControllerTransform(rightPos, rightRot);
        }

        private void OnAdjustmentModeChanged(AdjustmentMode adjustmentMode) {
            UpdateVisibility();
        }

        private void OnControllerTypeChanged(ControllerType controllerType) {
            LeftHandGizmosController.SetControllerType(controllerType, Hand.Left);
            RightHandGizmosController.SetControllerType(controllerType, Hand.Right);
            UpdateVisibility();
        }

        #endregion

        #region Visibility

        private void UpdateVisibility() {
            GetVisibilityValues(
                PluginConfig.AdjustmentMode,
                PluginConfig.DisplayControllerType,
                out var isPivotVisible,
                out var isSphericalBasisVisible,
                out var isOrthonormalBasisVisible,
                out var isOrthonormalBasisPointerVisible,
                out var isControllerModelVisible,
                out var isSwingPreviewVisible
            );

            LeftHandGizmosController.SetVisibility(
                isPivotVisible,
                isSphericalBasisVisible,
                isOrthonormalBasisVisible,
                isOrthonormalBasisPointerVisible,
                isControllerModelVisible,
                isSwingPreviewVisible
            );

            RightHandGizmosController.SetVisibility(
                isPivotVisible,
                isSphericalBasisVisible,
                isOrthonormalBasisVisible,
                isOrthonormalBasisPointerVisible,
                isControllerModelVisible,
                isSwingPreviewVisible
            );
        }

        private static void GetVisibilityValues(
            AdjustmentMode adjustmentMode,
            ControllerType controllerType,
            out bool isPivotVisible,
            out bool isSphericalBasisVisible,
            out bool isOrthonormalBasisVisible,
            out bool isOrthonormalBasisPointerVisible,
            out bool isControllerModelVisible,
            out bool isSwingPreviewVisible
        ) {
            switch (adjustmentMode) {
                case AdjustmentMode.None:
                    isPivotVisible = false;
                    isOrthonormalBasisVisible = false;
                    isOrthonormalBasisPointerVisible = false;
                    isSphericalBasisVisible = false;
                    isControllerModelVisible = true;
                    isSwingPreviewVisible = false;
                    break;
                case AdjustmentMode.Basic:
                    isPivotVisible = true;
                    isOrthonormalBasisVisible = controllerType == ControllerType.None;
                    isOrthonormalBasisPointerVisible = false;
                    isSphericalBasisVisible = false;
                    isControllerModelVisible = true;
                    isSwingPreviewVisible = true;
                    break;
                case AdjustmentMode.Position:
                    isPivotVisible = true;
                    isOrthonormalBasisVisible = true;
                    isOrthonormalBasisPointerVisible = true;
                    isSphericalBasisVisible = false;
                    isControllerModelVisible = true;
                    isSwingPreviewVisible = true;
                    break;
                case AdjustmentMode.Rotation:
                    isPivotVisible = true;
                    isOrthonormalBasisVisible = false;
                    isOrthonormalBasisPointerVisible = false;
                    isSphericalBasisVisible = true;
                    isControllerModelVisible = true;
                    isSwingPreviewVisible = true;
                    break;
                case AdjustmentMode.SwingBenchmark:
                    isPivotVisible = true;
                    isOrthonormalBasisVisible = false;
                    isOrthonormalBasisPointerVisible = false;
                    isSphericalBasisVisible = false;
                    isControllerModelVisible = true;
                    isSwingPreviewVisible = false;
                    break;
                case AdjustmentMode.RotationAuto:
                    isPivotVisible = true;
                    isOrthonormalBasisVisible = false;
                    isOrthonormalBasisPointerVisible = false;
                    isSphericalBasisVisible = true;
                    isControllerModelVisible = true;
                    isSwingPreviewVisible = true;
                    break;
                case AdjustmentMode.RoomOffset:
                    isPivotVisible = false;
                    isOrthonormalBasisVisible = false;
                    isOrthonormalBasisPointerVisible = false;
                    isSphericalBasisVisible = false;
                    isControllerModelVisible = true;
                    isSwingPreviewVisible = false;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(adjustmentMode), adjustmentMode, null);
            }
        }

        #endregion

        #region Utils

        private static GizmosController InstantiateGizmosController() {
            var gameObject = Object.Instantiate(BundleLoader.GizmosController);
            // Object.DontDestroyOnLoad(gameObject);
            return gameObject.GetComponent<GizmosController>();
        }

        #endregion
    }
}