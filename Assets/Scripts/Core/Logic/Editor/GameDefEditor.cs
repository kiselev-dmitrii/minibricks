using UnityEditor;
using UnityEngine;

namespace MiniBricks.Core.Logic {
    public class GameDefEditor : EditorWindow {
        [MenuItem("Tools/Game/GameDef Editor")]
        public static void Create() {
            var self = (GameDefEditor)EditorWindow.GetWindow(typeof(GameDefEditor));
            self.titleContent = new GUIContent("GameDef Editor"); 
            self.Show();
        }
        
        public void Update() {
            Repaint();
        }
        
        private void OnGUI() {
            var game = Game.Current;
            if (game == null) {
                return;
            }

            DrawGame(game);
        }
        
        private void DrawGame(Game game) {
            EditorGUILayout.BeginVertical("Box");
            game.TargetHeight = Mathf.Clamp(EditorGUILayout.IntField("Required Height", game.TargetHeight), 10, 100);
            game.Gravity = EditorGUILayout.Slider("Gravity", game.Gravity, -20, 0);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginHorizontal();
            foreach (var tower in game.Towers) {
                DrawTower(tower);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawTower(Tower tower) {
            var originalColor = GUI.color;
            if (tower.IsActive()) {
                GUI.color = Color.green;
            }
            
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.LabelField($"Tower {tower.Id}");
            EditorGUILayout.FloatField("Reached height", tower.MaxHeight);
            tower.NumLives = Mathf.Clamp(EditorGUILayout.IntField("Num Lives", tower.NumLives), 0, 10);
            tower.SpawnHeight = EditorGUILayout.Slider("Spawn Height", tower.SpawnHeight, 0, 20);
            EditorGUILayout.EndVertical();

            GUI.color = originalColor;
        }
    }
}