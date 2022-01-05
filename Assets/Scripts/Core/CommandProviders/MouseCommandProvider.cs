using MiniBricks.Core.Logic.Commands;
using MiniBricks.Core.Logic.Interfaces;
using MiniBricks.Core.View;
using UnityEngine;

namespace MiniBricks.Core.CommandProviders {
    public class MouseCommandProvider : MonoBehaviour, ICommandProvider {
        [SerializeField]
        private float horizontalSensitivity = 1;
        [SerializeField]
        private float verticalSensitivity = 2;
        
        private float horizontalStep;
        private float verticalStep;
        private int towerId;
        private Vector3 pressPos;
        private bool wasPressed;
        private bool wasDragged;
        
        public MouseCommandProvider Initialize(int towerId, CameraView camera) {
            this.towerId = towerId;
            horizontalStep = camera.GetPixelsPerUnit() * horizontalSensitivity;
            verticalStep = camera.GetPixelsPerUnit() * verticalSensitivity;
            return this;
        }
        
        public ICommand GetNextCommand() {
            var curPos = Input.mousePosition;
            bool isPressed = Input.GetMouseButton(0);
            
            if (isPressed && !wasPressed) {
                pressPos = curPos;
            }
            ICommand result = CalculateCommand(curPos, isPressed);
            if (!isPressed && wasPressed) {
                wasDragged = false;
            }

            wasPressed = isPressed;
            
            return result;
        }

        private ICommand CalculateCommand(Vector3 curPos, bool isPressed) {
            if (!isPressed && wasPressed && !wasDragged) {
                return new RotateCommand(towerId);
            }

            if (!isPressed) {
                return null;
            }

            var delta = curPos - pressPos;
            bool hDragged = Mathf.Abs(delta.x) >= horizontalStep;
            bool vDraggeed = Mathf.Abs(delta.y) >= verticalStep;

            if (vDraggeed || hDragged) {
                wasDragged = true;
                pressPos = curPos;
            }
            
            if (hDragged) {
                if (delta.x < 0) {
                    return new LeftCommand(towerId);
                } else {
                    return new RightCommand(towerId);
                }
            }

            if (vDraggeed) {
                if (delta.y < 0) {
                    return new StartAccelerateCommand(towerId);
                } else {
                    return new StopAccelerateCommand(towerId);
                }
            }

            return null;
        }
    }
}