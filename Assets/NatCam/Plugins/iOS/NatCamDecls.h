//
//  NatCamDecls.h
//  NatCam
//
//  Created by Yusuf on 4/14/16.
//  Copyright (c) 2016 Yusuf Olokoba
//

//Callback definitions
typedef void (*RenderCallback) (int request);
typedef void (*UpdateCallback) (void* RGBA32GPUPtr, void* RGBA32Ptr, size_t width, size_t height, size_t size);
typedef void (*UpdatePhotoCallback) (void* JPGPtr, size_t width, size_t height, size_t size);
typedef void (*UpdateCodeCallback) (int format, void* code, size_t length);
typedef void (*UpdateFaceCallback) (int id, float xpos, float ypos, float xdim, float ydim, float roll, float yaw);

//Callbacks
extern RenderCallback _renderCallback;
extern UpdateCallback _updateCallback;
extern UpdatePhotoCallback _updatePhotoCallback;
extern UpdateCodeCallback _updateCodeCallback;
extern UpdateFaceCallback _updateFaceCallback;