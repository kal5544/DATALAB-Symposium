/* 
*   NatCam
*   Copyright (c) 2016 Yusuf Olokoba
*/

//#define OPENCV_DEVELOPER_MODE //Uncomment this to have access to the PreviewMatrix OpenCV Matrix

using UnityEngine;
using System;
using NatCamU.Internals;
using Native = NatCamU.Internals.NatCamNativeInterface;
using Fallback = NatCamU.Internals.NatCamFallbackInterface;
using Ext = NatCamU.Internals.NatCamExtensions;

#if OPENCV_DEVELOPER_MODE
using OpenCVForUnity;
#endif

namespace NatCamU {
    
    ///<summary>
    ///Central class for controlling NatCam
    ///</summary>
    [NCDoc(7)]
    public static class NatCam {
        
        #region ---Events---
        ///<summary>
        ///Event fired when NatCam receives captured photos
        ///</summary>
        [NCDoc(0)] public static event PhotoCallback OnPhotoCapture;
        ///<summary>
        ///Event fired when NatCam detects barcodes
        ///</summary>
        [NCDoc(1)] public static event BarcodeCallback OnBarcodeDetect;
        ///<summary>
        ///Event fired when NatCam detects faces
        ///</summary>
        [NCDoc(159)] public static event FaceCallback OnFaceDetect;
        ///<summary>
        ///Event fired when the preview starts
        ///</summary>
        [NCDoc(2)] public static event PreviewCallback OnPreviewStart;
        ///<summary>
        ///Event fired on each camera preview update
        ///</summary>
        [NCDoc(3)] public static event PreviewCallback OnPreviewUpdate;
        #endregion
        
        
        #region ---Preview Vars---
        ///<summary>
        ///The camera preview as a Texture
        ///</summary>
        [NCDoc(4)] public static Texture Preview {
            get {
                return Interface == NatCamInterface.NativeInterface ? (Texture)Native.Preview : (Texture)Fallback.Preview;
            }
        }
        ///<summary>
        ///The current camera preview frame
        ///</summary>
        [NCDoc(0, 5)] public static Texture2D PreviewFrame {
            get {
                return Interface == NatCamInterface.NativeInterface ? Native.PreviewFrame : Fallback.PreviewFrame;
            }
        }
        #if OPENCV_DEVELOPER_MODE
        ///<summary>
        ///The camera preview as an OpenCV Matrix
        ///</summary>
        [NCDoc(1, 6)] [NCCode(6)] public static Mat PreviewMatrix {
            get {
                return Interface == NatCamInterface.NativeInterface ? Native.PreviewMatrix : Fallback.PreviewMatrix;
            }
        }
        #endif
        #endregion
        
        
        #region ---Public Op Vars---
        ///<summary>
        ///The backing interface NatCam was initialized with
        ///</summary>
        [NCDoc(129)] public static NatCamInterface Interface {get; private set;}
        ///<summary>
        ///Get or set NatCam's verbose mode
        ///</summary>
        [NCDoc(160)] [NCRef(2)] public static Switch Verbose { //Calligraphy bug, no support for setter-only properties
            get {
                return Ext.Verbose ? Switch.On : Switch.Off;
            }
            set {
                Ext.Verbose = value == Switch.On;
                Native.SetVerbose(value == Switch.On);
            }
        }
        ///<summary>
        ///Get or set the active camera.
        ///</summary>
        [NCDoc(12, 119)] [NCRef(11)] [NCCode(4)] public static DeviceCamera ActiveCamera {
            get {
                return Interface == NatCamInterface.NativeInterface ? Native.mActiveCamera : Fallback.mActiveCamera;
            }
            set {
                if (value == ActiveCamera) {
                    Ext.Warn("Camera is already active");
                    return;
                }
                if (Interface ==  NatCamInterface.NativeInterface) {
                    Native.mActiveCamera = value;
                    if (IsPlaying) Native.SwitchActiveCamera();
                    #if UNITY_ANDROID
                    else Native.A.Call("SetSessionCamera", (int)value);
                    #endif
                }
                else {
                    Fallback.mActiveCamera = value;
                    if (IsPlaying) Fallback.SwitchActiveCamera();
                }
            }
        }
        ///<summary>
        ///Get or set the photo save mode for subsequent captured photos
        ///</summary>
        [NCDoc(18, 130)] [NCRef(16)] public static SaveMode PhotoSaveMode {
            get {
                if (Interface == NatCamInterface.FallbackInterface) {
                    Ext.Warn("Photo save mode is not implemented on the fallback interface");
                    return SaveMode.DoNotSave;
                }
                return (SaveMode)Native.mPhotoSaveMode;
            }
            set {
                if (Interface == NatCamInterface.NativeInterface) Native.SetSaveMode((int)value);
                else Ext.Warn("Cannot set photo save mode on fallback interface");
            }
        }
        ///<summary>
        ///Is this device or platform supporrted?
        ///</summary>
        [NCDoc(122)] public static bool IsSupportedDevice {
            get {
                return Native.supportedDevice;
            }
        }
        ///<summary>
        ///Is NatCam initialized?
        ///</summary>
        [NCDoc(178)] public static bool IsInitialized { //Developers pass this assertion themselves
            get {
                return Native.isInitialized || Fallback.isInitialized;
            }
        }
        ///<summary>
        ///Is the preview running?
        ///</summary>
        [NCDoc(123)] public static bool IsPlaying {
            get {
                return Interface == NatCamInterface.NativeInterface ? Native.isPlaying : Fallback.Preview != null ? Fallback.Preview.isPlaying : false;
            }
        }
        ///<summary>
        ///Does the app has permissions to use the camera? This must be true for NatCam to work properly.
        ///</summary>
        [NCDoc(23, 151)] [NCCode(19)] public static bool HasPermissions {
            get {
                return Native.HasPermission();
            }
        }
        #endregion
        
        
        #region ---Public Ops---
        
        ///<summary>
        ///Execute the supplied callbacks when the camera preview has started
        ///</summary>
        ///<param name="callbacks">The callbacks that should be invoked when the preview starts</param>
        [NCDoc(19, 131)] [NCRef(15)] [NCCode(16)]
        public static void ExecuteOnPreviewStart (params PreviewCallback[] callbacks) {
            //Check that the OnPreviewStart event hasn't been fired
            if ((Interface == NatCamInterface.NativeInterface && !Native.FirstFrameReceived) || (Interface == NatCamInterface.FallbackInterface && !Fallback.FirstFrameReceived)) {
                for (int i = 0; i < callbacks.Length; i++) {
                    PreviewCallback callback = callbacks[i], wrapper = null;
                    wrapper = delegate () {
                        if (callback != null) callback();
                        OnPreviewStart -= wrapper;
                    };
                    OnPreviewStart += wrapper;
                }
            }
            //The preview has already started
            else for (int i = 0; i < callbacks.Length; i++) if (callbacks[i] != null) callbacks[i]();
        }
        
        ///<summary>
        ///Initialize NatCam
        ///</summary>
        ///<param name="NatCamInterface">The backing interface that NatCam should use. Native allows the full capabilities of NatCam to be used</param>
        ///<param name="PreviewType">Whether the preview should be readable or not. Use readable for OpenCV and getting pixel data</param>
        ///<param name="MetadataDetection">Whether NatCam should prepare for face and barcode detection</param>
        [NCDoc(17, 128)] [NCRef(10)] [NCCode(0)] [NCCode(2)]
        public static void Initialize (NatCamInterface NatCamInterface = NatCamInterface.NativeInterface, PreviewType PreviewType = PreviewType.NonReadable, Switch MetadataDetection = Switch.Off) {
            Interface = NatCamInterface;
            if (Interface == NatCamInterface.NativeInterface && !IsSupportedDevice) {
                Ext.Warn("Running on an unsupported platform or a device without cameras. Falling back to Fallback");
                Interface = NatCamInterface.FallbackInterface;
            }
            switch (Interface) {
                case NatCamInterface.NativeInterface:
                    Native.Initialize(PreviewType, MetadataDetection == Switch.On);
                    Native.RegisterEvents(PropagateStart, PropagateUpdate, PropagatePhoto, PropagateBarcode, PropagateFace, SetApplicationFocus);
                break;
                case NatCamInterface.FallbackInterface:
                    Fallback.Initialize(MetadataDetection == Switch.On);
                    Fallback.RegisterEvents(PropagateStart, PropagateUpdate, PropagatePhoto, PropagateBarcode, PropagateFace, SetApplicationFocus);
                break;
                default:
                    goto case NatCamInterface.FallbackInterface;
            }
        }
        
        ///<summary>
        ///Start the camera preview
        ///</summary>
        ///<param name="camera">Optional. Camera that the preview should start from</param>
        [NCDoc(113)] [NCRef(11)]
        public static void Play (DeviceCamera camera = null) {
            switch (Interface) {
                case NatCamInterface.NativeInterface:
                    if (camera != null) Native.mActiveCamera = camera;
                    Native.Play();
                break;
                case NatCamInterface.FallbackInterface:
                    if (camera != null) Fallback.mActiveCamera = camera;
                    Fallback.Play();
                break;
            }
        }
        
        ///<summary>
        ///Pause the camera preview
        ///</summary>
        [NCDoc(114)]
        public static void Pause () {
            switch (Interface) {
                case NatCamInterface.NativeInterface:
                    Native.Pause();
                break;
                case NatCamInterface.FallbackInterface:
                    Fallback.Pause();
                break;
            }
        }
        
        ///<summary>
        ///Stop NatCam and release all resources
        ///</summary>
        [NCDoc(115)] [NCCode(11)]
        public static void Release () {
            DeviceCamera.Reset();
            switch (Interface) {
                case NatCamInterface.NativeInterface:
                    Native.Release();
                break;
                case NatCamInterface.FallbackInterface:
                    Fallback.Release();
                break;
            }
        }
        
        ///<summary>
        ///Capture a photo
        ///</summary>
        ///<param name="callbacks">The callbacks that should be invoked when NatCam receives the captured photo</param>
        [NCDoc(15, 126)] [NCRef(12)] [NCCode(13)] [NCCode(14)] [NCCode(15)]
        public static void CapturePhoto (params PhotoCallback[] callbacks) {
            if (!IsPlaying) {
                Ext.Warn("Cannot capture photo when session is not running");
                return;
            }
            if (callbacks.Length == 0 && OnPhotoCapture == null) {
                Ext.Warn("Cannot capture photo because there is no callback subscribed");
                return;
            }
            else if (callbacks.Length > 0) {
                for (int i = 0; i < callbacks.Length; i++) {
                    PhotoCallback callback = callbacks[i];
                    PhotoCallback captureWrapper = null;
                    captureWrapper = (Texture2D photo) => {
                        if (callback != null) callback (photo);
                        OnPhotoCapture -= captureWrapper;
                    };
                    OnPhotoCapture += captureWrapper;
                }
            }
            if (Interface == NatCamInterface.NativeInterface) Native.CapturePhoto();
            else Fallback.CapturePhoto();
        }
        
        ///<summary>
        ///Request to be notified of a barcode
        ///</summary>
        ///<param name="requests">Barcode requests that should be addressed</param>
        [NCDoc(16, 127)] [NCRef(13)] [NCCode(12)]
        public static void RequestBarcode (params BarcodeRequest[] requests) {
            if ((Interface == NatCamInterface.NativeInterface && !Native.mMetadataDetection) || (Interface == NatCamInterface.FallbackInterface && !Fallback.mMetadataDetection)) {
                Ext.Warn("NatCam was initialized without preparing for metadata detection. Ignoring call");
                return;
            }
            if (requests.Length == 0) {
                Ext.Warn("Cannot request barcode with no requests supplied");
                return;
            }
            for (int i = 0; i < requests.Length; i++) {
                BarcodeRequest req = requests[i];
                BarcodeCallback temp = null;
                temp  = delegate (Barcode code) {
                    if ((code.format & req.format) > 0) {
                        if (req.callback != null) req.callback (code); //Null checking saves one, kills none
                        if (req.detectOnce) OnBarcodeDetect -= temp;
                    }
                };
                OnBarcodeDetect += temp;
            }
        }

        ///<summary>
        ///Request to be notified of a face
        ///</summary>
        ///<param name="callbacks">Callbacks that should be invoked when NatCam detects a face</param>
        [NCDoc(25, 161)] [NCRef(18)] [NCCode(25)]
        public static void RequestFace (params FaceCallback[] callbacks) {
            if (Interface == NatCamInterface.FallbackInterface) {
                Ext.Warn("Cannot request face detection on fallback interface");
                return;
            }
            if (!Native.mMetadataDetection) {
                Ext.Warn("NatCam was initialized without preparing for metadata detection. Ignoring call");
                return;
            }
            if (callbacks.Length == 0) {
                Ext.Warn("Cannot request faces with no callbacks supplied");
                return;
            }
            for (int i = 0; i < callbacks.Length; i++) if (callbacks[i] != null) OnFaceDetect += callbacks[i];
        }

        ///<summary>
        ///Save a photo to the camera roll
        ///</summary>
        ///<param name="photo">The photo to be saved</param>
        ///<param name="saveMode">The photo save mode</param>
        [NCDoc(26, 162)] [NCRef(16)] [NCCode(26)]
		public static void SaveToPhotos (Texture2D photo, SaveMode saveMode = SaveMode.SaveToPhotoGallery) { //DEPLOY
            if (Interface == NatCamInterface.FallbackInterface) {
                Ext.Warn("Cannot save photo on fallback interface");
                return;
            }
            if (saveMode == SaveMode.DoNotSave) {
                Ext.Warn("Don't ask me to save a photo then say I shouldn't :/"); //Sassy
                return;
            }
            Native.SaveToPhotos(photo, (int)saveMode);
        }
        #endregion
        
        
        #region ---Event Propagation---
        
        private static Action PropagateStart = () => {
            if (OnPreviewStart != null) OnPreviewStart();
        };
        private static Action PropagateUpdate = () => {
            if (OnPreviewUpdate != null) OnPreviewUpdate();
        };
        private static PhotoCallback PropagatePhoto = photo => {
            if (OnPhotoCapture != null) OnPhotoCapture(photo);
        };
        private static BarcodeCallback PropagateBarcode = barcode => {
            if (OnBarcodeDetect != null) OnBarcodeDetect(barcode);
        };
        private static FaceCallback PropagateFace = face => {
            if (OnFaceDetect != null) OnFaceDetect(face);
        };
        private static Action<bool> SetApplicationFocus = focus => {
            if (Interface == NatCamInterface.NativeInterface) Native.SetApplicationFocus(focus);
        };
        #endregion
    }
}