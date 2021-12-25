using UnityEngine;

namespace NData {
	public class MasterPath : MonoBehaviour {
		public string Path = "";
		public bool IsRelative = false;
	
		public string GetFullPath() {
			var parent = Utils.MasterPath<MasterPath>(gameObject);
			var parentMasterPath = (parent == null) ? string.Empty : parent.GetFullPath();
			var fullPath = (string.IsNullOrEmpty(parentMasterPath)) ?
				Path : (parentMasterPath + "." + Path);
			return fullPath;
		}
	}
}