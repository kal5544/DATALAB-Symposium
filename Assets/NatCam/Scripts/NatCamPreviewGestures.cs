/* 
*   NatCam
*   Copyright (c) 2016 Yusuf Olokoba
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using NatCamU.Internals;
using Ext = NatCamU.Internals.NatCamExtensions;

namespace NatCamU {
	
	[RequireComponent(typeof(Graphic), typeof(RectTransform), typeof(EventTrigger))] [NCDoc(139)]
	public class NatCamPreviewGestures : NatCamPreviewBehaviour, IPointerUpHandler, IDragHandler, IEndDragHandler {
				
		//Publics
		[Tooltip(tapTip)] [NCDoc(13, 120)] public bool trackTapGestures;
		[Tooltip(pinchTip)] [NCDoc(14, 121)] public bool trackPinchGestures;
		[HideInInspector] [NCDoc(124)] public float ZoomDelta;
		
		//Op vars
		private Vector2[] previous = new Vector2[2];
		private Vector2[] current = new Vector2[2];
		private float zoomDelta;
		private float previousDelta;
		private float currentDelta;
		private float currentRatio;
		private const float ZOOM_DELTA_SMOOTHING = 14f;
		
		protected override void Awake () {
			//Base
			base.Awake();
			//Set raycast target
			GetComponent<Graphic>().raycastTarget = true;
		}
		
		void Update () {
			//Smoothing
			ZoomDelta = Mathf.Lerp(ZoomDelta, zoomDelta, ZOOM_DELTA_SMOOTHING * Time.deltaTime);
		}
		
		public void OnPointerUp (PointerEventData eventData) {
			if (trackTapGestures && NatCam.ActiveCamera != null && (NatCam.ActiveCamera.FocusMode == FocusMode.TapToFocus || NatCam.ActiveCamera.FocusMode == FocusMode.HybridFocus)) NatCam.ActiveCamera.SetFocus(Camera.main.ScreenToViewportPoint(eventData.pressPosition));
		}
		
		public void OnDrag (PointerEventData eventData) {
			if (!trackPinchGestures || NatCam.ActiveCamera == null || eventData.pointerId < 0 || eventData.pointerId > 1) return;
			previous[eventData.pointerId] = eventData.position - eventData.delta;
			current[eventData.pointerId] = eventData.position;
			previousDelta = Camera.main.ScreenToViewportPoint(previous[0] - previous[1]).magnitude;
			currentDelta = Camera.main.ScreenToViewportPoint(current[0] - current[1]).magnitude;
			zoomDelta = currentDelta - previousDelta; //transpose -1
			//Check for zoom override and zoom support
			if (NatCam.ActiveCamera == null || !NatCam.ActiveCamera.IsZoomSupported || NatCamPreviewZoomer.zoomOverride) return;
			currentRatio = NatCam.ActiveCamera.ZoomRatio;
			currentRatio = Mathf.Clamp01(currentRatio + zoomDelta * Ext.ZoomMultiplier);
			NatCam.ActiveCamera.ZoomRatio = currentRatio;
		}
		
		public void OnEndDrag (PointerEventData eventData) {
			//Zerofy
			currentDelta = previousDelta = 0f;
			zoomDelta = 0f;
		}
		
		private const string tapTip = @"Whether tap gestures should be detected for focusing.";
		private const string pinchTip = @"Whether pinch gestures should be tracked for zooming.";
	}
}