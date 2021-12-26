using KiselevDmitry.Utils;
using NData;
using UnityEngine;

namespace KiselevDmitry.NData.Bindings {
    public class GridBinding : Binding {
        public RectTransform Template;        
        private Collection collection;

        protected override void Bind() {
            var context = GetContext(Path);
            if (context == null) return;

            collection = context.FindCollection(Path, this);
            if (collection == null) return;

            collection.OnItemInsert += OnItemInsert;
            collection.OnItemsClear += OnItemsClear;
            for (var i = 0; i < collection.ItemsCount; ++i) {
                OnItemInsert(i, collection.GetBaseItem(i));
            }
        }

        protected override void Unbind() {
            if (collection == null) return;
            collection.OnItemInsert -= OnItemInsert;
            collection.OnItemsClear -= OnItemsClear;
            collection = null;
            OnItemsClear();
        }

        protected void OnItemInsert(int position, Context item) {
            var widget = Instantiate(Template);
            widget.transform.SetParent(transform, false);
            widget.transform.localScale = Vector3.one;
            widget.transform.localPosition = Vector3.zero;
            widget.name = position.ToString();

            var itemData = widget.gameObject.AddComponent<ItemDataContext>();
            itemData.SetContext(item);
            itemData.SetIndex(position);
        }

        protected void OnItemsClear() {
            transform.DestroyChildren();
        }
    }
}
