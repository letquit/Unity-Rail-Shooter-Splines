using System;
using KBCore.Refs;
using UnityEngine;

namespace RailShooter
{
    /// <summary>
    /// 武器系统。
    /// 负责处理玩家的瞄准逻辑（包括偏移和回中）以及子弹发射。
    /// </summary>
    public class WeaponSystem : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private InputReader input; // 输入读取器

        [SerializeField] private Transform targetPoint; // 瞄准点（准星实际指向的 3D 位置）
        [SerializeField] private float targetDistance = 50f; // 瞄准平面的距离
        [SerializeField] private float smoothTime = 0.2f; // 瞄准平滑时间
        [SerializeField] private Vector2 aimLimit = new Vector2(50f, 20f); // 瞄准偏移限制
        [SerializeField] private float aimSpeed = 10f; // 瞄准移动速度
        [SerializeField] private float aimReturnSpeed = 0.2f; // 瞄准回中速度

        [SerializeField] private GameObject projectilePrefab; // 子弹预制体
        [SerializeField] private Transform firePoint; // 枪口/发射点
        
        private Vector3 velocity; // 平滑阻尼用的速度缓存
        private Vector2 aimOffset; // 当前的瞄准偏移量

        private void Awake()
        {
            // 订阅射击事件
            input.Fire += OnFire;
        }

        private void Start()
        {
            // 隐藏并锁定鼠标光标，用于 FPS/TPS 视角控制
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            // 1. 计算基础目标位置：玩家前方 targetDistance 处
            Vector3 targetPosition = transform.position + transform.forward * targetDistance;
            
            // 2. 转换为局部坐标，以便应用偏移
            Vector3 localPos = transform.InverseTransformPoint(targetPosition);
            
            // 3. 处理瞄准输入
            if (input.Aim != Vector2.zero)
            {
                // 累加瞄准偏移
                aimOffset += input.Aim * aimSpeed * Time.deltaTime;
                
                // 限制偏移范围，防止瞄得太偏
                aimOffset.x = Mathf.Clamp(aimOffset.x, -aimLimit.x, aimLimit.x);
                aimOffset.y = Mathf.Clamp(aimOffset.y, -aimLimit.y, aimLimit.y);
            }
            else
            {
                // 无输入时，平滑回中
                aimOffset = Vector2.Lerp(aimOffset, Vector2.zero, Time.deltaTime * aimReturnSpeed);
            }
            
            // 4. 应用偏移到局部坐标
            localPos.x += aimOffset.x;
            localPos.y += aimOffset.y;
            
            // 5. 转换回世界坐标
            var desiredPosition = transform.TransformPoint(localPos);
            
            // 6. 平滑移动瞄准点
            targetPoint.position = Vector3.SmoothDamp(targetPoint.position, desiredPosition, ref velocity, smoothTime);
        }

        /// <summary>
        /// 处理射击逻辑。
        /// </summary>
        private void OnFire()
        {
            // 计算枪口指向瞄准点的旋转
            var rotation = Quaternion.LookRotation(targetPoint.position - firePoint.position);
            
            // 实例化子弹
            var projectile = Instantiate(projectilePrefab, firePoint.position, rotation);
            
            // 5秒后销毁子弹，防止场景中存在过多无用物体
            Destroy(projectile, 5f);
        }

        private void OnDestroy()
        {
            // 销毁时取消订阅
            input.Fire -= OnFire;
        }
    }
}