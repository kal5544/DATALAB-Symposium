/* 
*   NatCam
*   Copyright (c) 2016 Yusuf Olokoba
*/

using UnityEngine;

namespace NatCamU {
    public abstract class NatCamPreviewBehaviour : MonoBehaviour {
        
        protected virtual void Awake () {
            if (this.isActiveAndEnabled) {
                //Subscribe for Apply event here
                NatCam.OnPreviewStart += Apply;
            }
        }
                
        protected virtual void OnDisable () {
            if (this.isActiveAndEnabled) {
                //Unsubscribe for Apply event here
                NatCam.OnPreviewStart -= Apply;
            }
        }
        
        protected virtual void Apply () {}
    }
}