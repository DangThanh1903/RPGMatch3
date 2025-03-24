using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Match3 {
    public class InputReader : MonoBehaviour{
        [SerializeField] PlayerInput playerInput;
        InputAction selectedAction;
        InputAction fireAction;

        public event Action Fire;

        public Vector2 Selected => selectedAction.ReadValue<Vector2>();

        void Start() {
            playerInput = GetComponent<PlayerInput>();
            selectedAction = playerInput.actions["Select"];
            fireAction = playerInput.actions["Fire"];

            fireAction.performed += OnFire;
        }

        void StopSwap() {
            fireAction.performed -= OnFire;
        }

        void OnFire(InputAction.CallbackContext obj) => Fire?.Invoke();
    }
}