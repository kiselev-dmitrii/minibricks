using System;
using NData;
using UnityEngine.UI;

namespace KiselevDmitry.NData.Bindings {
    public class TextBinding<T> : Binding {
        public String Format = "{0}";
        private Property<T> property;
        private Text text;

        public override void Awake() {
            text = GetComponent<Text>();
        }

        protected override void Bind() {
            var context = GetContext(Path);
            if (context == null) return;

            property = context.FindProperty<T>(Path, this);
            if (property == null) return;

            property.OnChange += OnChange;
        }

        protected override void Unbind() {
            if (property == null) return;
            property.OnChange -= OnChange;
            property = null;
        }

        protected override void OnChange() {
            if (property == null) return;
            text.text = String.Format(Format, property.GetValue());
        }
    }
}
