using System.Collections.Generic;
using KiselevDmitry.Utils;
using NData;
using UnityEngine;

namespace KiselevDmitry.NData.Bindings {
    public class CollectionBinding : Binding {
        public RectTransform Template;
        
        private Collection collection;
        private List<RectTransform> widgets;

        public override void Awake() {
            widgets = new List<RectTransform>();
            base.Awake();
        }

        protected override void Bind() {
            var context = GetContext(Path);
            if (context == null) return;

            collection = context.FindCollection(Path, this);
            if (collection == null) return;

            collection.OnItemInsert += OnItemInsert;
            collection.OnItemsClear += OnItemsClear;
            collection.OnItemRemove += OnItemRemove;
            for (var i = 0; i < collection.ItemsCount; ++i) {
                OnItemInsert(i, collection.GetBaseItem(i));
            }
        }
        
        protected override void Unbind() {
            if (collection == null) return;
            collection.OnItemInsert -= OnItemInsert;
            collection.OnItemsClear -= OnItemsClear;
            collection.OnItemRemove -= OnItemRemove;
            collection = null;
            OnItemsClear();
        }

        protected void OnItemInsert(int position, Context item) {
            var widget = Instantiate(Template);
            var widgetTransform = widget.transform;
            
            widgetTransform.SetParent(transform, false);
            widgetTransform.localScale = Vector3.one;
            widgetTransform.localPosition = Vector3.zero;
            widget.name = position.ToString();

            var itemData = widget.gameObject.AddComponent<ItemDataContext>();
            itemData.SetContext(item);
            itemData.SetIndex(position);
            widgets.Insert(position, widget);
        }

        private void OnItemRemove(int position) {
            var widget = widgets[position];
            widgets.RemoveAt(position);
            GameObject.Destroy(widget.gameObject);
        }
        
        protected void OnItemsClear() {
            transform.DestroyChildren();
            widgets.Clear();
        }
    }
}
