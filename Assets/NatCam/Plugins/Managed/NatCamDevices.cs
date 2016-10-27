/* 
*   NatCam
*   Copyright (c) 2016 Yusuf Olokoba
*/

using UnityEngine;
using System.Collections.ObjectModel;
using System.Linq;
using NatCamU.Internals;
using Native = NatCamU.Internals.NatCamNativeInterface;
using Fallback = NatCamU.Internals.NatCamFallbackInterface;
using Ext = NatCamU.Internals.NatCamExtensions;

namespace NatCamU {
    
    ///<summary>
    ///An abstraction for device cameras
    ///</summary>
    [NCDoc(8)]
	public sealed class DeviceCamera { //Abstract data type
        
        #region --Getters---
        
        ///<summary>
        ///Get the facing of the camera
        ///</summary>
        [NCDoc(74)] [NCRef(3)] public Facing Facing {
            get {
                #if UNITY_IOS
                    if (NatCam.Interface == NatCamInterface.NativeInterface) return Native.IsRearFacing(this) ? Facing.Rear : Facing.Front;
                    else return WebCamTexture.devices[this].isFrontFacing ? Facing.Front : Facing.Rear;
                #elif UNITY_ANDROID
                    if (NatCam.Interface == NatCamInterface.NativeInterface) return Native.A.Call<bool>("IsRearFacing", (int)this) ? Facing.Rear : Facing.Front;
                    else return WebCamTexture.devices[this].isFrontFacing ? Facing.Front : Facing.Rear;
                #else
                    return WebCamTexture.devices[this].isFrontFacing ? Facing.Front : Facing.Rear;
                #endif
            }
        }
        ///<summary>
        ///Get current preview resolution of the camera
        ///</summary>
        [NCDoc(75)] public Vector2 ActiveResolution {
            get {
                #if UNITY_IOS
                    if (NatCam.Interface == NatCamInterface.NativeInterface) {
                        int width;
                        int height;
                        Native.GetActiveResolution(this, out width, out height);
                        return new Vector2 {x = width, y = height};
                    }
                    else return Fallback.Preview == null ? Vector2.zero : Fallback.Preview.deviceName == WebCamTexture.devices[this].name ? new Vector2 {x = Fallback.Preview.width, y = Fallback.Preview.height} : Vector2.zero;
                #elif UNITY_ANDROID
                    if (NatCam.Interface == NatCamInterface.NativeInterface) {
                        int width = 0, height = 0;
                        AndroidJavaObject jRet = Native.A.Call<AndroidJavaObject>("GetActiveResolution", (int)this);
                        if (jRet.GetRawObject().ToInt32() != 0) {
                            int[] res = AndroidJNIHelper.ConvertFromJNIArray<int[]>(jRet.GetRawObject());
                            width = res[0];
                            height = res[1];
                        }
                        return new Vector2 {x = width, y = height};
                    }
                    else return Fallback.Preview == null ? Vector2.zero : Fallback.Preview.deviceName == WebCamTexture.devices[this].name ? new Vector2 {x = Fallback.Preview.width, y = Fallback.Preview.height} : Vector2.zero;
                #else
                    return Fallback.Preview == null ? Vector2.zero : Fallback.Preview.deviceName == WebCamTexture.devices[this].name ? new Vector2 {x = Fallback.Preview.width, y = Fallback.Preview.height} : Vector2.zero;
                #endif
            }
        }
        ///<summary>
        ///Get current photo resolution of the camera
        ///</summary>
        [NCDoc(152)] public Vector2 ActivePhotoResolution {
            get {
                #if UNITY_IOS
                    if (NatCam.Interface == NatCamInterface.NativeInterface) {
                        int width;
                        int height;
                        Native.GetActivePhotoResolution(this, out width, out height);
                        return new Vector2 {x = width, y = height};
                    }
                    else return Fallback.Preview == null ? Vector2.zero : Fallback.Preview.deviceName == WebCamTexture.devices[this].name ? new Vector2 {x = Fallback.Preview.width, y = Fallback.Preview.height} : Vector2.zero;
                #elif UNITY_ANDROID
                    if (NatCam.Interface == NatCamInterface.NativeInterface) {
                        int width = 0, height = 0;
                        AndroidJavaObject jRet = Native.A.Call<AndroidJavaObject>("GetActivePhotoResolution", (int)this);
                        if (jRet.GetRawObject().ToInt32() != 0) {
                            int[] res = AndroidJNIHelper.ConvertFromJNIArray<int[]>(jRet.GetRawObject());
                            width = res[0];
                            height = res[1];
                        }
                        return new Vector2 {x = width, y = height};
                    }
                    else return Fallback.Preview == null ? Vector2.zero : Fallback.Preview.deviceName == WebCamTexture.devices[this].name ? new Vector2 {x = Fallback.Preview.width, y = Fallback.Preview.height} : Vector2.zero;
                #else
                    return Fallback.Preview == null ? Vector2.zero : Fallback.Preview.deviceName == WebCamTexture.devices[this].name ? new Vector2 {x = Fallback.Preview.width, y = Fallback.Preview.height} : Vector2.zero;
                #endif
            }
        }
        ///<summary>
        ///Does this camera support flash?
        ///</summary>
        [NCDoc(76)] public bool IsFlashSupported {
            get {
                #if UNITY_IOS
                    if (NatCam.Interface == NatCamInterface.NativeInterface) return Native.IsFlashSupported(this);
                    else return false;
                #elif UNITY_ANDROID
                    if (NatCam.Interface == NatCamInterface.NativeInterface) return Native.A.Call<bool>("IsFlashSupported", (int)this);
                    else return false;
                #else
                    return false;
                #endif
            }
        }
        ///<summary>
        ///Does this camera support torch?
        ///</summary>
        [NCDoc(77)] [NCCode(18)] public bool IsTorchSupported {
            get {
                #if UNITY_IOS
                    if (NatCam.Interface == NatCamInterface.NativeInterface) return Native.IsTorchSupported(this);
                    else return false;
                #elif UNITY_ANDROID
                    if (NatCam.Interface == NatCamInterface.NativeInterface) return Native.A.Call<bool>("IsTorchSupported", (int)this);
                    else return false;
                #else
                return false;
                #endif
            }
        }
        ///<summary>
        ///Does this camera support smooth zoom?
        ///</summary>
        [NCDoc(78)] public bool IsZoomSupported {
            get {
                #if UNITY_IOS
                    if (NatCam.Interface == NatCamInterface.NativeInterface) return Native.IsZoomSupported(this);
                    else return false;
                #elif UNITY_ANDROID
                    if (NatCam.Interface == NatCamInterface.NativeInterface) return Native.A.Call<bool>("IsZoomSupported", (int)this);
                    else return false;
                #else
                return false;
                #endif
            }
        }
        ///<summary>
        ///Get the camera's horizontal field-of-view
        ///</summary>
        [NCDoc(79)] public float HorizontalFOV {
            get {
                #if UNITY_IOS
                    if (NatCam.Interface == NatCamInterface.NativeInterface) return Native.HorizontalFOV(this);
                    else return 0f;
                #elif UNITY_ANDROID
                    if (NatCam.Interface == NatCamInterface.NativeInterface) return Native.A.Call<float>("HorizontalFOV", (int)this);
                    else return 0f;
                #else
                    return 0f;
                #endif
            }
        }
        ///<summary>
        ///Get the camera's vertical field-of-view
        ///</summary>
        [NCDoc(80)] public float VerticalFOV {
            get {
                #if UNITY_IOS
                    if (NatCam.Interface == NatCamInterface.NativeInterface) return Native.VerticalFOV(this);
                    else return 0f;
                #elif UNITY_ANDROID
                    if (NatCam.Interface == NatCamInterface.NativeInterface) return Native.A.Call<float>("VerticalFOV", (int)this);
                    else return 0f;
                #else
                    return 0f;
                #endif
            }
        }
        ///<summary>
        ///Get the camera's minimum exposure bias
        ///</summary>
        [NCDoc(153)] public float MinExposureBias {
            get {
                #if UNITY_IOS
                    if (NatCam.Interface == NatCamInterface.NativeInterface) return Native.MinExposureBias(this);
                    else return 0f;
                #elif UNITY_ANDROID
                    if (NatCam.Interface == NatCamInterface.NativeInterface) return (float)Native.A.Call<int>("MinExposureBias", (int)this);
                    else return 0f;
                #else
                    return 0f;
                #endif
            }
        }
        ///<summary>
        ///Get the camera's maximum exposure bias
        ///</summary>
        [NCDoc(154)] public float MaxExposureBias {
            get {
                #if UNITY_IOS
                    if (NatCam.Interface == NatCamInterface.NativeInterface) return Native.MaxExposureBias(this);
                    else return 0f;
                #elif UNITY_ANDROID
                    if (NatCam.Interface == NatCamInterface.NativeInterface) return (float)Native.A.Call<int>("MaxExposureBias", (int)this);
                    else return 0f;
                #else
                    return 0f;
                #endif
            }
        }
        #endregion
        
        
        #region ---Camera Op Properties---
        
        ///<summary>
        ///Get or set the camera's focus mode
        ///</summary>
        [NCDoc(81)] [NCRef(5)] [NCCode(17)] public FocusMode FocusMode {
            get {
                return mFocusMode;
            }
            set {
                //Check that this camera is active
                if (NatCam.ActiveCamera.index != index) Ext.Warn("This camera is not the NatCam ActiveCamera. set_FocusMode might fail");
                #if UNITY_IOS
                    if (NatCam.Interface == NatCamInterface.NativeInterface) mFocusMode = Native.SetFocusMode(this, (int)value) ? value : mFocusMode;
                    else Ext.Warn("Cannot set focus mode on fallback interface");
                #elif UNITY_ANDROID
                    if (NatCam.Interface == NatCamInterface.NativeInterface) mFocusMode = Native.A.Call<bool>("SetFocusMode", (int)this, (int)value) ? value : mFocusMode;
                    else Ext.Warn("Cannot set focus mode on fallback interface");
                #else
                    Ext.Warn("Cannot set focus mode on fallback interface");
                #endif
            }
        }
        ///<summary>
        ///Get or set the camera's exposure mode
        ///</summary>
        [NCDoc(155)] [NCRef(17)] [NCCode(20)] public ExposureMode ExposureMode {
            get {
                return mExposureMode;
            }
            set {
                //Check that this camera is active
                if (NatCam.ActiveCamera.index != index) Ext.Warn("This camera is not the NatCam ActiveCamera. set_ExposureMode might fail");
                #if UNITY_IOS
                    if (NatCam.Interface == NatCamInterface.NativeInterface) mExposureMode = Native.SetExposureMode(this, (int)value) ? value : mExposureMode;
                    else Ext.Warn("Cannot set exposure mode on fallback interface");
                #elif UNITY_ANDROID
                    if (NatCam.Interface == NatCamInterface.NativeInterface) mExposureMode = Native.A.Call<bool>("SetExposureMode", (int)this, (int)value) ? value : mExposureMode;
                    else Ext.Warn("Cannot set exposure mode on fallback interface");
                #else
                    Ext.Warn("Cannot set exposure mode on fallback interface");
                #endif
            }
        }
        ///<summary>
        ///Get or set the camera's exposure bias
        ///</summary>
        [NCDoc(24, 156)] [NCCode(22)]
        public float ExposureBias {
            get {
                return mExposureBias;
            }
            set {
                //Check that this camera is active
                if (NatCam.ActiveCamera.index != index) Ext.Warn("This camera is not the NatCam ActiveCamera. set_ExposureBias might fail");
                #if UNITY_IOS
                    if (NatCam.Interface == NatCamInterface.NativeInterface) mExposureBias = Native.SetExposure(this, value);
                    else Ext.Warn("Cannot set exposure bias on fallback interface");
                #elif UNITY_ANDROID
                    if (NatCam.Interface == NatCamInterface.NativeInterface) mExposureBias = Native.A.Call<float>("SetExposure", (int)this, Mathf.RoundToInt(value));
                    else Ext.Warn("Cannot set exposure bias on fallback interface");
                #else
                    Ext.Warn("Cannot set exposure bias on fallback interface");
                #endif
            }
        }
        ///<summary>
        ///Get or set the camera's flash mode when taking a picture
        ///</summary>
        [NCDoc(82)] [NCRef(4)] public FlashMode FlashMode {
            get {
                return mFlashMode;
            }
            set {
                #if UNITY_IOS
                    if (NatCam.Interface == NatCamInterface.NativeInterface) mFlashMode = Native.SetFlash(this, (int)value) ? value : mFlashMode;
                    else Ext.Warn("Cannot set flash mode on fallback interface");
                #elif UNITY_ANDROID
                    if (NatCam.Interface == NatCamInterface.NativeInterface) mFlashMode = Native.A.Call<bool>("SetFlash", (int)this, (int)value) ? value : mFlashMode;
                    else Ext.Warn("Cannot set flash mode on fallback interface");
                #else
                    Ext.Warn("Cannot set flash mode on fallback interface");
                #endif
            }
        }
        ///<summary>
        ///Get or set the camera's torch mode
        ///</summary>
        [NCDoc(83)] [NCRef(2)] public Switch TorchMode {
            get {
                return mTorchMode;
            }
            set {
                //Check that this camera is active
                if (NatCam.ActiveCamera.index != index) Ext.Warn("This camera is not the NatCam ActiveCamera. set_TorchMode might fail");
                #if UNITY_IOS
                    if (NatCam.Interface == NatCamInterface.NativeInterface) mTorchMode = Native.SetTorch(this, (int)value) ? value : mTorchMode;
                    else Ext.Warn("Cannot set torch mode on fallback interface");
                #elif UNITY_ANDROID
                    if (NatCam.Interface == NatCamInterface.NativeInterface) mTorchMode = Native.A.Call<bool>("SetTorch", (int)this, (int)value) ? value : mTorchMode;
                    else Ext.Warn("Cannot set torch mode on fallback interface");
                #else
                    Ext.Warn("Cannot set torch mode on fallback interface");
                #endif
            }
        }
        ///<summary>
        ///Get or set the camera's current zoom ratio
        ///</summary>
        [NCDoc(9, 84)] public float ZoomRatio {
            get {
                return mZoomRatio;
            }
            set {
                //Check that this camera is active
                if (NatCam.ActiveCamera.index != index) Ext.Warn("This camera is not the NatCam ActiveCamera. set_ZoomRatio might fail");
                float val = value;
                if (val > 1f || val < 0f) {
                    Ext.Warn("Zoom ratio must be in [0, 1] range. Clamping");
                    val = Mathf.Clamp01(val);
                }
                #if UNITY_IOS
                    if (NatCam.Interface == NatCamInterface.NativeInterface) mZoomRatio = Native.SetZoom(this, val) ? val : mZoomRatio;
                    else Ext.Warn("Cannot set zoom ratio on fallback interface");
                #elif UNITY_ANDROID
                    if (NatCam.Interface == NatCamInterface.NativeInterface) mZoomRatio = Native.A.Call<bool>("SetZoom", (int)this, val) ? val : mZoomRatio;
                    else Ext.Warn("Cannot set zoom ratio on fallback interface");
                #else
                    Ext.Warn("Cannot set zoom ratio on fallback interface");
                #endif
            }
        }
        #endregion
        
        
        #region ---Private Vars---
        private FocusMode mFocusMode = FocusMode.AutoFocus;
        private ExposureMode mExposureMode = ExposureMode.AutoExpose;
        private FlashMode mFlashMode = FlashMode.Auto;
        private Switch mTorchMode = Switch.Off;
        private float mExposureBias = 0, mZoomRatio = 0f;
        private Vector2 requestedResolution;
        private float requestedFramerate;
        private readonly int index;
        #endregion
        
        
        #region ---Public Ops---
        
        ///<summary>
        ///Set the camera's focus point of interest
        ///</summary>
        [NCDoc(8, 116)] [NCCode(21)]
        public void SetFocus (Vector2 viewportPoint) {
            //Check that this camera is active
            if (NatCam.ActiveCamera.index != index) Ext.Warn("This camera is not the NatCam ActiveCamera. SetFocus might fail");
            #if UNITY_IOS
                if (NatCam.Interface == NatCamInterface.NativeInterface) Native.SetFocus(this, viewportPoint.x, viewportPoint.y);
                else Ext.Warn("SetFocus is not supported on the fallback interface");
            #elif UNITY_ANDROID
                if (NatCam.Interface == NatCamInterface.NativeInterface) Native.A.Call("SetFocus", (int)this, viewportPoint.x, viewportPoint.y);
                else Ext.Warn("SetFocus is not supported on the fallback interface");
            #else
                Ext.Warn("SetFocus is not supported on the fallback interface");
            #endif
        }
        
        ///<summary>
        ///Set the camera's preview resolution
        ///</summary>
        [NCDoc(111)] [NCRef(8)] [NCCode(8)]
        public void SetResolution (ResolutionPreset preset) {
            int width;
            int height;
            preset.Dimensions(out width, out height);
            SetResolution(width, height);
        }
        
        ///<summary>
        ///Set the camera's preview resolution
        ///</summary>
        [NCDoc(112)] [NCCode(7)]
        public void SetResolution (int width, int height) {
            if (NatCam.IsPlaying) {
                Ext.Warn("Cannot set resolution when preview is running");
                return;
            }
            #if UNITY_IOS
                if (NatCam.Interface == NatCamInterface.NativeInterface) Native.SetResolution(this, width, height);
                else requestedResolution = new Vector2(width, height);
            #elif UNITY_ANDROID
                if (NatCam.Interface == NatCamInterface.NativeInterface) Native.A.Call("SetResolution", (int)this, width, height);
                else requestedResolution = new Vector2(width, height);
            #else
                requestedResolution = new Vector2(width, height);
            #endif
        }
        
        ///<summary>
        ///Set the camera's frame rate
        ///</summary>
        [NCDoc(10, 117)] [NCRef(9)] [NCCode(10)]
        public void SetFramerate (FrameratePreset preset) {
            SetFramerate((float)(int)(byte)preset);
        }
        
        ///<summary>
        ///Set the camera's frame rate
        ///</summary>
        [NCDoc(11, 118)] [NCCode(9)]
        public void SetFramerate (float framerate) {
            if (NatCam.IsPlaying) {
                Ext.Warn("Cannot set frame rate when preview is running");
                return;
            }
            #if UNITY_IOS
                if (NatCam.Interface == NatCamInterface.NativeInterface) Native.SetFramerate(this, framerate);
                else requestedFramerate = framerate;
            #elif UNITY_ANDROID
                if (NatCam.Interface == NatCamInterface.NativeInterface) Native.A.Call("SetFramerate", (int)this, framerate);
                else requestedFramerate = framerate;
            #else
                requestedFramerate = framerate;
            #endif
        }
        
        ///<summary>
        ///Set the camera's photo resolution
        ///</summary>
        [NCDoc(157)] [NCRef(8)] [NCCode(23)]
        public void SetPhotoResolution (ResolutionPreset preset) {
            int width;
            int height;
            preset.Dimensions(out width, out height);
            SetPhotoResolution(width, height);            
        }
        
        ///<summary>
        ///Set the camera's photo resolution
        ///</summary>
        [NCDoc(158)] [NCCode(24)]
        public void SetPhotoResolution (int width, int height) {
            if (NatCam.IsPlaying) {
                Ext.Warn("Cannot set photo resolution when preview is running");
                return;
            }
            #if UNITY_IOS
                if (NatCam.Interface == NatCamInterface.NativeInterface) Native.SetPhotoResolution(this, width, height);
                else Ext.Warn("Cannot set photo resolution on fallback interface");
            #elif UNITY_ANDROID
                if (NatCam.Interface == NatCamInterface.NativeInterface) Native.A.Call("SetPhotoResolution", (int)this, width, height);
                else Ext.Warn("Cannot set photo resolution on fallback interface");
            #else
                Ext.Warn("Cannot set photo resolution on fallback interface");
            #endif
        }
        #endregion
        
        
        #region ---Conversion and Extraction---
        
		public static implicit operator int (DeviceCamera cam) {
			return cam == null ? -1 : cam.index;
		}
        public static implicit operator DeviceCamera (int camIndex) {
            return Cameras == null ? null : Cameras.Count == 0 ? null : Cameras.Where(c => c.index == camIndex).FirstOrDefault();
        }
        public static explicit operator Vector2 (DeviceCamera cam) { //Hidden in plain sight >:)
            return cam.requestedResolution;
        }
        public static explicit operator float (DeviceCamera cam) {
            return cam.requestedFramerate;
        }
        #endregion
        
        
        #region ---Intializers---
        private DeviceCamera (int i) {
            index = i;
        }
        static DeviceCamera () {
            int cameraCount = WebCamTexture.devices.Length;
            DeviceCamera[] cameras = new DeviceCamera[cameraCount];
            for (int i = 0; i < cameraCount; i++) {
                cameras[i] = new DeviceCamera(i);
            }
            Cameras = new ReadOnlyCollection<DeviceCamera>(cameras);
            RearCamera = Cameras.FirstOrDefault(c => c.Facing == Facing.Rear);
            FrontCamera = Cameras.FirstOrDefault(c => c.Facing == Facing.Front);
        }
        #endregion
        
        
		#region ---Statics---
        [NCDoc(61)] public static readonly DeviceCamera FrontCamera;
		[NCDoc(62)] public static readonly DeviceCamera RearCamera;
        [NCDoc(110)] [NCCode(18)] public static readonly ReadOnlyCollection<DeviceCamera> Cameras; //You shall not touch!
        #endregion
        
        #region ---Utility---
        ///<summary>
        ///Internal. Do not use.
        ///</summary>
        public static void Reset () {
            for (int i = 0, len = Cameras.Count; i < len; i++) {
                DeviceCamera cam = Cameras[i];
                cam.mFocusMode = FocusMode.AutoFocus;
                cam.mFlashMode = FlashMode.Auto;
                cam.mTorchMode = Switch.Off;
                cam.mZoomRatio = 0f;
                cam.requestedResolution = Vector2.zero;
                cam.requestedFramerate = (int)FrameratePreset.Default;
            }
        }
        #endregion
	}
}