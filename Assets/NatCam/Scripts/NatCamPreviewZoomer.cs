/* 
*   NatCam
*   Copyright (c) 2016 Yusuf Olokoba
*/


using UnityEngine;
using UnityEngine.UI;
using NatCamU.Internals;

namespace NatCamU {
    
    [RequireComponent(typeof(Graphic), typeof(RectTransform), typeof(NatCamPreviewGestures))] [NCDoc(20)]
    public class NatCamPreviewZoomer : NatCamPreviewBehaviour {
        
        [Tooltip(modeTip)] [NCDoc(70)] [NCRef(1)] public ZoomMode zoomMode = ZoomMode.DigitalZoomAsFallback;
        [Tooltip(ratioTip)] [Range(0f, 1f)] [NCDoc(71)] public float zoomRatio;
        [Tooltip(speedTip)] [Range(1f, 5f)] [NCDoc(72)] public float zoomSpeed = 4f;
        
        //State vars
        private Graphic graphic;
        private Material material;
        private Material originalMaterial;
        private bool isActive = false;
        
        public static bool zoomOverride = false;
        
        //Utility
        private NatCamPreviewGestures gestures {
            get {
                _gestures = _gestures ?? GetComponent<NatCamPreviewGestures>();
                return _gestures;
            }
        }
        private NatCamPreviewGestures _gestures;
        
        protected override void Apply () {
            //State checking
            if (isActive) return;
            graphic = GetComponent<Graphic>();
            material = new Material(Shader.Find("Hidden/NatCam/Zoom2D"));
            originalMaterial = graphic.material;
            isActive = true;
            Debug.Log("NatCam: Active camera "+(NatCam.ActiveCamera.IsZoomSupported ? "supports hardware optical zoom" : "does not support hardware optical zoom. Falling back to shader-accelerated digital zoom"));
        }
        
        public void Update () {
            if (isActive) {
                zoomRatio += gestures.ZoomDelta * zoomSpeed;
                zoomRatio = Mathf.Clamp01(zoomRatio);
                switch (zoomMode) {
                    case ZoomMode.DigitalZoomAsFallback:
                        if (!NatCam.ActiveCamera.IsZoomSupported) {
                            if (graphic.material != material) graphic.material = material;
                            material.SetFloat("_ZoomFactor", zoomRatio);
                            zoomOverride = true;
                        }
                        break;
                    case ZoomMode.ForceDigitalZoomOnly:
                        if (graphic.material != material) graphic.material = material;
                        material.SetFloat("_ZoomFactor", zoomRatio);
                        zoomOverride = true;
                        break;
                    case ZoomMode.ZoomSpeedOverrideOnly:
                        ;
                        break;
                }
            }
        }
        
        protected override void OnDisable () {
            base.OnDisable();
            if (graphic != null && graphic.material != originalMaterial) graphic.material = originalMaterial;
            if (zoomOverride) zoomOverride = false;
            isActive = false;
        }
        
        private const string modeTip = @"This dictates how NatCam applies zooming. DigitalZoomAsFallback mode will 
        use shader-accelerated digital zoom as a fallback when hardware zooming is not available. ForceDigitalZoomOnly 
        mode will always perform digital zoom regardless of whether the camera supports optical zoom. Forcing hardware 
        optical zoom is implied when no NatCamPreviewUIPanelZoomer is active in the scene. ZoomSpeedOverrideOnly mode 
        will override NatCam's default zoomSpeed when performing pinchToZoom, but will not digitally zoom the 
        camera preview.";
        private const string ratioTip = "This is the percent zoom ratio for the active camera.";
        private const string speedTip = @"This dictates the zoom speed multiplier to apply when NatCam calculates pinch-to-zoom. 
        This overrides NatCam's default pinch-to-zoom multiplier.";
    }
}