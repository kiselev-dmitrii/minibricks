using UnityEngine;

namespace NData {
	public static class Utils
	{
		public static string FullName(GameObject go)
		{
			var fullName = go.name;
			var p = go.transform.parent;
			while (p != null)
			{
				fullName = p.name + "." + fullName;
				p = p.parent;
			}
			return fullName;
		}
	
		public static DataContext FindRootContext(GameObject gameObject, int depthToGo)
		{
			DataContext lastGoodContext = null; 
			var p = gameObject;
			depthToGo++;
			while (p != null && depthToGo > 0)
			{
				var context = p.GetComponent<DataContext>();
				if (context != null)
				{
					lastGoodContext = context;
					depthToGo--;
				}
				p = (p.transform.parent == null) ? null : p.transform.parent.gameObject;
			}
			return lastGoodContext;
		}
	
		public const int MaxPathDepth = 999999;
	
		public static int GetPathDepth(string path)
		{
			if (!path.StartsWith("#"))
				return 0;
			var depthString = path.Substring(1);
			var dotIndex = depthString.IndexOf('.');
			if (dotIndex >= 0)
				depthString = depthString.Substring(0, dotIndex);
			if (depthString == "#")
			{
				return MaxPathDepth;
			}
			var depth = 0;
			if (int.TryParse(depthString, out depth))
			{
				return depth;
			}
			Debug.LogWarning("Failed to get binding context depth for: " + path);
			return 0;
		}
	
		public static string GetCleanPath(string path)
		{
			if (!path.StartsWith("#"))
				return path;
			var dotIndex = path.IndexOf('.');
			var result = (dotIndex < 0) ? path : path.Substring(dotIndex + 1);
			return result;
		}
	
		public static T GetComponentInParents<T>(GameObject gameObject)
			where T : Component
		{
			var p = gameObject;
		
			T component = null;
			while (p != null && component == null)
			{
				component = p.GetComponent<T>();
				p = (p.transform.parent == null) ? null : p.transform.parent.gameObject;
			}
			return component;
		}
		
		public static T MasterPath<T>(GameObject gameObject)
			where T : Component
		{
			return GetComponentInParents<T>((gameObject.transform.parent == null)
				? null
				: gameObject.transform.parent.gameObject);
		}
	
	}
}
