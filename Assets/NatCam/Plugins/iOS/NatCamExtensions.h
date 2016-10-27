//
//  NatCamExtensions.h
//  NatCam Extensions
//
//  Created by Yusuf on 1/9/16.
//  Copyright (c) 2016 Yusuf Olokoba
//

#import <AVFoundation/AVFoundation.h>
#import <Accelerate/Accelerate.h>
#import <AssetsLibrary/AssetsLibrary.h>

//Helpers
void SetOptions (bool verbose);
void Log (NSString* log);
void LogVerbose (NSString* log);
void GetOrientation (AVCaptureVideoOrientation& rotation);
void GetOrientation (float& rotation);
AVCaptureFlashMode FlashMode (int mode);
AVCaptureDeviceFormat* FormatWithBestResolution (AVCaptureDevice* device, int pwidth, int pheight);
AVCaptureDeviceFormat* FormatWithPhotoResolution (AVCaptureDevice* device, int pwidth, int pheight);
AVFrameRateRange* ClosestFramerate (AVCaptureDevice* device, float rate);
int MRCodeFormat (NSString* mrType);
UIImage* ImageFromSampleBuffer (CMSampleBufferRef sampleBuffer);
GLubyte* CorrectPhoto (CVImageBufferRef imageBuffer, size_t& width, size_t& height, size_t& size, const float rotation, const bool flip);
void SaveImageToAppAlbum (UIImage* image, ALAssetsLibrary* library);