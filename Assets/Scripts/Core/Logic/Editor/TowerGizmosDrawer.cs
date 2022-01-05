using UnityEditor;
using UnityEngine;

namespace MiniBricks.Core.Logic {
    public static class TowerGizmosDrawer {
        private const float maxHeightLineWidth = 20;
        
        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
        public static void DrawGizmo(Tower tower, GizmoType gizmoType) {
            DrawMaxHeight(tower);
        }

        private static void DrawMaxHeight(Tower tower) {
            var bottom = tower.GetBottomPoint();
            var maxHeightPoint = bottom + Vector3.up * tower.MaxHeight;

            var half = maxHeightLineWidth / 2;
            var start = maxHeightPoint - Vector3.right * half;
            var end = maxHeightPoint + Vector3.right * half;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(start, end);
        }
    }
}