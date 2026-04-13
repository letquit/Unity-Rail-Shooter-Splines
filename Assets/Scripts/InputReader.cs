using System;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RailShooter
{
    [RequireComponent(typeof(PlayerController))]
    public class InputReader : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private PlayerInput playerInput;
        [SerializeField] private float doubleTabTime = 0.5f;
        
        private InputAction moveAction;
        private InputAction aimAction;
        private InputAction fireAction;

        private float lastMoveTime;
        private float lastMoveDirection;

        public event Action LeftTab;
        public event Action RightTab;
        public event Action Fire;
        
        public Vector2 Move => moveAction.ReadValue<Vector2>();
        public Vector2 Aim => aimAction.ReadValue<Vector2>();

        private void Awake()
        {
            moveAction = playerInput.actions["Move"];
            aimAction = playerInput.actions["Aim"];
            fireAction = playerInput.actions["Fire"];
        }

        private void OnEnable()
        {
            moveAction.performed += OnMovePerformed;
            fireAction.performed += OnFire;
        }
        
        private void OnDisable()
        {
            moveAction.performed -= OnMovePerformed;
            fireAction.performed -= OnFire;
        }

        private void OnFire(InputAction.CallbackContext ctx) => Fire?.Invoke();

        private void OnMovePerformed(InputAction.CallbackContext ctx)
        {
            float currentDirection = Move.x;

            if (Time.time - lastMoveTime < doubleTabTime && currentDirection == lastMoveDirection)
            {
                if (currentDirection < 0)
                {
                    LeftTab?.Invoke();
                }
                else if (currentDirection > 0)
                {
                    RightTab?.Invoke();
                }
            }

            lastMoveTime = Time.time;
            lastMoveDirection = currentDirection;
        }
    }
}
