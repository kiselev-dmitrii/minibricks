using System;
using NData;
using UnityEngine;

namespace KiselevDmitry.NData.Bindings {
    [RequireComponent(typeof(Animation))]
    public class AnimationOnChangeBinding : Binding {
        public String ClipName;

        private Property property;
        private new Animation animation;
        private bool isInited;

        public override void Awake() {
            base.Awake();
            animation = GetComponent<Animation>();
        }

        protected override void Bind() {
            var context = GetContext(Path);
            if (context == null) return;

            property = context.FindProperty(Path, this);
            if (property == null) return;

            isInited = false;
            property.OnChange += OnChange;
        }

        protected override void Unbind() {
            if (property == null) return;
            property.OnChange -= OnChange;
            property = null;
        }

        protected override void OnChange() {
            if (property == null) return;

            if (!isInited) {
                isInited = true;
                return;
            }

            var clip = animation[ClipName];
            clip.normalizedTime = 0;
            animation.Play(ClipName);
        }
    }
}
