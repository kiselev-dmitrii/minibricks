using MiniBricks.Core.Logic;
using MiniBricks.Core.Logic.Interfaces;
using UnityEngine;

namespace MiniBricks.Core.View {
    public class TowerView : MonoBehaviour {
        [SerializeField]
        private Tower tower;

        [SerializeField]
        private Transform highlighting;
        [SerializeField]
        private Transform finishLine;
        [SerializeField]
        private GameObject spawnEffectPrefab;

        private Game game;
        
        public void Initialize(Game game) {
            this.game = game;
            
            UpdateFinishLine();
            UpdateHighlight();
            
            tower.PieceSpawned += OnPieceSpawned;
            game.CommandExecuted += OnCommandExecuted;
            game.TargetHeightChanged += OnTargetHeightChanged;
        }

        public void OnDestroy() {
            tower.PieceSpawned -= OnPieceSpawned;
            game.CommandExecuted -= OnCommandExecuted;
            game.TargetHeightChanged -= OnTargetHeightChanged;
        }
        
        private void OnTargetHeightChanged() {
            UpdateFinishLine();
        }

        private void OnCommandExecuted(ICommand cmd) {
            if (cmd.TowerId != tower.Id) {
                return;
            }
            UpdateHighlight();
        }

        private void OnPieceSpawned(Piece piece) {
            PlaySpawnEffect(piece.GetPosition());
            UpdateHighlight();
        }

        private void PlaySpawnEffect(Vector3 position) {
            GameObject.Instantiate(spawnEffectPrefab, position, Quaternion.identity, transform);
        }

        private void UpdateHighlight() {
            var currentPiece = tower.GetCurrentPiece();
            if (currentPiece == null) {
                return;
            }
            
            var piecePos = currentPiece.GetPosition();

            var size = currentPiece.GetSize();
            
            var hPos = highlighting.position;
            var hScale = highlighting.localScale;
            
            highlighting.position = new Vector3(hPos.x, piecePos.y, hPos.z);
            highlighting.localScale = new Vector3(size.x, hScale.y, hScale.z);
            
            highlighting.position = currentPiece.GetPosition();
        }
        
        private void UpdateFinishLine() {
            var targetHeight = game.TargetHeight;
    
            var platformPoint = tower.GetPlatform().GetTopPoint();
            var finishPoint = platformPoint + Vector3.up * targetHeight;
            finishLine.localPosition = finishPoint;
        } 
    }
}