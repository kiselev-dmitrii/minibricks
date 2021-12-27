using System;
using NData;
using UnityEngine;
using UnityEngine.UI;

namespace KiselevDmitry.NData.Bindings {
    [RequireComponent(typeof(Text))]
    public class TextBinding : Binding {
        public String Format = "{0}";
        private Property property;
        private Text text;

        public override void Awake() {
            text = GetComponent<Text>();
        }

        protected override void Bind() {
            var context = GetContext(Path);
            if (context == null) return;

            property = context.FindProperty(Path, this);
            if (property != null) {
                property.OnChange += OnChange;
            }
        }

        protected override void Unbind() {
            if (property != null) {
                property.OnChange -= OnChange;
                property = null;
            }
        }

        protected override void OnChange() {
            if (property == null) {
                return;
            }
            text.text = String.Format(Format, property.GetStringValue());
        }
    }
}
