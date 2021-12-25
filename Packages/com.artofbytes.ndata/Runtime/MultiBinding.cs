using System.Collections.Generic;

namespace NData {
	[System.Serializable]
	public class MultiBinding : BaseBinding {
		public List<string> Paths;
		public override IList<string> ReferencedPaths => Paths;
	}
}