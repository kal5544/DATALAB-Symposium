using UnityEngine;
using UnityEngine.UI;
using NatCamU.Internals;
using Ext = NatCamU.Internals.NatCamExtensions;

namespace NatCamU {
	
	[NCDoc(140)]
	public class UnitygramBase : MonoBehaviour { //This class is easily subclassed and overriden
		
		[Header("Preview")]
		[NCDoc(149)] public RawImage RawImage;
		
		[Header("Parameters")]
		[NCDoc(141)] public NatCamInterface Interface = NatCamInterface.NativeInterface;
		[NCDoc(142)] public PreviewType PreviewType = PreviewType.NonReadable;
		[NCDoc(143)] public Switch DetectMetadata = Switch.Off;
		[NCDoc(144)] public Facing Facing = Facing.Rear;
		[NCDoc(145)] public ResolutionPreset Resolution = ResolutionPreset.MediumResolution;
		[NCDoc(146)] public FrameratePreset Framerate = FrameratePreset.Default;
		[NCDoc(147)] public FocusMode FocusMode = FocusMode.AutoFocus;
		
		[Header("Debugging")]
		[NCDoc(148)] public Switch VerboseMode;
		
		// Use this for initialization
		[NCDoc(150)]
		public virtual void Start () {
			//Initialize NatCam
			NatCam.Initialize(Interface, PreviewType, DetectMetadata);
			//Set verbose mode
			NatCam.Verbose = VerboseMode;
			//Set the active camera
			NatCam.ActiveCamera = Facing == Facing.Front ? DeviceCamera.FrontCamera : DeviceCamera.RearCamera;
			//Null checking
			if (NatCam.ActiveCamera == null) {
				//Log
				Ext.Warn("Unitygram: Active camera is null. Consider changing the facing of the camera. Terminating");
				//Return
				return;
			}
			//Set the camera's resolution
			NatCam.ActiveCamera.SetResolution(Resolution);
			//Set the camera's framerate
			NatCam.ActiveCamera.SetFramerate(Framerate);
			//Set the camera's focus mode
			NatCam.ActiveCamera.FocusMode = FocusMode;
			//Play
			NatCam.Play();
			//Pass a callback to be executed once the preview starts //Note that this is a MUST when assigning the preview texture to anything
			NatCam.ExecuteOnPreviewStart(OnPreviewStart);
			//Register for preview updates
			NatCam.OnPreviewUpdate += OnPreviewUpdate;
		}

		///<summary>
		///Method called when the camera preview starts
		///</summary>
		[NCDoc(176)] [NCCode(31)]
		public virtual void OnPreviewStart () {
			//Set the RawImage texture once the preview starts
			if (RawImage != null) RawImage.texture = NatCam.Preview;
			//Log
			else Ext.Warn("Unitygram: Preview RawImage has not been set");
			//Log
			Ext.LogVerbose("Unitygram: Preview resolution: " + NatCam.ActiveCamera.ActiveResolution);
		}

		///<summary>
		///Method called on every frame that the camera preview updates
		///</summary>
		[NCDoc(177)] [NCCode(32)]
		public virtual void OnPreviewUpdate () {}

		///<summary>
		///Switch the active camera of the preview
		///</summary>
		///<param name="newCamera">Optional. The camera to switch to. An int or a DeviceCamera can be passed here</param>
		[NCDoc(27, 163)] [NCCode(27)] [NCCode(28)] [NCCode(29)]
		public virtual void SwitchCamera (int newCamera = -1) {
			//Select the new camera ID //If -1 is passed in, NatCam will switch to the next camera
			newCamera = newCamera < 0 ? (NatCam.ActiveCamera + 1) % DeviceCamera.Cameras.Count : newCamera;
			//Set the new active camera
			NatCam.ActiveCamera = (DeviceCamera)newCamera;
		}

		private void OnDisable () {
			//Unregister from preview updates
			NatCam.OnPreviewUpdate -= OnPreviewUpdate;
		}
	}
}