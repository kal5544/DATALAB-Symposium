/* 
*   NatCam
*   Copyright (c) 2016 Yusuf Olokoba
*/

using UnityEngine;

namespace NatCamU {
    
    namespace Internals {
        
        [AddComponentMenu("")]
        public class NatCamHelper : MonoBehaviour {
            
            System.Action<bool> SetApplicationFocus;
            bool willDestroy;
            
            void Awake () {
                DontDestroyOnLoad(this);
                DontDestroyOnLoad(this.gameObject);
            }
            
            void OnApplicationPause (bool paused) {
                SetApplicationFocus(paused);
            }
            
            void OnApplicationQuit () {
                NatCam.Release();
            }
            
            void OnDisable () {
                if (!willDestroy) NatCam.Release(); //We check that this wasn't caused by NatCam.Release() before calling NatCam.Release();
            }
            
            public void RegisterCallback (System.Action<bool> focus) {
                SetApplicationFocus = focus;
            }

            public void WillDestroyMe () {
                willDestroy = true;
            }
        }
    }
}