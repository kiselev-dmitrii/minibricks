using MiniBricks.Tetris;
using UnityEngine;

namespace MiniBricks {
    public class TestApplication : MonoBehaviour, IPieceDef, ITowerDef {
        [SerializeField]
        private float moveStep;
        [SerializeField]
        private float fallSpeed;
        [SerializeField]
        private float spawnHeight;
        [SerializeField]
        private Tower tower;
        [SerializeField]
        private Piece[] piecePrefabs;

        private class PieceFactory : IPieceFactory {
            private readonly TestApplication application;

            public PieceFactory(TestApplication application) {
                this.application = application;
            }
            
            public Piece Create(Piece prefab, Vector2 position) {
                var result = Instantiate(prefab, position, Quaternion.identity, application.transform);
                result.Initialize(application);
                return result;
            }
        }
        
        
        public float MoveStep => moveStep;
        public float FallSpeed => fallSpeed;
        public float SpawnHeight => spawnHeight;
        public Piece[] PiecePrefabs => piecePrefabs;

        public void Start() {
            var pieceFactory = new PieceFactory(this);
            tower.Initialize(this, pieceFactory);
            tower.Run();
        }

        public void Update() {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                tower.Move(-1);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow)) {
                tower.Move(1);
            }
            
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                tower.Rotate();
            }
        }
    }
}
