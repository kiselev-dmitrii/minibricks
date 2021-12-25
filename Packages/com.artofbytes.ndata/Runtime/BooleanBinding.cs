using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NData {
	[System.Serializable]
	public class BooleanBinding : Binding
	{
		private readonly Dictionary<Type, global::NData.Property> _properties = new Dictionary<Type, global::NData.Property>();
	
		public enum CHECK_TYPE
		{
			BOOLEAN,
			EQUAL_TO_REFERENCE,
			GREATER_THAN_REFERENCE,
			LESS_THAN_REFERENCE,
			EMPTY,
			IS_LIST_SELECTION
		}
	
		public CHECK_TYPE CheckType = CHECK_TYPE.BOOLEAN;
		public double Reference = 0;
		[HideInInspector] public String StringReference;
	
		public bool DefaultValue = false;

		public bool Invert = false;

		public bool EvaluateExpression = false;

		// For EnumBinding only
		[HideInInspector] public String EnumType;
		[HideInInspector] public List<int> EnumValues;

		private bool _ignoreValueChange = false;
	
		private ItemDataContext _listItem;

		public override void Awake()
		{
			base.Awake();

			if (!EvaluateExpression) {
				_properties.Add(typeof (bool), null);
				_properties.Add(typeof (int), null);
				_properties.Add(typeof (Enum), null);
				_properties.Add(typeof (float), null);
				_properties.Add(typeof (double), null);
				_properties.Add(typeof (string), null);
			}
		}
	
		
		protected override void Unbind() {
			base.Unbind();

			foreach (var p in _properties) {
				if (p.Value != null) p.Value.OnChange -= OnChange;
			}

			var keys = _properties.Keys.ToArray();
			foreach (var key in keys) _properties[key] = null;
		}
	
		protected override void Bind() {
			base.Bind();

			var context = GetContext(Path);
			if (context != null) {
				_properties[typeof (bool)] = context.FindProperty<bool>(Path, this);
				_properties[typeof (int)] = context.FindProperty<int>(Path, this);
				_properties[typeof (Enum)] = context.FindEnumProperty(Path, this);
				_properties[typeof (float)] = context.FindProperty<float>(Path, this);
				_properties[typeof (double)] = context.FindProperty<double>(Path, this);
				_properties[typeof (string)] = context.FindProperty<string>(Path, this);
			}

			foreach (var p in _properties) {
				if (p.Value != null) p.Value.OnChange += OnChange;
			}

		}
	
		protected override void OnChange() {
			base.OnChange();
		
			var newValue = DefaultValue;

			if (CheckType == CHECK_TYPE.BOOLEAN) {
				if (_properties[typeof (bool)] != null) {
					newValue = ((global::NData.Property<bool>) _properties[typeof (bool)]).GetValue();
				}
			} else if (CheckType == CHECK_TYPE.EMPTY) {
				if (_properties[typeof (string)] != null) newValue = string.IsNullOrEmpty(((global::NData.Property<string>) _properties[typeof (string)]).GetValue());
			} else if (CheckType == CHECK_TYPE.IS_LIST_SELECTION) {
				if (_listItem != null) newValue = _listItem.Selected;
			} else {
				var val = 0.0;
				if (_properties[typeof (int)] != null) val = ((global::NData.Property<int>) _properties[typeof (int)]).GetValue();
				if (_properties[typeof (Enum)] != null) val = ((global::NData.Property<int>) _properties[typeof (Enum)]).GetValue();
				if (_properties[typeof (float)] != null) val = ((global::NData.Property<float>) _properties[typeof (float)]).GetValue();
				if (_properties[typeof (double)] != null) val = ((global::NData.Property<double>) _properties[typeof (double)]).GetValue();

				switch (CheckType) {
					case CHECK_TYPE.EQUAL_TO_REFERENCE:
						newValue = (val == Reference);
						break;
					case CHECK_TYPE.GREATER_THAN_REFERENCE:
						newValue = (val > Reference);
						break;
					case CHECK_TYPE.LESS_THAN_REFERENCE:
						newValue = (val < Reference);
						break;
				}
			}

			if (!_ignoreValueChange) {
				ApplyNewValue(Invert ? (!newValue) : newValue);
			}
		}
	
		protected virtual void ApplyInputValue(bool inputValue) {
			if (CheckType != CHECK_TYPE.BOOLEAN) return;
			if (EvaluateExpression) return;
		
			inputValue = Invert ? (!inputValue) : inputValue;
		
			_ignoreValueChange = true;
		
			if (_properties[typeof(bool)] != null)
				((global::NData.Property<bool>)_properties[typeof(bool)]).SetValue(inputValue);
		
			_ignoreValueChange = false;
		}
	
		protected virtual void ApplyNewValue(bool newValue) {
			Debug.LogError("Not supposed to be here for " + Path);
		}
	}
}