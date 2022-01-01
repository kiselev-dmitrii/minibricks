namespace NData {
	public class ItemDataContext : DataContext {
		public event System.Action OnSelectedChange;
	
		private bool _selected;
		public bool Selected
		{
			get { return _selected; }
			private set
			{
				bool needUpdate = (value != _selected) && (OnSelectedChange != null);
				_selected = value;
				if (needUpdate)
					OnSelectedChange();
			}
		}
		public int Index { get; private set; }
	
		public void SetSelected(bool selected)
		{
			Selected = selected;
		}
	
		public void SetIndex(int index)
		{
			Index = index;
		}

		public IContext PublicContext {
			get {
				return _context;
			}
		}
		public virtual void SetContext(IContext c, bool includeInactive = false)
		{
			_context =  c;

			if (!gameObject.activeInHierarchy && !includeInactive) return;

			var bindings = gameObject.GetComponentsInChildren<Binding>(includeInactive);
			foreach (var binding in bindings) {
				if (binding == null) continue;
				binding.UpdateBinding();
			}

			var multiBindings = gameObject.GetComponentsInChildren<MultiBinding>(includeInactive);
			foreach (var binding in multiBindings) {
				if (binding == null) continue;
				binding.UpdateBinding();
			}
		}
	}
}