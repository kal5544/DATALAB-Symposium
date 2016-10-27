/* 
*   NatCam
*   Copyright (c) 2016 Yusuf Olokoba
*/

//#define OPENCV_DEVELOPER_MODE //Uncomment this to have access to the PreviewMatrix OpenCV Matrix

#define ALLOCATE_NEW_PHOTO_TEXTURES //Comment this to make NatCam reuse the photo texture it has created instead of allocating new memory
#define ALLOCATE_NEW_FRAME_TEXTURES //Comment this to make NatCam reuse the preview frame texture it has created instead of allocating new memory

#pragma warning disable 0414

using UnityEngine;
using System;
using Ext = NatCamU.Internals.NatCamExtensions;

#if OPENCV_DEVELOPER_MODE
using OpenCVForUnity;
#endif

namespace NatCamU {
    
    namespace Internals {
        
        [NCDoc(109)]
        public static class NatCamFallbackInterface {
            
            #region --Publics--

            public static Texture2D Photo;
            [NCDoc(125)] public static WebCamTexture Preview {get; private set;}
            public static Texture2D PreviewFrame {
                get {
                    if (Preview == null || PreviewBuffer == null) {
                        Ext.Warn("Cannot retrieve preview frame because preview has not started. Returning null.");
                        return null;
                    }
                    #if ALLOCATE_NEW_FRAME_TEXTURES
                    mPreviewFrame = new Texture2D(Preview.width, Preview.height);
                    #else
                    if (mPreviewFrame == null) mPreviewFrame = new Texture2D(Preview.width, Preview.height);
                    else if (mPreviewFrame.width != Preview.width || mPreviewFrame.height != Preview.height) { //Realloc if size is different
                        MonoBehaviour.Destroy(mPreviewFrame);
                        mPreviewFrame = new Texture2D(Preview.width, Preview.height);
                    }
                    #endif
                    //Check buffer
                    CheckBuffer();
                    Preview.GetPixels32(PreviewBuffer);
                    mPreviewFrame.SetPixels32(PreviewBuffer);
                    mPreviewFrame.Apply();
                    return mPreviewFrame;
                }
            }
            #if OPENCV_DEVELOPER_MODE
            public static Mat PreviewMatrix;
            #endif
            public static bool isInitialized {get; private set;}
            public static int mActiveCamera = -1;
            [NCDoc(132)] public static Color32[] PreviewBuffer {get; private set;}
            [NCDoc(133)] public static bool FirstFrameReceived {get; private set;}
            public static bool mMetadataDetection {get; private set;}
            public static bool supportedDevice {
                get {
                    return WebCamTexture.devices.Length > 0;
                }
            }
            #endregion

            //Privates
            private static Texture2D mPreviewFrame;
            private static volatile int width, height;
            private static NatCamDispatch Dispatch, MetadataDispatch;
            private static Color32[] metadataBuffer;
            private static readonly object bufferLock = new object();


            #region --Events--
            private static Action PropagateStart, PropagateUpdate;
            private static PhotoCallback PropagatePhoto;
            private static BarcodeCallback PropagateBarcode;
            private static FaceCallback PropagateFace; //Maybe one day
            #endregion
            
            
            #region ---Public Ops---
            
            public static void Initialize (bool metadataDetection) {
                if (!supportedDevice) return;
                mMetadataDetection = metadataDetection;
                Dispatch = NatCamDispatch.Prepare(DispatchMode.Synchronous);
                Dispatch.DispatchContinuous(Update);
                if (mMetadataDetection) {
                    MetadataDispatch = NatCamDispatch.Prepare(DispatchMode.Asynchronous);
                    MetadataDispatch.DispatchContinuous(UpdateMetadata);
                }
                isInitialized = true;
                Ext.LogVerbose("Initialized fallback interface");
            }

            public static void RegisterEvents (Action Start, Action Update, PhotoCallback Photo, BarcodeCallback Barcode, FaceCallback Face, Action<bool> Focus) {
                PropagateStart = Start; PropagateUpdate = Update; PropagatePhoto = Photo; PropagateBarcode = Barcode;
            }
            
            public static void Play () {
                if (!supportedDevice) {
                    Ext.Warn("Current device has no cameras");
                    return;
                }
                if (Preview == null) { //First Play()
                    if (((Vector2)(DeviceCamera)mActiveCamera).x == 0) { //Default
                        Preview = new WebCamTexture(WebCamTexture.devices[mActiveCamera].name);
                    }
                    else { //Resolution set
                        Vector2 resolution = (Vector2)(DeviceCamera)mActiveCamera;
                        float frameRate = (float)(DeviceCamera)mActiveCamera;
                        if ((int)frameRate == 0) {
                            Preview = new WebCamTexture(WebCamTexture.devices[mActiveCamera].name, (int)resolution.x, (int)resolution.y);
                        }
                        else {
                            Preview = new WebCamTexture(WebCamTexture.devices[mActiveCamera].name, (int)resolution.x, (int)resolution.y, (int)frameRate);
                        }
                    }
                }
                Preview.Play();
            }
            
            public static void Pause () {
                if (!supportedDevice) {
                    Ext.Warn("Current device has no cameras");
                    return;
                }
                Preview.Pause();
            }
            
            public static void Release () {
                #if !ALLOCATE_NEW_FRAME_TEXTURES //NatCam retains ownership if we aren't reallocating
                if (Photo != null) {MonoBehaviour.Destroy(Photo); Photo = null;}
                #endif
                #if !ALLOCATE_NEW_PHOTO_TEXTURES //NatCam retains ownership if we aren't reallocating
                if (mPreviewFrame != null) {MonoBehaviour.Destroy(mPreviewFrame); mPreviewFrame = null;}
                #endif
                if (Preview.isPlaying) Preview.Stop(); MonoBehaviour.Destroy(Preview); Preview = null;
                #if OPENCV_DEVELOPER_MODE
                if (PreviewMatrix != null) PreviewMatrix.release(); PreviewMatrix = null;
                #endif
                mActiveCamera = -1;
                PreviewBuffer = metadataBuffer = null;
                NatCamDispatch.Release(Dispatch);
                if (mMetadataDetection) NatCamDispatch.Release(MetadataDispatch);
                Dispatch = MetadataDispatch = null;
                FirstFrameReceived = 
                isInitialized = 
                mMetadataDetection = false;
            }
            
            public static void SwitchActiveCamera () {
                if (!supportedDevice) {
                    Ext.Warn("Current device has no cameras");
                    return;
                }
                FirstFrameReceived = false;
                Preview.Stop();
                Preview.deviceName = WebCamTexture.devices[mActiveCamera].name;
                Preview.Play();
            }
            
            public static void CapturePhoto () {
                if (!supportedDevice) {
                    Ext.Warn("Current device has no cameras");
                    return;
                }
                if (Preview == null) return;
                if (!Preview.isPlaying) return;
                #if ALLOCATE_NEW_PHOTO_TEXTURES
                Photo = new Texture2D(Preview.width, Preview.height);
                #else
                if (Photo == null) Photo = new Texture2D(Preview.width, Preview.height);
                #endif
                lock (bufferLock) {
                    Preview.GetPixels32(PreviewBuffer);
                    Photo.SetPixels32(PreviewBuffer);
                }
                Photo.Apply();
                PropagatePhoto(Photo);
            }
            
            private static void Update () {
                if (Preview == null || !Preview.isPlaying) return;
                if (!FirstFrameReceived && Preview.didUpdateThisFrame) {PropagateStart(); FirstFrameReceived = true;}
                CheckBuffer();
                //OpenCV
                #if OPENCV_DEVELOPER_MODE
                PreviewMatrix = PreviewMatrix ?? new Mat(new Size(Preview.width, Preview.height), CvType.CV_8UC4);
                if (PreviewMatrix.cols() != width || PreviewMatrix.rows() != height) Imgproc.resize(PreviewMatrix, PreviewMatrix, new Size(width, height));
                Utils.webCamTextureToMat(Preview, PreviewMatrix, PreviewBuffer);
                #endif
                //Update
                PropagateUpdate();
            }

			private static void UpdateMetadata () {
				if (PreviewBuffer == null || !FirstFrameReceived) return;
				metadataBuffer = metadataBuffer ?? new Color32[width * height];
				if (metadataBuffer.Length != width * height) metadataBuffer = new Color32[width * height];
				lock (bufferLock) Array.Copy(PreviewBuffer, metadataBuffer, metadataBuffer.Length); //We don't want to lock for too long
				NatCamMetadata.Decode(metadataBuffer, width, height, Dispatch, PropagateBarcode);
			}
            
            private static void CheckBuffer () {
                width = Preview.width; height = Preview.height;
                if (PreviewBuffer == null) PreviewBuffer = new Color32[width * height];
                if (PreviewBuffer.Length != width * height) PreviewBuffer = new Color32[width * height];
                if (mMetadataDetection) lock (bufferLock) Preview.GetPixels32(PreviewBuffer);
            }
            #endregion
        }
    }
}
#pragma warning restore 0414