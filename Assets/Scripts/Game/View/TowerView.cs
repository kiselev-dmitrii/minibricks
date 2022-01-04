using MiniBricks.Controllers;
using MiniBricks.Game.Entities;
using UnityEngine;

namespace MiniBricks.Tetris {
    public class TowerView : MonoBehaviour {
        [SerializeField]
        private Tower tower;

        [SerializeField]
        private Transform highlighting;
        [SerializeField]
        private Transform finishLine;

        private MultiplayerGame game;
        
        public void Initialize(MultiplayerGame game) {
            this.game = game;
            ConfigureFinishLine();
        }

        private void Update() {
            var currentPiece = tower.GetCurrentPiece();
            var piecePos = currentPiece.GetPosition();

            var size = currentPiece.GetSize();
            
            var hPos = highlighting.position;
            var hScale = highlighting.localScale;
            
            highlighting.position = new Vector3(hPos.x, piecePos.y, hPos.z);
            highlighting.localScale = new Vector3(size.x, hScale.y, hScale.z);
            
            highlighting.position = currentPiece.GetPosition();
        }

        private void ConfigureFinishLine() {
            var targetHeight = game.GetTargetHeight();
    
            var platformPoint = tower.GetPlatform().GetTopPoint();
            var finishPoint = platformPoint + Vector3.up * targetHeight;
            finishLine.localPosition = finishPoint;
        } 
    }
}