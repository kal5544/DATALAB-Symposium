/* 
*   NatCam
*   Copyright (c) 2016 Yusuf Olokoba
*/

//#define NATCAM_DOC_GEN_MODE //Internal. Do not use

using System;

#if NATCAM_DOC_GEN_MODE
using Calligraphy;
using DF = DocumentationFactory;
#endif

namespace NatCamU {
    
    namespace Internals {
        
        #if NATCAM_DOC_GEN_MODE
        public class NCDocAttribute : CADescriptionAttribute {
            public NCDocAttribute (int id) : base (Map.summary[id]) {}
            public NCDocAttribute (int id, int sid) : base (Map.documentation[id], Map.summary[sid]) {}
        }
        
        public class NCCodeAttribute : CACodeExampleAttribute {
            public NCCodeAttribute (int id) : base (Map.code[id]) {}
        }
        
        public class NCRefAttribute : CASeeAlsoAttribute {
            public NCRefAttribute (int id) : base (Map.reference[id]) {}
        }
        
        class Map {
            public static readonly string[] documentation = {
                DF.PreviewFrameDescription, //0
                DF.PreviewMatrixDescription,
                DF.HybridFocusFocusModeDescription,
                DF.ReadablePreviewTypeDescription, //3
                DF.Y4Description,
                DF.UV2Description,
                DF.OnNativePreviewUpdateDescription, //6
                DF.NatCamReleaseDescription,
                DF.DeviceCameraSetFocusDescription,
                DF.DeviceCameraZoomRatioDescription, //9
                DF.DeviceCameraSetFramerate0Description,
                DF.DeviceCameraSetFramerate1Description,
                DF.NatCamActiveCameraDescription, //12
                DF.NatCamGesturesTapDescription,
                DF.NatCamGesturesPinchDescription,
                DF.NatCamCapturePhotoDescription, //15
                DF.NatCamRequestBarcodeDescription,
                DF.NatCamInitializeDescription,
                DF.NatCamPhotoSaveModeDescription, //18
                DF.NatCamExecuteOnPreviewStartDescription,
                DF.NatCamNativeInterfacePreviewDescription,
                DF.NatCamNativeInterfaceComponentUpdateDescription, //21
                DF.NatCamNativeInterfaceRenderPipelineDescription,
                DF.NatCamHasPermissionsDescription,
                DF.DeviceCameraSetExposureDescription, //24
                DF.NatCamRequestFaceDescription,
                DF.NatCamSaveToPhotosDescription,
                DF.UnitygramBaseSwitchCameraDescription, //27
            };
            public static readonly string[] summary = {
                DF.OnPhotoCaptureSummary, //0
                DF.OnBarcodeDetectSummary,
                DF.OnPreviewStartSummary,
                DF.OnPreviewUpdateSummary, //3
                DF.PreviewSummary,
                DF.PreviewFrameSummary,
                DF.PreviewMatrixSummary, //6
                DF.NatCamSummary,
                DF.DeviceCameraSummary,
                DF.ResolutionPresetSummary, //9
                DF.FlashSummary,
                DF.FocusModeSummary,
                DF.BarcodeFormatSummary, //12
                DF.SwitchSummary,
                DF.PhotoSaveModeSummary,
                DF.PreviewTypeSummary, //15
                DF.ScaleModeSummary,
                DF.ZoomModeSummary,
                DF.BarcodeSummary, //18
                DF.NatCamPreviewScalerSummary,
                DF.NatCamPreviewZoomerSummary,
                DF.NativePreviewCallbackSummary, //21
                DF.PreviewCallbackSummary,
                DF.BarcodeCallbackSummary,
                DF.PhotoCallbackSummary, //24
                DF.FrameratePresetSummary,
                DF.HDResolutionPresetDescription,
                DF.FullHDResolutionPresetDescription, //27
                DF.HighestResolutionResolutionPresetDescription,
                DF.MediumResolutionResolutionPresetDescription,
                DF.LowestResolutionResolutionPresetDescription, //30
                DF.AutoFlashDescription,
                DF.OnFlashDescription,
                DF.OffFlashDescription, //33
                DF.QRBarcodeFormatDescription,
                DF.EAN13BarcodeFormatDescription,
                DF.EAN8BarcodeFormatDescription, //36
                DF.DataMatrixBarcodeFormatDescription,
                DF.PDF417BarcodeFormatDescription,
                DF.Code128BarcodeFormatDescription, //39
                DF.Code93BarcodeFormatDescription,
                DF.Code39BarcodeFormatDescription,
                DF.AnyBarcodeFormatDescription, //42
                DF.OffSwitchDescription,
                DF.OnSwitchDescription,
                DF.DoNotSavePhotoSaveModeDescription, //45
                DF.SaveToPhotosPhotoSaveModeDescription,
                DF.SaveToAppAlbumPhotoSaveModeDescription,
                DF.NonReadablePreviewTypeDescription, //48
                DF.ReadablePreviewTypeSummary,
                DF.FixedWidthVariableHeightScaleModeDescription,
                DF.FixedHeightVariableWidthScaleModeDescription, //51
                DF.FillScreenScaleModeDescription,
                DF.NoneScaleModeDescription,
                DF.DigitalZoomAsFallbackZoomModeDescription, //54
                DF.ForceDigitalZoomOnlyZoomModeDescription,
                DF.ZoomSpeedOverrideOnlyZoomModeDescription,
                DF.DataBarcodeDescription, //57
                DF.FormatBarcodeDescription,
                DF.BarcodeBarcodeDescription,
                DF.ToStringBarcodeDescription, //60
                DF.FrontCameraDeviceCameraDescription,
                DF.RearCameraDeviceCameraDescription,
                DF.AutoFocusFocusModeDescription, //63
                DF.TapToFocusFocusModeDescription,
                DF.HybridFocusFocusModeSummary,
                DF.OffFocusModeDescription, //66
                DF.PositionCentreDescription,
                DF.ScaleModeScalerDescription,
                DF.OverrideApplyDescription, //69
                DF.ZoomModeZoomerDescription,
                DF.ZoomRatioZoomerDescription,
                DF.ZoomSpeedZoomerDescription, //72
                DF.PinchToZoomZoomerDescription,
                DF.DeviceCameraFacing,
                DF.DeviceCameraActiveResolution, //75
                DF.DeviceCameraFlashSupport,
                DF.DeviceCameraTorchSupport,
                DF.DeviceCameraZoomSupport, //78
                DF.DeviceCameraHFOV,
                DF.DeviceCameraVFOV,
                DF.DeviceCameraFocusMode, //81
                DF.DeviceCameraFlashMode,
                DF.DeviceCameraTorchMode,
                DF.DeviceCameraZoomRatio, //84
                DF.BarcodeRequestSummary,
                DF.BarcodeRequestCallback,
                DF.BarcodeRequestFormat, //87
                DF.BarcodeRequestUnsubscribe,
                DF.BarcodeRequestBarcodeRequest,
                DF.FacingSummary, //90
                DF.FacingRear,
                DF.FacingFront,
                DF.NatCamInterfaceSummary, //93
                DF.NativeInterfaceSummary,
                DF.FallbackInterfaceSummary,
                DF.ComponentBufferTypeSummary, //96
                DF.RGBA32Summary,
                DF.Y4Summary,
                DF.UV2Summary, //99
                DF.RGBA32GPUSummary,
                DF.Y4GPUSummary,
                DF.NatCamNativeInterfaceSummary, //102
                DF.OnNativePreviewUpdateSummary,
                DF.FramerateDefaultSummary,
                DF.FramerateSmoothSummary, //105
                DF.FramerateSlowMotionSummary,
                DF.FramerateHighestSummary,
                DF.FramerateLowestSummary, //108
                DF.NatCamFallbackInterfaceSummary,
                DF.DeviceCameraCamerasSummary,
                DF.DeviceCameraSetResolution0Summary, //111
                DF.DeviceCameraSetResolution1Summary,
                DF.NatCamPlaySummary,
                DF.NatCamPauseSummary, //114
                DF.NatCamReleaseSummary,
                DF.DeviceCameraSetFocusSummary,
                DF.DeviceCameraSetFramerate0Summary, //117
                DF.DeviceCameraSetFramerate1Summary,
                DF.NatCamActiveCameraSummary,
                DF.NatCamGesturesTapSummary, //120
                DF.NatCamGesturesPinchSummary,
                DF.NatCamIsSupportedPlatformSummary,
                DF.NatCamIsPlayingSummary, //123
                DF.NatCamGesturesZoomDeltaSummary,
                DF.NatCamFallbackInterfacePreviewSummary,
                DF.NatCamCapturePhotoSummary, //126
                DF.NatCamRequestBarcodeSummary,
                DF.NatCamInitializeSummary,
                DF.NatCammInterfaceSummary, //129
                DF.NatCamPhotoSaveModeSummary,
                DF.NatCamExecuteOnPreviewStartSummary,
                DF.NatCamFallbackInterfacePreviewBufferSummary, //132
                DF.NatCamFallbackInterfaceFirstFrameSummary,
                DF.NatCamNativeInterfacePreviewSummary,
                DF.NatCamNativeInterfaceComponentUpdateSummary, //135
                DF.NatCamNativeInterfaceRenderPipelineSummary,
                DF.NatCamPreviewScalerOverrideApply0Summary,
                DF.NatCamPreviewScalerOverrideApply1Summary, //138
                DF.NatCamGesturesDescription,
                DF.UnitygramBaseDescription,
                DF.UnitygramBaseInterfaceSummary, //141
                DF.UnitygramBasePreviewTypeSummary,
                DF.UnitygramBaseDetectBarcodesSummary,
                DF.UnitygramBaseFacingSummary, //144
                DF.UnitygramBaseResolutionSummary,
                DF.UnitygramBaseFramerateSummary,
                DF.UnitygramBaseFocusModeSummary, //147
                DF.UnitygramBaseVerboseSummary,
                DF.UnitygramBaseRawImageSummary,
                DF.UnitygramBaseStartSummary, //150
                DF.NatCamHasPermissionsSummary,
                DF.DeviceCameraActivePhotoResolution,
                DF.DeviceCameraMinExposureBias, //153
                DF.DeviceCameraMaxExposureBias,
                DF.DeviceCameraExposureMode,
                DF.DeviceCameraSetExposureSummary, //156
                DF.DeviceCameraSetPhotoResolution0Summary,
                DF.DeviceCameraSetPhotoResolution1Summary,
                DF.OnFaceDetectSummary, //159
                DF.NatCamVerbose,
                DF.NatCamRequestFaceSummary,
                DF.NatCamSaveToPhotosSummary, //162
                DF.UnitygramBaseSwitchCameraSummary,
                DF.FaceCallbackSummary,
                DF.ExposureModeSummary, //165
                DF.ExposureModeAutoExpose,
                DF.ExposureModeLocked,
                DF.FaceSummary, //168
                DF.FaceFaceID,
                DF.FacePosition,
                DF.FaceSize, //171
                DF.FaceRollAngle,
                DF.FaceYawAngle,
                DF.FaceFace, //174
                DF.FaceToString,
                DF.UnitygramBaseOnPreviewStartSummary,
                DF.UnitygramBaseOnPreviewUpdateSummary, //177
                DF.NatCamIsInitializedSummary,
            };
            public static readonly string[] code = {
                DF.InitializeExample, //0
                DF.BarcodeToStringExample,
                DF.Initialize1Example,
                DF.OverrideApplyExample, //3
                DF.SetActiveCameraExample,
                DF.NatCamNativeInterfaceOnNativePreviewUpdateExample,
                DF.NatCamPreviewMatrixExample, //6
                DF.SetResolutionIntExample,
                DF.SetResolutionResolutionPresetExample,
                DF.SetFramerateFloatExample, //9
                DF.SetFramerateFrameratePresetExample,
                DF.NatCamReleaseExample,
                DF.NatCamRequestBarcodeExample, //12
                DF.CapturePhotoDelegateSubscriptionExample,
                DF.CapturePhotoDelegateExample,
                DF.CapturePhotoLambdaExample, //15
                DF.ExecuteOnPreviewStartExample,
                DF.DeviceCameraSetFocusModeExample,
                DF.DeviceCameraCamerasExample, //18
                DF.NatCamHasPermissionsExample,
                DF.DeviceCameraSetExposureModeExample,
                DF.DeviceCameraSetFocusExample, //21
                DF.DeviceCameraSetExposureExample,
                DF.SetPhotoResolutionResolutionPresetExample,
                DF.SetPhotoResolutionIntExample, //24
                DF.NatCamRequestFaceExample,
                DF.NatCamSaveToPhotosExample,
                DF.UnitygramBaseSwitchCameraNoParamExample, //27
                DF.UnitygramBaseSwitchCameraIntExample,
                DF.UnitygramBaseSwitchCameraDeviceCameraExample,
                DF.FaceToStringExample, //30
                DF.UnitygramBaseOnPreviewStartExample,
                DF.UnitygramBaseOnPreviewUpdateExample,
            };
            public static readonly Type[][] reference = {
                new [] {typeof(ScaleMode)}, //0
                new [] {typeof(ZoomMode)},
                new [] {typeof(Switch)},
                new [] {typeof(Facing)}, //3
                new [] {typeof(FlashMode)},
                new [] {typeof(FocusMode)},
                new [] {typeof(BarcodeCallback)}, //6
                new [] {typeof(BarcodeFormat)},
                new [] {typeof(ResolutionPreset)},
                new [] {typeof(FrameratePreset)}, //9
                new [] {typeof(NatCamInterface), typeof(PreviewType), typeof(Switch)},
                new [] {typeof(DeviceCamera)},
                new [] {typeof(PhotoCallback)}, //12
                new [] {typeof(BarcodeRequest)},
                new [] {typeof(NativePreviewCallback), typeof(ComponentBuffer)},
                new [] {typeof(PreviewCallback)}, //15
                new [] {typeof(PhotoSaveMode)},
                new [] {typeof(ExposureMode)},
                new [] {typeof(FaceCallback)}, //18
            };
        }
        #else
        
        [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
        public class NCDocAttribute : Attribute {
            public NCDocAttribute (int id) {}
            public NCDocAttribute (int id, int sid) {}
        }
        
        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, Inherited = false, AllowMultiple = true)]
        public class NCCodeAttribute : Attribute {
            public NCCodeAttribute (int id) {}
        }
        
        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, Inherited = false, AllowMultiple = false)]
        public class NCRefAttribute : Attribute {
            public NCRefAttribute (int id) {}
        }
        #endif
    }
}