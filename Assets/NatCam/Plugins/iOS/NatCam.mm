//
//  NatCam.mm
//  NatCam Control Pipeline
//
//  Created by Yusuf on 1/19/16.
//  Copyright (c) 2016 Yusuf Olokoba
//

#import "NatCam.h"

#pragma mark --Callbacks--
RenderCallback _renderCallback;
UpdateCallback _updateCallback;
UpdatePhotoCallback _updatePhotoCallback;
UpdateCodeCallback _updateCodeCallback;
UpdateFaceCallback _updateFaceCallback;

#pragma mark  --NatCamX Singleton--
static NatCam* _sharedInstance = nil;
NatCam* NatCamInstance () {
    if( !_sharedInstance ) {
        _sharedInstance = [[NatCam alloc] init];
    }
    return _sharedInstance;
}


@implementation NatCam

#pragma mark --Top Level Initialization--

void RegisterCallbacks (RenderCallback renderCallback, UpdateCallback updateCallback, UpdatePhotoCallback updatePhotoCallback, UpdateCodeCallback updateCodeCallback, UpdateFaceCallback updateFaceCallback) {
    _renderCallback = renderCallback;
    _updateCallback = updateCallback;
    _updatePhotoCallback = updatePhotoCallback;
    _updateCodeCallback = updateCodeCallback;
    _updateFaceCallback = updateFaceCallback;
    Log(@"NatCam Native: Registered Callbacks");
}

void InspectDevice (bool _readablePreview, bool mrDetection) {
    NatCamInstance()->readablePreview = _readablePreview;
    NatCamInstance()->metaDetection = mrDetection;
    [NatCamInstance() InspectDevice];
}

bool HasPermissions () {
    return [AVCaptureDevice authorizationStatusForMediaType:AVMediaTypeVideo] == AVAuthorizationStatusAuthorized;
}

#pragma mark --Learning--

void GetActiveResolution (int camera, int* width, int* height) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    AVCaptureDevice* device = (AVCaptureDevice*)[NatCamInstance()->Cameras objectAtIndex:camera];
    CMVideoDimensions dimensions = CMVideoFormatDescriptionGetDimensions(device.activeFormat.formatDescription);
    *width = dimensions.width;
    *height = dimensions.height;
}

void GetActivePhotoResolution (int camera, int* width, int* height) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    AVCaptureDevice* device = (AVCaptureDevice*)[NatCamInstance()->Cameras objectAtIndex:camera];
    CMVideoDimensions dimensions = device.activeFormat.highResolutionStillImageDimensions;
    *width = dimensions.width;
    *height = dimensions.height;
}

bool IsRearFacing (int camera) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    return ((AVCaptureDevice*)[NatCamInstance()->Cameras objectAtIndex:camera]).position == AVCaptureDevicePositionBack;
}

bool IsFlashSupported (int camera) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    return ((AVCaptureDevice*)[NatCamInstance()->Cameras objectAtIndex:camera]).flashAvailable;
}

bool IsTorchSupported (int camera) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    return ((AVCaptureDevice*)[NatCamInstance()->Cameras objectAtIndex:camera]).torchAvailable;
}

bool IsZoomSupported (int camera) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    return ((AVCaptureDevice*)[NatCamInstance()->Cameras objectAtIndex:camera]).activeFormat.videoMaxZoomFactor > 1;
}

float HorizontalFOV (int camera) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    return ((AVCaptureDevice*)[NatCamInstance()->Cameras objectAtIndex:camera]).activeFormat.videoFieldOfView;
}

float VerticalFOV (int camera) {
    int w, h;
    GetActiveResolution(camera, &w, &h);
    return HorizontalFOV(camera) * (float)h / (float)w;
}

float MinExposureBias (int camera) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    return ((AVCaptureDevice*)[NatCamInstance()->Cameras objectAtIndex:camera]).minExposureTargetBias;
}

float MaxExposureBias (int camera) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    return ((AVCaptureDevice*)[NatCamInstance()->Cameras objectAtIndex:camera]).maxExposureTargetBias;
}

#pragma mark --Control--

void SetResolution (int camera, int pWidth, int pHeight) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    AVCaptureDevice* device = (AVCaptureDevice*)[NatCamInstance()->Cameras objectAtIndex:camera];
    AVCaptureDeviceFormat* bestFormat = FormatWithBestResolution(device, pWidth, pHeight);
    if ([device lockForConfiguration:NULL] == YES) {
        device.activeFormat = bestFormat;
        LogVerbose([NSString stringWithFormat:@"Changed camera format for resolution %ix%i", pWidth, pHeight]);
        [device unlockForConfiguration];
    }
}

void SetFramerate (int camera, float framerate) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    AVCaptureDevice* device = (AVCaptureDevice*)[NatCamInstance()->Cameras objectAtIndex:camera];
    AVFrameRateRange* bestRange = ClosestFramerate(device, framerate);
    if ([device lockForConfiguration:NULL] == YES) {
        device.activeVideoMinFrameDuration = bestRange.minFrameDuration;
        device.activeVideoMaxFrameDuration = bestRange.maxFrameDuration;
        LogVerbose([NSString stringWithFormat:@"Changed camera framerate to %f", bestRange.maxFrameRate]);
        [device unlockForConfiguration];
    }
}

void SetPhotoResolution (int camera, int pWidth, int pHeight) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    AVCaptureDevice* device = (AVCaptureDevice*)[NatCamInstance()->Cameras objectAtIndex:camera];
    AVCaptureDeviceFormat* bestFormat = FormatWithPhotoResolution(device, pWidth, pHeight);
    if ([device lockForConfiguration:NULL] == YES) {
        device.activeFormat = bestFormat;
        LogVerbose([NSString stringWithFormat:@"Changed camera format for photo resolution %ix%i", pWidth, pHeight]);
        [device unlockForConfiguration];
    }
}

void SetFocus (int camera, float x, float y) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    AVCaptureDevice* device = [NatCamInstance()->Cameras objectAtIndex:camera];
    LogVerbose([NSString stringWithFormat:@"Attempting to focus camera %i at (%f, %f)", camera, x, y]);
    CGPoint focusPoint = CGPointMake(x, y);
    if ([device lockForConfiguration:NULL] == YES) {
        if ([device isFocusPointOfInterestSupported]) [device setFocusPointOfInterest:focusPoint];
        if ([device isExposurePointOfInterestSupported]) [device setExposurePointOfInterest:focusPoint];
        if ([device isFocusModeSupported:AVCaptureFocusModeAutoFocus]) [device setFocusMode:AVCaptureFocusModeAutoFocus];
        LogVerbose(@"Set focus point");
        [device unlockForConfiguration];
    }
}

float SetExposure (int camera, float bias) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    AVCaptureDevice* device = [NatCamInstance()->Cameras objectAtIndex:camera];
    LogVerbose([NSString stringWithFormat:@"Attempting to expose camera %i to %fEV", camera, bias]);
    if ([device lockForConfiguration:NULL] == YES) {
        [device setExposureTargetBias:bias completionHandler:nil];
        LogVerbose(@"Set exposure bias");
        [device unlockForConfiguration];
        return bias;
    }
    else return [device exposureTargetBias];
}

bool SetFocusMode (int camera, int state) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    AVCaptureDevice* device = [NatCamInstance()->Cameras objectAtIndex:camera];
    bool ret = false;
    if (state % 2 == 0) [NatCamInstance() RegisterForAutofocus:device];
    else [NatCamInstance() UnregisterFromAutofocus:device];
    AVCaptureFocusMode focusMode = state % 2 == 0 ? AVCaptureFocusModeContinuousAutoFocus : AVCaptureFocusModeLocked;
    if ([device lockForConfiguration:NULL] == YES ) {
        if ([device isFocusModeSupported:focusMode]) {
            [device setFocusMode:focusMode];
            ret = true;
            LogVerbose([NSString stringWithFormat:@"Set focus mode to %i", state]);
        }
        [device unlockForConfiguration];
    }
    return ret;
}

bool SetExposureMode (int camera, int state) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    AVCaptureDevice* device = [NatCamInstance()->Cameras objectAtIndex:camera];
    bool ret = false;
    AVCaptureExposureMode exposureMode = state == 1 ? AVCaptureExposureModeLocked : AVCaptureExposureModeAutoExpose;
    if ([device lockForConfiguration:NULL] == YES ) {
        if ([device isExposureModeSupported:exposureMode]) {
            [device setExposureMode:exposureMode];
            ret = true;
            LogVerbose([NSString stringWithFormat:@"Set exposure mode to %i", state]);
        }
        [device unlockForConfiguration];
    }
    return ret;
}

bool SetFlash (int camera, int state) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    AVCaptureDevice* device = [NatCamInstance()->Cameras objectAtIndex:camera];
    bool ret = false;
    if (device.flashAvailable) {
        if ([device lockForConfiguration:NULL] == YES ) {
            AVCaptureFlashMode flashMode = FlashMode(state);
            if ([device isFlashModeSupported:flashMode]) {
                [device setFlashMode:flashMode];
                ret = true;
            }
            else Log([NSString stringWithFormat:@"NatCam Error: Camera does not support %ld", (long)flashMode]);
            [device unlockForConfiguration];
        }
    }
    else {
        Log(@"NatCam Error: Camera does not have flash");
    }
    return ret;
}

bool SetTorch (int camera, int state) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    AVCaptureDevice* device = [NatCamInstance()->Cameras objectAtIndex:camera];
    bool ret = false;
    if ([device isTorchAvailable]) {
        if ([device lockForConfiguration:NULL] == YES) {
            if (state == 0) {
                [device setTorchMode:AVCaptureTorchModeOff];
                LogVerbose(@"Disabled torch");
                ret = true;
            }
            else if (state == 1) {
                BOOL success = [device setTorchModeOnWithLevel:AVCaptureMaxAvailableTorchLevel error:nil];
                if (success) {
                    LogVerbose(@"Enabled torch");
                    ret = true;
                }
                else Log(@"NatCam Error: Failed to enable torch");
            }
            [device unlockForConfiguration];
        }
    }
    else {
        Log(@"NatCam Error: Camera does not support torch.");
    }
    return ret;
}

bool SetZoom (int camera, float ratio) {
    if (!NatCamInstance()->inspectedDevice) [NatCamInstance() InspectDevice];
    AVCaptureDevice* device = [NatCamInstance()->Cameras objectAtIndex:camera];
    bool ret = false;
    float zoomRate = 2.0f; //CONST
    if ([[device activeFormat] videoMaxZoomFactor] > 1) {
        float zoomRatio = (([[device activeFormat] videoMaxZoomFactor] - 1) * ratio) + 1;
        if ([device lockForConfiguration:nil]) {
            [device rampToVideoZoomFactor:zoomRatio withRate:zoomRate];
            [device unlockForConfiguration];
            LogVerbose([NSString stringWithFormat:@"Set zoom ratio: %f", zoomRatio]);
            ret = true;
        }
    }
    return ret;
}

void SetPhotoSaveMode (int saveMode) {
    NatCamInstance()->photoSaveMode = saveMode;
}

#pragma mark --Utility--

void PreviewBuffer (void** RGBAPtr, size_t* RGBAS) {
    *RGBAPtr = NatCamInstance()->RGBA32;
    *RGBAS = NatCamInstance()->previewSize;
}

void ReleasePhotoBuffer () {
    if (NatCamInstance()->JPEG == NULL) return;
    delete [] NatCamInstance()->JPEG;
    NatCamInstance()->JPEG = NULL;
}

void SaveToGallery (const char * imageBytes, int mode) {
    NSString* image64 = [NSString stringWithUTF8String:imageBytes];
    NSData* imageData = [[NSData alloc] initWithBase64EncodedString:image64 options:NSDataBase64DecodingIgnoreUnknownCharacters];
    UIImage* image = [UIImage imageWithData:imageData];
    if (mode == 1) { //Save to Photos
        UIImageWriteToSavedPhotosAlbum(image, nil, nil, nil);
        Log(@"NatCam: Saved photo to photo album");
    }
    if (mode == 2) { //Save to App Album
        SaveImageToAppAlbum(image, NatCamInstance()->library);
    }
}

void SetVerboseMode (bool verbose) {
    SetOptions(verbose);
}

#pragma mark --Operations--

void PlayPreview (int cameraLocation) {
    [NatCamInstance() PlayPreview:cameraLocation];
}

void PausePreview () {
    [NatCamInstance() PausePreview];
}

void TerminateOperations () {
    [NatCamInstance() StopPreview];
}

void SwitchCamera (int camera) {
    [NatCamInstance() SwitchCamera:camera];
}

void CaptureStill () {
    [NatCamInstance() CapturePhoto];
}


#pragma mark --Initilaization--

-(void) InspectDevice {
    Cameras = [AVCaptureDevice devicesWithMediaType:AVMediaTypeVideo];
    inspectedDevice = true;
    Log([NSString stringWithFormat:@"NatCam: Inspected Device: Found %i cameras", (int)[Cameras count]]);
}

-(void) InitializeSession {
    session = [[AVCaptureSession alloc] init];
    sessionQueue = dispatch_queue_create("session queue", DISPATCH_QUEUE_SERIAL);
    LogVerbose(@"Created session");
    [session setSessionPreset:AVCaptureSessionPresetInputPriority];
    captureOutput	= [[AVCaptureVideoDataOutput alloc] init];
    captureOutput.alwaysDiscardsLateVideoFrames = YES;
    LogVerbose(@"Created preview output");
    [captureOutput setSampleBufferDelegate:self queue:dispatch_get_main_queue()]; //GLES thread conflict aversion
    [captureOutput setVideoSettings:@{(id)kCVPixelBufferPixelFormatTypeKey : [NSNumber numberWithInt:kCVPixelFormatType_32BGRA]}];
    if ([session canAddOutput:captureOutput]) { //Safe guard
        [session addOutput:captureOutput];
        LogVerbose(@"Added preview output");
    }
    else {
        Log(@"NatCam Error: Failed to add preview output to session. Terminating command");
        return;
    }
    if (metaDetection) {
        metadataOutput = [[AVCaptureMetadataOutput alloc] init];
        LogVerbose(@"Created MR output");
        if ([session canAddOutput:metadataOutput]) {
            [session addOutput:metadataOutput];
            LogVerbose(@"Successfully pinned MR output to session");
            [metadataOutput setMetadataObjectsDelegate:self queue:dispatch_get_main_queue()];
            LogVerbose(@"Set MR detection for 8 formats");
        }
        else {
            Log(@"NatCam Error: Failed to initialize machine-readable code detection");
        }
    }
    photoOutput = [[AVCaptureStillImageOutput alloc] init];
    photoOutput.highResolutionStillImageOutputEnabled = YES;
    LogVerbose(@"Created photo output");
    //NSDictionary *outputSettings = @{AVVideoCodecKey : AVVideoCodecJPEG};
    NSDictionary *outputSettings = @{(id)kCVPixelBufferPixelFormatTypeKey : [NSNumber numberWithInt:kCVPixelFormatType_32BGRA]};
    [photoOutput setOutputSettings:outputSettings];
    LogVerbose(@"Set photo output to data format JPEG");
    if ([session canAddOutput:photoOutput]) { //Safeguard
        [session addOutput:photoOutput];
        LogVerbose(@"Successfully pinned photo output to session");
    }
    else {
        Log(@"NatCam Error: Failed to pin photo output to session");
    }
    library = [[ALAssetsLibrary alloc] init];
    CMVideoSampling_Initialize(&videoSampling);
    previewWidth =
    previewHeight = 0;
    initializedSession = true;
}

#pragma mark --Control--

-(void) RegisterForAutofocus:(AVCaptureDevice*) device {
    //State checking
    if (device.isSubjectAreaChangeMonitoringEnabled == YES) return;
    if ([device lockForConfiguration:NULL] == YES) {
        device.subjectAreaChangeMonitoringEnabled = YES;
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(AutofocusDelegate:) name:AVCaptureDeviceSubjectAreaDidChangeNotification object:device];
        [device unlockForConfiguration];
    }
}

-(void) UnregisterFromAutofocus:(AVCaptureDevice*) device {
    //State checking
    if (device.isSubjectAreaChangeMonitoringEnabled == NO) return;
    if ([device lockForConfiguration:NULL] == YES ) {
        device.subjectAreaChangeMonitoringEnabled = NO;
        [[NSNotificationCenter defaultCenter] removeObserver:self name:AVCaptureDeviceSubjectAreaDidChangeNotification object:device];
        [device unlockForConfiguration];
    }
}

-(void) AutofocusDelegate:(NSNotification*) notification {
    AVCaptureDevice* device = [notification object];
    if (device.focusMode == AVCaptureFocusModeLocked){
        if ([device lockForConfiguration:NULL] == YES ) {
            if (device.isFocusPointOfInterestSupported) [device setFocusPointOfInterest:CGPointMake(0.5f, 0.5f)];
            if (device.isExposurePointOfInterestSupported) [device setExposurePointOfInterest:CGPointMake(0.5f, 0.5f)];
            if ([device isFocusModeSupported:AVCaptureFocusModeContinuousAutoFocus]) {
                [device setFocusMode:AVCaptureFocusModeContinuousAutoFocus];
            }
            [device unlockForConfiguration];
        }
    }
}

#pragma mark --Running--

-(void) PlayPreview:(int)location {
    if (!initializedSession) {
        [self InitializeSession];
    }
    //Remove all session inputs
    for (AVCaptureInput *input in [session inputs]) {
        [session removeInput:input];
    }
    LogVerbose(@"Disposed all inputs");
    activeCamera = location;
    AVCaptureDevice* device = [Cameras objectAtIndex:activeCamera];
    AVCaptureDeviceInput *input = [AVCaptureDeviceInput deviceInputWithDevice:device error:nil];
    if ([session canAddInput:input]) {
        [session addInput:input];
        LogVerbose(@"Added camera input");
    }
    else Log(@"NatCam Error: Failed to add camera input");
    if (metaDetection) [metadataOutput setMetadataObjectTypes:[metadataOutput availableMetadataObjectTypes]];
    if ([device lockForConfiguration:NULL] == YES) {
        [session startRunning];
        LogVerbose(@"Started session");
        [device unlockForConfiguration];
    }
    else Log(@"NatCam Error: Failed to acquire camera lock and start preview");
}

-(void) PausePreview {
    [session stopRunning];
    LogVerbose(@"Stopped session");
}

-(void) StopPreview {
    if (session.isRunning) [session stopRunning];
    for (AVCaptureInput *input1 in session.inputs) {
        [session removeInput:input1];
    }
    for (AVCaptureOutput *output2 in session.outputs) {
        [session removeOutput:output2];
    }
    [[NSNotificationCenter defaultCenter] removeObserver:self];
    if (RGBA32 != NULL) delete [] RGBA32; RGBA32 = NULL;
    Cameras = nil;
    initializedSession =
    inspectedDevice =
    metaDetection = false;
    photoSaveMode = 0;
    sessionQueue = nil;
    library = nil;
    session = nil;
    photoOutput = nil;
    metadataOutput = nil;
    captureOutput = nil;
    CMVideoSampling_Uninitialize(&videoSampling);
    _sharedInstance = nil;
}

-(void) SwitchCamera:(int)toCamera {
    //Select the camera
    activeCamera = toCamera;
    AVCaptureDevice *device = [Cameras objectAtIndex:activeCamera];
    [session beginConfiguration];
    NSArray* inputs = [session inputs];
    for (AVCaptureInput *input in inputs)
        [session removeInput:input];
    if (metaDetection) [metadataOutput setMetadataObjectTypes:nil];
    AVCaptureDeviceInput *newVideoInput = [AVCaptureDeviceInput deviceInputWithDevice:device error:nil];
    if ([session canAddInput:newVideoInput]) {
        [session addInput:newVideoInput];
        if (metaDetection) [metadataOutput setMetadataObjectTypes:[metadataOutput availableMetadataObjectTypes]];
        LogVerbose(@"Added new device camera input to session");
    }
    else {
        Log(@"NatCam Error: Failed to add new device camera to session");
    }
    [session commitConfiguration];
}

-(void) CapturePhoto {
    //Execute async
    dispatch_async(sessionQueue, ^{
        AVCaptureConnection *videoConnection = [photoOutput connectionWithMediaType:AVMediaTypeVideo];
        //Capture still
        [photoOutput captureStillImageAsynchronouslyFromConnection:videoConnection completionHandler: ^(CMSampleBufferRef sampleBuffer, NSError *error) {
            Log(@"NatCam: Captured photo");
            CVPixelBufferRef photo = CMSampleBufferGetImageBuffer(sampleBuffer);
            size_t w, h, s;
            CVPixelBufferLockBaseAddress(photo, 0);
            bool flip = IsRearFacing(activeCamera);
            float rotation;
            GetOrientation(rotation);
            JPEG = CorrectPhoto(photo, w, h, s, rotation, flip);
            CVPixelBufferUnlockBaseAddress(photo, 0);
            _updatePhotoCallback(JPEG, w, h, s);
            //Continue with saving image to device
            if (photoSaveMode > 0) {
                 UIImage *image = [[UIImage alloc] initWithCGImage:ImageFromSampleBuffer(sampleBuffer).CGImage scale:1 orientation:UIImageOrientationRight];
                 if (photoSaveMode == 1) { //Save to Photos
                     UIImageWriteToSavedPhotosAlbum(image, nil, nil, nil);
                     Log(@"NatCam: Saved image to photo album");
                 }
                 else if (photoSaveMode == 2) { //Save to App Album
                     SaveImageToAppAlbum(image, library);
                 }
            }
         }];
    });
}


#pragma mark --Callbacks--

- (void)captureOutput:(AVCaptureOutput*)avcaptureoutput didOutputSampleBuffer:(CMSampleBufferRef)sampleBuffer fromConnection:(AVCaptureConnection*)connection {
    AVCaptureVideoOrientation rotation;
    bool flip = IsRearFacing(activeCamera);
    GetOrientation(rotation);
    if ([connection isVideoOrientationSupported]) [connection setVideoOrientation:rotation];
    if ([connection isVideoMirroringSupported]) [connection setVideoMirrored:flip];
    void* gpuPtr = (void*)CMVideoSampling_SampleBuffer(&videoSampling, sampleBuffer, &previewWidth, &previewHeight);
    if (readablePreview) [self UpdateFrameData:sampleBuffer];
    _updateCallback(gpuPtr, RGBA32, previewWidth, previewHeight, previewSize);
    LogVerbose(@"Preview Callback");
}

- (void)captureOutput:(AVCaptureOutput *)captureOutputM didOutputMetadataObjects:(NSArray *)metadataObjects fromConnection:(AVCaptureConnection *)connection {
    for (AVMetadataObject *metadataObject in metadataObjects) {
        if (metadataObject.type == AVMetadataObjectTypeFace) {
            AVMetadataFaceObject *face = (AVMetadataFaceObject*) metadataObject;
            float
            roll = [face hasRollAngle] ? [face rollAngle] : 0.f,
            yaw = [face hasYawAngle] ? [face yawAngle] : 0.f,
            width = [face bounds].size.width,
            height = [face bounds].size.height,
            xpos = [face bounds].origin.x + 0.5f * width, //CHECK //Viewport coords
            ypos = [face bounds].origin.y + 0.5f * height; //CHECK //Viewport coords
            _updateFaceCallback([face faceID], xpos, ypos, width, height, roll, yaw);
        }
        else {
            AVMetadataMachineReadableCodeObject *code = (AVMetadataMachineReadableCodeObject*) metadataObject;
            _updateCodeCallback(MRCodeFormat(code.type), (void*)[code.stringValue cStringUsingEncoding:NSUTF16StringEncoding], code.stringValue.length);
        }
    }
}

#pragma mark --Utility--

-(void) UpdateFrameData:(CMSampleBufferRef)sampleBuffer {
    CVImageBufferRef frame = CMSampleBufferGetImageBuffer(sampleBuffer);
    CVPixelBufferLockBaseAddress(frame, kCVPixelBufferLock_ReadOnly);
    GLubyte *data = (GLubyte*)CVPixelBufferGetBaseAddress(frame);
    previewWidth = CVPixelBufferGetBytesPerRow(frame) / 4; //adjust for padding
    previewHeight = CVPixelBufferGetHeight(frame);
    previewSize = CVPixelBufferGetBytesPerRow(frame) * CVPixelBufferGetHeight(frame);
    if (RGBA32 == NULL) RGBA32 = new GLubyte[previewSize];
    memcpy(RGBA32, data, previewSize);
    CVPixelBufferUnlockBaseAddress(frame, kCVPixelBufferLock_ReadOnly);
}


@end