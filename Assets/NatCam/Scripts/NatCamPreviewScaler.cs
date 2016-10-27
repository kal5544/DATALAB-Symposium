/* 
*   NatCam
*   Copyright (c) 2016 Yusuf Olokoba
*/

#if UNITY_5_3_4 || UNITY_5_3_5 || UNITY_5_3_6 || UNITY_5_4_OR_NEWER
    #define VERTEX_HELPER
#endif

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NatCamU.Internals;
using Ext = NatCamU.Internals.NatCamExtensions;

namespace NatCamU {
    
    [RequireComponent(typeof(Graphic))] [NCDoc(19)]
    public class NatCamPreviewScaler : NatCamPreviewBehaviour, IMeshModifier { //UPDATE //NCDOC
        
        [Tooltip(scaleTip)] [NCDoc(68)] [NCRef(0)] public ScaleMode scaleMode = ScaleMode.None;
		[Tooltip(scaleOrientationTip)] public bool scaleWithOrientation;
        
        private Vector2 dim = Vector2.zero;
        private Graphic graphic {get {_graphic = _graphic ?? GetComponent<Graphic>(); return _graphic;}}
        private Graphic _graphic;


        #region --Unity Messages--

        void LateUpdate () {
            //Unity iOS bug :/
            if (Screen.orientation == ScreenOrientation.Unknown) return;
            //Check texture dimensions //GC
			if (graphic.mainTexture != null) dim = graphic.mainTexture == NatCam.Preview ? OrientedDimensions(NatCam.Preview) : scaleWithOrientation ? OrientedDimensions(graphic.mainTexture) : new Vector2(graphic.mainTexture.width, graphic.mainTexture.height);
			else dim = graphic.rectTransform.rect.size;
            //Dirty
            if (graphic) graphic.SetAllDirty();
        }
        #endregion


        #region --Unity UI Callbacks--
        
        public void ModifyMesh (VertexHelper helper) {
            #if VERTEX_HELPER
            Vector3[] verts = CalculateVertices();
            UIVertex[] quad = new UIVertex[4];
            UIVertex vert = UIVertex.simpleVert;
            //Get the color
            Color color = graphic == null ? Color.white : graphic.color;
            //Vert0
            vert.position = verts[0];
            vert.uv0 = new Vector2(0f, 0f);
            vert.color = color;
            quad[0] = vert;
            //Vert1
            vert.position = verts[1];
            vert.uv0 = new Vector2(0, 1);
            vert.color = color;
            quad[1] = vert;
            //Vert2
            vert.position = verts[2];
            vert.uv0 = new Vector2(1, 1);
            vert.color = color;
            quad[2] = vert;
            //Vert3
            vert.position = verts[3];
            vert.uv0 = new Vector2(1, 0f);
            vert.color = color;
            quad[3] = vert;
            //Helper
            helper.Clear();
            helper.AddUIVertexQuad(quad);
            #endif
        }

        public void ModifyMesh (Mesh mesh) {
            #if !VERTEX_HELPER
            var list = new System.Collections.Generic.List<Vector3>(CalculateVertices());
            mesh.SetVertices(list);
            #endif
        }
        #endregion
        

        #region --Panel Scaling--

        Vector3[] CalculateVertices () {
            Vector2 corner1 = Vector2.zero;
            Vector2 corner2 = Vector2.one;
            float width, height;
            CalculateExtents(out width, out height);
            //Scale
            corner1.x *= width;
            corner1.y *= height;
            corner2.x *= width;
            corner2.y *= height;
            //Create a pivot vector, and pivot compensation vector
            Vector3 piv = new Vector3(graphic.rectTransform.pivot.x, graphic.rectTransform.pivot.y, 0f), comp = new Vector3(piv.x * width, piv.y * height, 0f);
            Vector3[] verts = new [] {
                new Vector3(corner1.x, corner1.y, 0f) - comp,
                new Vector3(corner1.x, corner2.y, 0f) - comp,
                new Vector3(corner2.x, corner2.y, 0f) - comp,
                new Vector3(corner2.x, corner1.y, 0f) - comp
            };
            return verts;
        }

        void CalculateExtents (out float width, out float height) {
            width = height = 0;
            if (graphic == null) return;
            dim = dim == Vector2.zero ? graphic.rectTransform.rect.size : dim;
            float
			aspect = dim.x / dim.y,
			viewAspect = graphic.rectTransform.rect.size.x / graphic.rectTransform.rect.size.y;
            switch (scaleMode) {
                case ScaleMode.FixedWidthVariableHeight :
                    width = graphic.rectTransform.rect.width;
                    height = width / aspect;
                break;
                case ScaleMode.FixedHeightVariableWidth :
                    height = graphic.rectTransform.rect.height;
                    width = height * aspect;
                break;
                case ScaleMode.FillView :
					if (aspect > viewAspect)
					goto case ScaleMode.FixedHeightVariableWidth;
					else goto case ScaleMode.FixedWidthVariableHeight;
                case ScaleMode.FillScreen :
                    float scale = Mathf.Max(graphic.canvas.pixelRect.width / dim.x, graphic.canvas.pixelRect.height / dim.y) / graphic.canvas.scaleFactor;
                    width = scale * dim.x;
                    height = scale * dim.y;
                break;
                case ScaleMode.None :
                    width = graphic.rectTransform.rect.width;
                    height = graphic.rectTransform.rect.height;
                break;
            }
        }

        Vector2 OrientedDimensions (Texture tex) {
            Vector2 input = new Vector2(tex.width, tex.height);
			if (!Application.isMobilePlatform) return input;
            bool isPortrait = 
                Screen.orientation == ScreenOrientation.Portrait || 
                Screen.orientation == ScreenOrientation.PortraitUpsideDown || 
                (Screen.orientation == ScreenOrientation.AutoRotation && 
                (Input.deviceOrientation == DeviceOrientation.Portrait || 
                Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)); //This is the only appropriate flag
            int min = Mathf.RoundToInt(Mathf.Min(input.x, input.y)), max = Mathf.RoundToInt(Mathf.Max(input.x, input.y));
            Ext.LogVerbose("PreviewScaler: orientation-"+Screen.orientation+" portrait-"+isPortrait+" min-"+min+" max-"+max);
            return new Vector2 {
                x = isPortrait ? min : max,
                y = isPortrait ? max : min
            };
        }
        #endregion

        private const string
		scaleTip = "This dictates how NatCam applies scaling considering the active camera's preview resolution.",
		scaleOrientationTip = @"Whether scaling should account for the app orientation.";
    }
}