using MiniBricks.Tetris;
using UnityEngine;
using UnityEngine.Serialization;

namespace MiniBricks {
    public class TestApplication : MonoBehaviour, IPieceDef, ITowerDef {
        [SerializeField]
        private float moveStep;
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
            
            public Piece Create(Vector2 position) {
                var i = Random.Range(0, application.piecePrefabs.Length);
                var prefab = application.piecePrefabs[i];
                var result = Instantiate(prefab, position, Quaternion.identity, application.transform);
                result.Initialize(application);
                return result;
            }
        }
        
        
        public float MoveStep => moveStep;
        public float SpawnHeight => spawnHeight;

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
