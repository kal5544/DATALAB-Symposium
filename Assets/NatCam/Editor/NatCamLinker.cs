#if UNITY_5 && UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

namespace NatCamU {

	namespace Internals {

		public static class NatCamLinker {

			[PostProcessBuild]
			public static void LinkFrameworks (BuildTarget buildTarget, string path) {
				if (buildTarget == BuildTarget.iOS) {
					string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
					PBXProject proj = new PBXProject();
					proj.ReadFromString(System.IO.File.ReadAllText(projPath));
					string target = proj.TargetGuidByName("Unity-iPhone");
					proj.AddFrameworkToProject(target, "Accelerate.framework", true);
					proj.AddFrameworkToProject(target, "AssetsLibrary.framework", true);
					File.WriteAllText(projPath, proj.WriteToString());
				}
			}
		}
	}
}
#endif