using System;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RailShooter
{
    /// <summary>
    /// 输入读取器。
    /// 负责封装 Unity 输入系统的细节，向外部提供简化的输入事件和状态。
    /// 包含双击/快速变向检测逻辑。
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class InputReader : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private PlayerInput playerInput; // Unity 输入系统组件（自动获取）
        [SerializeField] private float doubleTabTime = 0.5f; // 双击判定的时间阈值（秒）
        
        private InputAction moveAction; // 移动输入动作
        private InputAction aimAction; // 瞄准输入动作
        private InputAction fireAction; // 射击输入动作

        private float lastMoveTime; // 上一次移动输入的时间
        private float lastMoveDirection; // 上一次移动的方向（-1, 0, 1）

        // 事件定义：供外部订阅
        public event Action LeftTab;  // 向左双击/快速变向事件
        public event Action RightTab; // 向右双击/快速变向事件
        public event Action Fire;     // 射击事件
        
        // 属性：供外部读取当前状态
        public Vector2 Move => moveAction.ReadValue<Vector2>(); // 当前移动向量
        public Vector2 Aim => aimAction.ReadValue<Vector2>();   // 当前瞄准向量

        private void Awake()
        {
            // 初始化动作引用
            moveAction = playerInput.actions["Move"];
            aimAction = playerInput.actions["Aim"];
            fireAction = playerInput.actions["Fire"];
        }

        private void OnEnable()
        {
            // 启用时订阅输入事件
            moveAction.performed += OnMovePerformed;
            fireAction.performed += OnFire;
        }
        
        private void OnDisable()
        {
            // 禁用时取消订阅，防止内存泄漏
            moveAction.performed -= OnMovePerformed;
            fireAction.performed -= OnFire;
        }

        /// <summary>
        /// 处理射击输入。
        /// </summary>
        private void OnFire(InputAction.CallbackContext ctx) => Fire?.Invoke();

        /// <summary>
        /// 处理移动输入，并检测“双击”或“快速变向”操作。
        /// </summary>
        private void OnMovePerformed(InputAction.CallbackContext ctx)
        {
            // 获取当前移动方向的 X 轴（-1 左, 1 右）
            float currentDirection = Move.x;

            // 检测逻辑：
            // 1. 当前时间与上次输入时间的差值小于阈值
            // 2. 当前方向与上次方向一致（即快速向同一方向推摇杆/按键）
            if (Time.time - lastMoveTime < doubleTabTime && currentDirection == lastMoveDirection)
            {
                // 触发对应的特殊事件
                if (currentDirection < 0)
                {
                    LeftTab?.Invoke();
                }
                else if (currentDirection > 0)
                {
                    RightTab?.Invoke();
                }
            }

            // 更新记录状态
            lastMoveTime = Time.time;
            lastMoveDirection = currentDirection;
        }
    }
}