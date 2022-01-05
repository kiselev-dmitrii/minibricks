using MiniBricks.Core.Logic;
using MiniBricks.Core.View;
using UnityEngine;

namespace MiniBricks.CompositionRoot {
    [CreateAssetMenu(menuName = "Game/GameDef")]
    public class ApplicationDef : ScriptableObject {
        public GameSettings GameSettings;
        public Platform[] Platforms;
        public CameraView Camera;
    }
}