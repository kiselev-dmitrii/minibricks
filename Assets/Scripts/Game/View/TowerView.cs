using MiniBricks.Controllers;
using MiniBricks.Game.Entities;
using UnityEngine;

namespace MiniBricks.Tetris {
    public class TowerView : MonoBehaviour {
        [SerializeField]
        private Tower tower;
        [SerializeField]
        private Transform finishLine;

        private MultiplayerGame game;
        
        public void Initialize(MultiplayerGame game) {
            this.game = game;
            ConfigureFinishLine();
        }

        private void ConfigureFinishLine() {
            var targetHeight = game.GetTargetHeight();
    
            var platformPoint = tower.GetPlatform().GetTopPoint();
            var finishPoint = platformPoint + Vector3.up * targetHeight;
            finishLine.localPosition = finishPoint;
        } 
    }
}