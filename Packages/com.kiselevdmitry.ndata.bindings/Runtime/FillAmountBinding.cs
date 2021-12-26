using System;
using System.Collections.Generic;
using NData;
using UnityEngine.UI;

namespace KiselevDmitry.NData.Bindings {
    public class FillAmountBinding : BaseBinding {
        public String Path;
        public float MinValue = 0;
        public float MaxValue = 1;

        public String MinPath;
        public String MaxPath;
        
        private List<String> paths;
        private Image image;
        private Property<float> valueProperty;
        private Property<float> minProperty;
        private Property<float> maxProperty;

        public override void Awake() {
            paths = new List<String>();
            paths.Add(Path);
            if (!String.IsNullOrEmpty(MinPath)) paths.Add(MinPath);
            if (!String.IsNullOrEmpty(MaxPath)) paths.Add(MaxPath);
            image = GetComponent<Image>();
        }

        protected override void Bind() {
            var context = GetContext(Path);
            if (context == null) return;

            valueProperty = context.FindProperty<float>(Path, this);
            if (valueProperty != null) {
                valueProperty.OnChange += OnChange;
            }

            if (!String.IsNullOrEmpty(MinPath)) {
                minProperty = context.FindProperty<float>(MinPath, this);
                minProperty.OnChange += OnChange;
            }

            if (!String.IsNullOrEmpty(MaxPath)) {
                maxProperty = context.FindProperty<float>(MaxPath, this);
                maxProperty.OnChange += OnChange;
            }
        }

        protected override void Unbind() {
            if (valueProperty != null) {
                valueProperty.OnChange -= OnChange;
                valueProperty = null;
            }

            if (minProperty != null) {
                minProperty.OnChange -= OnChange;
                minProperty = null;
            }

            if (maxProperty != null) {
                maxProperty.OnChange -= OnChange;
                maxProperty = null;
            }
        }

        private float GetValue() {
            if (valueProperty != null) return valueProperty.GetValue();
            else return 0;
        }

        private float GetMinValue() {
            if (minProperty != null) return minProperty.GetValue();
            else return MinValue;
        }

        private float GetMaxValue() {
            if (maxProperty != null) return maxProperty.GetValue();
            else return MaxValue;
        }

        protected override void OnChange() {
            float value = GetValue();
            float minValue = GetMinValue();
            float maxValue = GetMaxValue();
            image.fillAmount = (value - minValue) / (maxValue - minValue);
        }

        public override IList<string> ReferencedPaths {
            get { return paths; }
        }
    }
}
