using System;
using DG.Tweening;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.Serialization;

namespace RailShooter
{
    /// <summary>
    /// 玩家控制器。
    /// 处理玩家的移动、跟随、转向以及桶滚特技。
    /// 依赖 InputReader 获取输入指令。
    /// </summary>
    public class PlayerController : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private InputReader input; // 输入读取器（自动获取）
        
        [SerializeField] private Transform followTarget; // 跟随的目标（通常是摄像机或空物体）
        [SerializeField] private Transform aimTarget; // 瞄准的目标点（用于控制朝向）
        
        [SerializeField] private Transform playerModel; // 玩家模型（用于动画）
        [SerializeField] private float followDistance = 2f; // 与跟随目标的距离
        [SerializeField] private Vector2 movementLimit = new Vector2(2f, 2f); // 移动范围限制 (X, Y)
        [SerializeField] private float movementSpeed = 10f; // 移动速度
        [SerializeField] private float smoothTime = 0.2f; // 跟随平滑时间

        [SerializeField] private float maxRoll; // 最大倾斜角度
        [SerializeField] private float rollSpeed = 2f; // 倾斜速度
        [SerializeField] private float rollDuration = 1f; // 桶滚动画时长

        [SerializeField] private Transform modelParent; // 模型父节点（用于控制朝向）
        [SerializeField] private float rotationSpeed = 5f; // 转向速度
        
        private Vector3 velocity; // 平滑跟随用的速度缓存
        private float roll; // 当前的倾斜角度

        private void Awake()
        {
            // 订阅输入事件（双击/快速变向）
            input.LeftTab += OnLeftTab;
            input.RightTab += OnRightTab;
        }

        private void Update()
        {
            // 每一帧更新玩家状态
            HandlePosition(); // 处理位置移动
            HandleRoll();     // 处理倾斜
            HandleRotation(); // 处理朝向
        }

        /// <summary>
        /// 处理玩家模型的旋转，使其始终面向瞄准目标。
        /// </summary>
        private void HandleRotation()
        {
            // 计算从玩家指向瞄准目标的方向
            Vector3 direction = aimTarget.position - transform.position;
            
            // 计算看向该方向所需的旋转四元数
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            
            // 使用 Lerp 平滑插值旋转，避免瞬间转身
            modelParent.rotation =
                Quaternion.Lerp(modelParent.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        /// <summary>
        /// 处理玩家的位置更新和移动逻辑。
        /// </summary>
        private void HandlePosition()
        {
            // 1. 计算理想跟随位置（目标后方指定距离）
            Vector3 targetPos = followTarget.position + followTarget.forward * -followDistance;
            
            // 2. 使用 SmoothDamp 实现平滑的摄像机/玩家跟随效果
            Vector3 smoothedPos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
            
            // 3. 将平滑后的世界坐标转换为相对于跟随目标的局部坐标
            // 这样移动就是相对于目标的，而不是绝对的世界坐标
            Vector3 localPos = transform.InverseTransformPoint(smoothedPos);
            
            // 4. 根据输入在局部坐标系中移动
            localPos.x += input.Move.x * movementSpeed * Time.deltaTime;
            localPos.y += input.Move.y * movementSpeed * Time.deltaTime;
            
            // 5. 限制移动范围，防止玩家飞出太远
            localPos.x = Mathf.Clamp(localPos.x, -movementLimit.x, movementLimit.x);
            localPos.y = Mathf.Clamp(localPos.y, -movementLimit.y, movementLimit.y);
            
            // 6. 将限制后的局部坐标转换回世界坐标并应用
            transform.position = transform.TransformPoint(localPos);
        }

        /// <summary>
        /// 处理玩家的倾斜效果（模拟飞机转弯）。
        /// </summary>
        private void HandleRoll()
        {
            // 基础旋转与跟随目标保持一致
            transform.rotation = followTarget.rotation;
            
            // 根据移动方向（input.Move.x）计算目标倾斜角度
            // 使用 Lerp 平滑过渡当前倾斜角度
            roll = Mathf.Lerp(roll, input.Move.x * maxRoll, Time.deltaTime * rollSpeed);
            
            // 应用倾斜旋转（只修改 Z 轴）
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, roll);
        }

        private void OnLeftTab() => BarrelRoll(); // 向左桶滚
        
        private void OnRightTab() => BarrelRoll(1); // 向右桶滚

        /// <summary>
        /// 执行桶滚动画。
        /// </summary>
        /// <param name="direction">方向：-1 为左，1 为右</param>
        private void BarrelRoll(int direction = -1)
        {
            // 检查是否正在播放动画，防止重叠
            if (!DOTween.IsTweening(playerModel))
            {
                // 使用 DOTween 进行局部旋转
                // RotateMode.LocalAxisAdd: 在原有角度基础上累加旋转
                playerModel
                    .DOLocalRotate(
                        new Vector3(playerModel.localEulerAngles.x, playerModel.localEulerAngles.y, 360 * direction), rollDuration,
                        RotateMode.LocalAxisAdd).SetEase(Ease.OutCubic);
            }
        }
        
        private void OnDestroy()
        {
            // 销毁时取消订阅，防止内存泄漏
            input.LeftTab -= OnLeftTab;
            input.RightTab -= OnRightTab;
        }
    }
}