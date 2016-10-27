/*
*   NatCam
*   Copyright (c) 2016 Yusuf Olokoba
*/

using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace NatCamU {
    
    namespace Internals {
        
        public static class NatCamExtensions {
            
            public static bool Verbose;
            
            #region --NatCam Zooming--
            
            //Zoom stuff
            const float ZoomMultiplierConstant = 4f;
            public static float ZoomMultiplier { 
                get {
                    return NatCamPreviewZoomer.zoomOverride ? GameObject.FindObjectsOfType<NatCamPreviewZoomer>()[0].zoomSpeed : ZoomMultiplierConstant;
                }
            }
            #endregion
            

            #region --Ops--
            
            public static byte[] MarshalNativeBuffer (this IntPtr pointer, int size) {
                if (pointer == IntPtr.Zero) return null;
                byte[] buffer = new byte[size];
                Marshal.Copy(pointer, buffer, 0, size);
                return buffer;
            }
            
            public static void Dimensions (this ResolutionPreset preset, out int width, out int height) {
                switch (preset) {
                    case ResolutionPreset.FullHD: width = 1920; height = 1080; break;
                    case ResolutionPreset.HD: width = 1280; height = 720; break;
                    case ResolutionPreset.MediumResolution: width = 640; height = 480; break;
                    case ResolutionPreset.HighestResolution: width = 9999; height = 9999; break; //NatCam will pick the resolution closest to this, hence the highest
                    case ResolutionPreset.LowestResolution: width = 50; height = 50; break; //NatCam will pick the resolution closest to this, hence the lowest
                    default: width = height = 0; break;
                }
            }
            
            public static Coroutine Invoke (this IEnumerator routine, MonoBehaviour mono) {
                return mono.StartCoroutine(routine);
            }
            
            public static void Terminate (this Coroutine routine, MonoBehaviour mono) {
                mono.StopCoroutine(routine);
            }
            
            public static void LogVerbose (string log) {
                if (Verbose) Debug.Log("NatCam Logging: "+log);
            }
            
            public static void Warn (string warning) {
                Debug.LogWarning("NatCam Error: "+warning);
            }
            #endregion
        }
    }
}