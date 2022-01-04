using UnityEngine;
using UnityEngine.UI;

namespace MiniBricks.Utils {
    public class FpsCounter : MonoBehaviour {
        [SerializeField]
        private Text text;

        private int counter;
        private float lastResetTime;
        
        private void Awake() {
            ResetCounter();
        }
        
        public void Update() {
            var curTime = Time.realtimeSinceStartup;
            if (curTime - lastResetTime >= 1) {
                text.text = counter.ToString();
                ResetCounter();
            }
            
            ++counter;
        }

        private void ResetCounter() {
            counter = 0;
            lastResetTime = Time.realtimeSinceStartup;
        }
    }
}