/* 
*   NatCam
*   Copyright (c) 2016 Yusuf Olokoba
*/

//#define OPENCV_DEVELOPER_MODE //Uncomment this to have access to the PreviewMatrix OpenCV Matrix

#define ALLOCATE_NEW_PHOTO_TEXTURES //Comment this to make NatCam reuse the photo texture it has created instead of allocating new memory
#define ALLOCATE_NEW_FRAME_TEXTURES //Comment this to make NatCam reuse the preview frame texture it has created instead of allocating new memory

#pragma warning disable 0414, 0067

using AOT;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using Ext = NatCamU.Internals.NatCamExtensions;

#if OPENCV_DEVELOPER_MODE
using OpenCVForUnity;
#endif

namespace NatCamU {
    
    namespace Internals {
        
        [NCDoc(102)]
        public static class NatCamNativeInterface {
            
            #region --Publics--
            
            [NCDoc(6, 103)] [NCRef(14)] [NCCode(5)] public static event NativePreviewCallback OnNativePreviewUpdate;
            
            public static Texture2D Photo;
            [NCDoc(20, 134)] public static Texture2D Preview {get; private set;}
            public static Texture2D PreviewFrame {
                get {
                    if (!mReadablePreview) {
                        Ext.Warn("Cannot acquire preview frame with non-readable preview");
                        return null;
                    }
                    IntPtr RGBAPtr;
                    UIntPtr RGBAS;
                    PreviewBuffer(out RGBAPtr, out RGBAS);
                    if (RGBAPtr == IntPtr.Zero || (uint)RGBAS == 0) {
                        Ext.Warn("Unable to retrieve preview frame from native layer");
                        return null;
                    }
                    TextureFormat format = Preview.format;
                    #if UNITY_IOS
                    format = TextureFormat.BGRA32;
                    #endif
                    #if ALLOCATE_NEW_FRAME_TEXTURES
                    mPreviewFrame = new Texture2D(Preview.width, Preview.height, format, false, false);
                    #else
                    mPreviewFrame = mPreviewFrame ?? new Texture2D(Preview.width, Preview.height, format, false, false);
                    if (mPreviewFrame.width != Preview.width || mPreviewFrame.height != Preview.height) { //Realloc if size is different
                        MonoBehaviour.Destroy(mPreviewFrame);
                        mPreviewFrame = new Texture2D(Preview.width, Preview.height, format, false, false);
                    }
                    #endif
                    mPreviewFrame.LoadRawTextureData(RGBAPtr, unchecked((int)(uint)RGBAS));
                    mPreviewFrame.Apply();
                    return mPreviewFrame;
                }
            }
            #if OPENCV_DEVELOPER_MODE
            public static Mat PreviewMatrix;
            #endif
            
            public static bool isInitialized {get; private set;}
            public static int mActiveCamera = -1;
            public static bool mMetadataDetection {get; private set;}
            public static bool supportedDevice {
                get {
                    bool supportedDevice = false;
                    #if (UNITY_IOS && !UNITY_EDITOR) || (UNITY_ANDROID && !UNITY_EDITOR)
                        if (WebCamTexture.devices.Length > 0) {
                            supportedDevice = true;
                        }
                    #endif
                    return supportedDevice;
                }
            }
            public static int mPhotoSaveMode;
            public static bool isPlaying;
            [NCDoc(133)] public static bool FirstFrameReceived {get; private set;}
            #endregion
            
            
            #region --Ops--
            
            private static Texture2D mPreviewFrame;
            private static bool mReadablePreview;
            private static NatCamDispatch Dispatch;
            private static NatCamHelper listener;
            private static bool rearCamera {get {return NatCam.ActiveCamera == null || NatCam.ActiveCamera.Facing == Facing.Rear;}}
            #endregion


            #region --Events--
            private static Action PropagateStart, PropagateUpdate;
            private static PhotoCallback PropagatePhoto;
            private static BarcodeCallback PropagateBarcode;
            private static FaceCallback PropagateFace;
            #endregion
            
            
            #region --Public Ops--
            
            public static void Initialize (PreviewType previewType, bool metadataDetection) {
                mReadablePreview = previewType == PreviewType.Readable;
                mPhotoSaveMode = (int)SaveMode.DoNotSave;
                mMetadataDetection = metadataDetection;
                listener = new GameObject("NatCamHelper").AddComponent<NatCamHelper>();
                Dispatch = NatCamDispatch.Prepare(DispatchMode.Synchronous, listener);
                RegisterCallbacks(Render, Update, UpdatePhoto, UpdateCode, UpdateFace);
                InspectDeviceCameras();
                isInitialized = true;
                Ext.LogVerbose("Initialized native interface");
            }
            
            public static void Play () {
                #if UNITY_IOS
                PlayPreview(mActiveCamera);
                #elif UNITY_ANDROID
                A.Call("PlayPreview", mActiveCamera);
                #endif
            }
            
            public static void Pause () {
                #if UNITY_IOS
                PausePreview();
                #elif UNITY_ANDROID
                A.Call("PausePreview");
                #endif
                FirstFrameReceived = isPlaying = false;
            }
            
            public static void Release () {
                #if !ALLOCATE_NEW_FRAME_TEXTURES //NatCam retains ownership if we aren't reallocating
                if (Photo != null) MonoBehaviour.Destroy(Photo); Photo = null;
                #endif
                #if !ALLOCATE_NEW_PHOTO_TEXTURES //NatCam retains ownership if we aren't reallocating
                if (mPreviewFrame != null) MonoBehaviour.Destroy(mPreviewFrame); mPreviewFrame = null;
                #endif
                if (Preview != null) MonoBehaviour.Destroy(Preview); Preview = null;
                #if OPENCV_DEVELOPER_MODE
                if (PreviewMatrix != null) PreviewMatrix.release(); PreviewMatrix = null;
                #endif
                mActiveCamera = -1;
                mPhotoSaveMode = 0;
                if (listener != null) {
                    listener.WillDestroyMe();
                    MonoBehaviour.Destroy(listener.gameObject); 
                }
                listener = null;
                #if UNITY_IOS
                TerminateOperations();
                NatCamDispatch.Release(Dispatch);
                Dispatch = null;
                #elif UNITY_ANDROID
                A.Call("TerminateOperations");
                NCNA = null;
                #endif
                FirstFrameReceived = 
                isInitialized = 
                isPlaying = false;
            }
            
            public static void SwitchActiveCamera () {
                FirstFrameReceived = false;
                #if UNITY_IOS
                SwitchCamera(mActiveCamera);
                #elif UNITY_ANDROID
                A.Call("SwitchCamera", mActiveCamera);
                #endif
            }
            
            public static void CapturePhoto () {
                #if UNITY_IOS
                CaptureStill();
                #elif UNITY_ANDROID
                A.Call("CaptureStill");
                #endif
            }
            
            public static void SetSaveMode (int saveMode) {
                mPhotoSaveMode = saveMode;
                #if UNITY_IOS
                SetPhotoSaveMode(mPhotoSaveMode);
                #elif UNITY_ANDROID
                A.Call("SetPhotoSaveMode", mPhotoSaveMode);
                #endif
            }
            
            public static bool HasPermission () {
                #if UNITY_IOS && !UNITY_EDITOR
                return HasPermissions();
                #elif UNITY_ANDROID && !UNITY_EDITOR
                bool support = false;
                using (AndroidJavaClass NCNAClass = new AndroidJavaClass("com.yusufolokoba.natcam.NatCam")) {
                    if (NCNAClass != null) support = NCNAClass.CallStatic<bool>("HasPermissions");
                }
                return support;
                #else
                return true; //Unity will automatically request and get permissions for WebCamTexture, so we're safe
                #endif
            }

            public static void SaveToPhotos (Texture2D photo, int mode) {
                #if !UNITY_EDITOR
                byte[] imageData = photo.EncodeToPNG();
                string image64 = Convert.ToBase64String(imageData);
                #if UNITY_IOS
                SaveToGallery(image64, mode);
                #elif UNITY_ANDROID
                A.Call("SaveToGallery", image64, mode);
                #endif
                #endif
            }
            #endregion


            #region --Utility--

            public static void RegisterEvents (Action Start, Action Update, PhotoCallback Photo, BarcodeCallback Barcode, FaceCallback Face, Action<bool> Focus) {
                PropagateStart = Start; PropagateUpdate = Update; PropagatePhoto = Photo; PropagateBarcode = Barcode; PropagateFace = Face; listener.RegisterCallback(Focus);
            }

            public static void SetVerbose (bool state) {
                #if !UNITY_EDITOR
                #if UNITY_IOS
                SetVerboseMode(state);
                #elif UNITY_ANDROID
                if (NCNA != null) NCNA.Call("SetVerboseMode", state);
                #endif
                #endif
            }

            public static void SetApplicationFocus (bool state) {
                #if UNITY_ANDROID
                if (state) A.Call("SuspendProcess");
                else A.Call("ResumeProcess", isPlaying);
                #endif
            }
            #endregion
                        
            
            #region --Initialization--
            
            private static void InspectDeviceCameras () {
                #if UNITY_IOS
                InspectDevice(mReadablePreview, mMetadataDetection);
                #elif UNITY_ANDROID
                A.Call("InspectDevice", mReadablePreview, mMetadataDetection);
                #endif
            }
            #endregion
            
            
            #region --Native Callbacks--
            
            [MonoPInvokeCallback(typeof(RenderCallback))]
            private static void Render (int request) {
                #if UNITY_ANDROID
				if (Dispatch == null) return;
                //Dispatch on the main thread
                Dispatch.Dispatch(() => {
                    //Invocation
                    Ext.LogVerbose("Native requested callback "+request+" on render thread");
                    GL.IssuePluginEvent(NatCamNativeCallback(), request);
                    //Release
                    if (request == 3) {
                        NatCamDispatch.Release(Dispatch);
                        Dispatch = null;
                    }
                });
                #endif
            }
            
            [MonoPInvokeCallback(typeof(UpdateCallback))]
            private static void Update (IntPtr RGBA32GPUPtr, UIntPtr RGBA32Ptr, UIntPtr width, UIntPtr height, UIntPtr size) {
                //Dispatch on main thread
                Dispatch.Dispatch(() => {
                    Ext.LogVerbose("Received native update: " + string.Format("ptr<{0}>, ptr<{1}>, {2}, {3}, {4}", RGBA32GPUPtr.ToInt32().ToString(), RGBA32Ptr.ToUInt32().ToString(), width.ToString(), height.ToString(), size.ToString()));
                    //Initialization
                    Preview = Preview ?? Texture2D.CreateExternalTexture((int)width, (int)height, TextureFormat.RGBA32, false, false, RGBA32GPUPtr);
                    //Size checking
                    if (Preview.width != (int)width || Preview.height != (int)height) Preview.Resize((int)width, (int)height, Preview.format, false);
                    //Update
                    Preview.UpdateExternalTexture(RGBA32GPUPtr);
                    //Propagation
                    if (OnNativePreviewUpdate != null) {
                        OnNativePreviewUpdate(ComponentBuffer.RGBA32GPU, unchecked((UIntPtr)(ulong)(long)RGBA32GPUPtr), (int)width, (int)height, (int)size);
                        if (mReadablePreview) OnNativePreviewUpdate(ComponentBuffer.RGBA32, RGBA32Ptr, (int)width, (int)height, (int)size);
                    }
                    //OpenCV
                    #if OPENCV_DEVELOPER_MODE
                    if (mReadablePreview) {
                        PreviewMatrix = PreviewMatrix ?? new Mat(new Size((int)width, (int)height), CvType.CV_8UC4);
                        if (PreviewMatrix.cols() != (int)width || PreviewMatrix.rows() != (int)height) Imgproc.resize(PreviewMatrix, PreviewMatrix, new Size((int)width, (int)height));
                        Utils.copyToMat(unchecked((IntPtr)(long)(ulong)RGBA32Ptr), PreviewMatrix);
                        //Core.flip (PreviewMatrix, PreviewMatrix, 0); //Dev should do this themselves
                    }
                    #endif
                    //Propagation
                    if (
                        !FirstFrameReceived
                        #if UNITY_ANDROID
                        && !A.Get<bool>("orientationDirty") //In NCNA, this flag is bound to active camera state
                        #endif
                    ) {PropagateStart(); FirstFrameReceived = isPlaying = true;}
                    if (FirstFrameReceived) PropagateUpdate();
                });
            }
            
            [MonoPInvokeCallback(typeof(UpdatePhotoCallback))]
            private static void UpdatePhoto (UIntPtr JPGPtr, UIntPtr width, UIntPtr height, UIntPtr size) {
                //Dispatch on main thread
                Dispatch.Dispatch(() => {
					if (JPGPtr == UIntPtr.Zero) {
						Ext.Warn("Failed to retrieve captured photo from device");
						return;
					}
					Photo =
					#if ALLOCATE_NEW_PHOTO_TEXTURES
					new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
					#else
					Photo ?? new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
					if (Photo.width !=(int)width || Photo.height != (int)height) { //Realloc if size is different
						MonoBehaviour.Destroy(Photo);
						Photo = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
					}
					#endif
					Photo.LoadRawTextureData(unchecked((IntPtr)(long)(ulong)JPGPtr), (int)size);
					Photo.Apply();
					ReleasePhotoBuffer();
					PropagatePhoto(Photo);
                });
            }
            
            [MonoPInvokeCallback(typeof(UpdateCodeCallback))]
            private static void UpdateCode (int format, IntPtr ptr, UIntPtr size) {
                //Get the string immediately since we can't guarantee memory preservation
                if (ptr == IntPtr.Zero) {
                    Ext.Warn("Detected barcode string does not exist");
                    return;
                }
                string code = Marshal.PtrToStringUni(ptr, unchecked((int)(uint)size));
                if (code == null) {
                    Ext.Warn("Detected barcode string is null");
                    return;
                }
                //Dispatch on main thread
                Dispatch.Dispatch(() => {
                    PropagateBarcode(new Barcode(code, (BarcodeFormat)format));
                });
            }

            [MonoPInvokeCallback(typeof(UpdateFaceCallback))]
            private static void UpdateFace (int id, float xpos, float ypos, float xdim, float ydim, float roll, float yaw) {
                //Dispatch on main thread
                Dispatch.Dispatch(() => {
                    PropagateFace(new Face(id, new Vector2(xpos, ypos), new Vector2(xdim, ydim), roll, yaw));
                });
            }
            #endregion
            
            
            #region ---Native Delegate Callbacks---
        
            public delegate void RenderCallback (int request);
            public delegate void UpdateCallback (IntPtr RGBA32GPUPtr, UIntPtr RGBA32Ptr, UIntPtr width, UIntPtr height, UIntPtr size);
            public delegate void UpdatePhotoCallback (UIntPtr JPGPtr, UIntPtr width, UIntPtr height, UIntPtr size);
            public delegate void UpdateCodeCallback (int format, IntPtr ptr, UIntPtr size);
            public delegate void UpdateFaceCallback (int id, float xpos, float ypos, float xdim, float ydim, float roll, float yaw);
            #endregion

            #region --External--
            
            #if UNITY_IOS

            [DllImport("__Internal")]
            private static extern void RegisterCallbacks (RenderCallback renderCallback, UpdateCallback updateCallback, UpdatePhotoCallback updatePhotoCallback, UpdateCodeCallback updateCodeCallback, UpdateFaceCallback updateFaceCallback);
            [DllImport("__Internal")]
            private static extern void InspectDevice (bool _readablePreview, bool mrDetection);
            [DllImport("__Internal")]
            private static extern bool HasPermissions ();
            [DllImport("__Internal")]
            public static extern bool IsRearFacing (int camera);
            [DllImport("__Internal")]
            public static extern bool IsFlashSupported (int camera);
            [DllImport("__Internal")]
            public static extern bool IsTorchSupported (int camera);
            [DllImport("__Internal")]
            public static extern bool IsZoomSupported (int camera);
            [DllImport("__Internal")]
            public static extern float HorizontalFOV (int camera);
            [DllImport("__Internal")]
            public static extern float VerticalFOV (int camera);
            [DllImport("__Internal")]
            public static extern float MinExposureBias (int camera);
            [DllImport("__Internal")]
            public static extern float MaxExposureBias (int camera);
            [DllImport("__Internal")]
            public static extern void GetActiveResolution (int camera, out int width, out int height);
            [DllImport("__Internal")]
            public static extern void GetActivePhotoResolution (int camera, out int width, out int height);
            [DllImport("__Internal")]
            public static extern void SetResolution (int camera, int pWidth, int pHeight);
            [DllImport("__Internal")]
            public static extern void SetFramerate (int camera, float framerate);
            [DllImport("__Internal")]
            public static extern void SetPhotoResolution (int camera, int pWidth, int pHeight);
            [DllImport("__Internal")]
            public static extern bool SetFocus (int camera, float x, float y);
            [DllImport("__Internal")]
            public static extern float SetExposure (int camera, float bias);
            [DllImport("__Internal")]
            public static extern bool SetFocusMode (int camera, int state);
            [DllImport("__Internal")]
            public static extern bool SetExposureMode (int camera, int state);
            [DllImport("__Internal")]
            public static extern bool SetFlash (int camera, int state);
            [DllImport("__Internal")]
            public static extern bool SetTorch (int camera, int state);
            [DllImport("__Internal")]
            public static extern bool SetZoom (int camera, float ratio);
            [DllImport("__Internal")]
            private static extern void PlayPreview (int cameraLocation);
            [DllImport("__Internal")]
            public static extern void PausePreview ();
            [DllImport("__Internal")]
            private static extern void TerminateOperations ();
            [DllImport("__Internal")]
            public static extern void SwitchCamera (int camera);
            [DllImport("__Internal")]
            public static extern void CaptureStill ();
            [DllImport("__Internal")]
            private static extern void PreviewBuffer (out IntPtr RGBAPtr, out UIntPtr RGBAS);
            [DllImport("__Internal")]
            private static extern void ReleasePhotoBuffer ();
            [DllImport("__Internal")]
            private static extern void SetPhotoSaveMode (int saveMode);
            [DllImport("__Internal")]
            private static extern void SaveToGallery (string imageBytes, int mode);
            [DllImport("__Internal")]
            private static extern void SetVerboseMode (bool verbose);
            
            #elif UNITY_ANDROID

            [DllImport("NatCam")]
			private static extern IntPtr NatCamNativeCallback ();
            [DllImport("NatCam")]
            private static extern void RegisterCallbacks (RenderCallback renderCallback, UpdateCallback updateCallback, UpdatePhotoCallback updatePhotoCallback, UpdateCodeCallback updateCodeCallback, UpdateFaceCallback updateFaceCallback);
            [DllImport("NatCam")]
            private static extern void PreviewBuffer (out IntPtr RGBAPtr, out UIntPtr RGBAS);
            [DllImport("NatCam")]
            private static extern void ReleasePhotoBuffer ();
            public static AndroidJavaObject A {
                get {
                    if (NCNA == null) {
					    using (AndroidJavaClass NCNAClass = new AndroidJavaClass("com.yusufolokoba.natcam.NatCam")) {
                            if (NCNAClass != null) {
                                NCNA = NCNAClass.CallStatic<AndroidJavaObject>("instance");
                            }
					    }
                    }
                    return NCNA;
                }
            }
            private static AndroidJavaObject NCNA;

            #else
            private static void RegisterCallbacks (RenderCallback renderCallback, UpdateCallback updateCallback, UpdatePhotoCallback updatePhotoCallback, UpdateCodeCallback updateCodeCallback, UpdateFaceCallback updateFaceCallback) {}
            private static void PreviewBuffer (out IntPtr ptr, out UIntPtr size) {ptr = IntPtr.Zero; size = UIntPtr.Zero;}
            private static void SetRotation (float rot, float flip) {}
            private static void ReleasePhotoBuffer () {}
            #endif

            #endregion
        }
    }
}
#pragma warning restore 0414, 0067