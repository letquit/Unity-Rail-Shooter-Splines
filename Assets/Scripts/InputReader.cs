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

        private float lastMoveTime;
        private float lastMoveDirection;

        public event Action LeftTab;
        public event Action RightTab;
        
        public Vector2 Move => moveAction.ReadValue<Vector2>();

        private void Awake()
        {
            moveAction = playerInput.actions["Move"];
        }

        private void OnEnable()
        {
            moveAction.performed += OnMovePerformed;
        }
        
        private void OnDisable()
        {
            moveAction.performed -= OnMovePerformed;
        }

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
