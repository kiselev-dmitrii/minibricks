using System;
using UnityEngine;

namespace NData {
	[Serializable]
	public class DataContext : MonoBehaviour
	{
		protected IContext _context;
		
		public Property<T> FindProperty<T>(string path, BaseBinding binding)
		{
			if (_context == null)
			{
				return null;
			}
			try
			{
				var context = _context as Context;
				if (context != null)
					return context.FindProperty<T>(binding.GetFullCleanPath(path), binding);

				var mbContext = _context as MonoBehaviourContext;
				if (mbContext != null)
					return mbContext.FindProperty<T>(binding.GetFullCleanPath(path), binding);

				Debug.LogWarning("Unsupported context implementation");
				return null;
			}
			catch(Exception ex)
			{
				Debug.LogError("Failed to find property " + path + "\n" + ex);
				return null;
			}
		}

		public Property FindProperty(string path, BaseBinding binding) {
			if (_context == null) return null;
			var context = _context as Context;
			if (context != null) {
				return context.FindProperty(binding.GetFullCleanPath(path), binding);
			}
			return null;
		}

		public Property<int> FindEnumProperty(string path, BaseBinding binding)
		{
			if (_context == null)
			{
				return null;
			}
			try
			{
				return _context.FindEnumProperty(binding.GetFullCleanPath(path), binding);
			}
			catch(Exception ex)
			{
				Debug.LogError("Failed to find enum property " + path + "\n" + ex);
				return null;
			}
		}
	
		public System.Delegate FindCommand(string path, BaseBinding binding)
		{
			if (_context == null)
			{
				return null;
			}
			try
			{
				return _context.FindCommand(binding.GetFullCleanPath(path), binding);
			}
			catch(Exception ex)
			{
				Debug.LogError("Failed to find command " + path + "\n" + ex);
				return null;
			}
		}

		public System.Delegate FindParameterizedCommand(string path, BaseBinding binding) {
			if (_context == null) {
				return null;
			}
			try {
				return _context.FindParameterizedCommand(binding.GetFullCleanPath(path), binding);
			} catch (Exception ex) {
				Debug.LogError("Failed to find parameterized command " + path + "\n" + ex);
				return null;
			}
		}
	
		public Collection FindCollection(string path, BaseBinding binding)
		{
			if (_context == null)
			{
				return null;
			}
			try
			{
				return _context.FindCollection(binding.GetFullCleanPath(path), binding);
			}
			catch(Exception ex)
			{
				Debug.LogError("Failed to find collection " + path + "\n" + ex);
				return null;
			}
		}

		public Context FindContext(string path, BaseBinding binding) {
			if (_context == null) {
				return null;
			}
			try {
				return _context.FindContext(binding.GetFullCleanPath(path), binding);
			} catch (Exception ex) {
				Debug.LogError("Failed to find collection " + path + "\n" + ex);
				return null;
			}
		}

		public IContext GetContext() {
			return _context;
		}
	}
}