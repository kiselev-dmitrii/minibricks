using UnityEditor;
using UnityEngine;

namespace MiniBricks.Core.Logic {
    [CustomEditor(typeof(Tower))]
    public class TowerInspector : Editor {
        private Tower tower;
        
        public void OnEnable() {
            tower = (Tower)target;
        }

        public void OnSceneGUI() {
            DrawMaxHeightLine();
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            
            if (GUILayout.Button("Add Live")) {
                tower.AddLives(1);
            }
                        
            if (GUILayout.Button("Activate")) {
                tower.Activate();
            }
            
            if (GUILayout.Button("Deactivate")) {
                tower.Deactivate();
            }
        }

        private void DrawMaxHeightLine() {

        }
    }
}