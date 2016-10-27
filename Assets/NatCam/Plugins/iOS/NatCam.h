//
//  NatCam.h
//  NatCam
//
//  Created by Yusuf on 1/9/16.
//  Copyright (c) 2016 Yusuf Olokoba
//

#import "NatCamDecls.h"
#import "NatCamExtensions.h"
#include "CMVideoSampling.h"

//NatCamU-NatCamX bridge
extern "C" {
    //Initialization
    void RegisterCallbacks (RenderCallback renderCallback, UpdateCallback updateCallback, UpdatePhotoCallback updatePhotoCallback, UpdateCodeCallback updateCodeCallback, UpdateFaceCallback updateFaceCallback);
    void InspectDevice (bool _readablePreview, bool mrDetection);
    bool HasPermissions ();
    //Learning
    void GetActiveResolution (int camera, int* width, int* height);
    void GetActivePhotoResolution (int camera, int* width, int* height);
    bool IsRearFacing (int camera);
    bool IsFlashSupported (int camera);
    bool IsTorchSupported (int camera);
    bool IsZoomSupported (int camera);
    float HorizontalFOV (int camera);
    float VerticalFOV (int camera);
    float MinExposureBias (int camera);
    float MaxExposureBias (int camera);
    //Control
    void SetResolution (int camera, int pWidth, int pHeight);
    void SetFramerate (int camera, float framerate);
    void SetPhotoResolution (int camera, int pWidth, int pHeight);
    void SetFocus (int camera, float x, float y);
    float SetExposure (int camera, float bias);
    bool SetFocusMode (int camera, int state);
    bool SetExposureMode (int camera, int state);
    bool SetFlash (int camera, int state);
    bool SetTorch (int camera, int state);
    bool SetZoom (int camera, float ratio);
    void SetPhotoSaveMode (int saveMode);
    //Running
    void PlayPreview (int cameraLocation);
    void PausePreview ();
    void TerminateOperations ();
    void SwitchCamera (int camera);
    void CaptureStill ();
    //Utility
    void PreviewBuffer (void** RGBAPtr, size_t* RGBAS);
    void ReleasePhotoBuffer ();
    void SaveToGallery (const char * imageBytes, int mode);
    void SetVerboseMode (bool verbose);
}

@interface NatCam : NSObject <AVCaptureVideoDataOutputSampleBufferDelegate, AVCaptureMetadataOutputObjectsDelegate> {
@public
    //Cameras
    NSArray* Cameras;
    int activeCamera;
    //State
@private
    bool initializedSession, inspectedDevice;
@public
    bool readablePreview, metaDetection;
    int photoSaveMode;
    //Session
    AVCaptureSession *session;
    AVCaptureStillImageOutput *photoOutput;
    AVCaptureMetadataOutput *metadataOutput;
    AVCaptureVideoDataOutput *captureOutput;
    //Utils
    GLubyte *RGBA32, *JPEG;
    size_t previewWidth, previewHeight, previewSize;
@private
    CMVideoSampling videoSampling;
    dispatch_queue_t sessionQueue;
    ALAssetsLibrary *library;
}

@end

//Singleton
NatCam* NatCamInstance ();
