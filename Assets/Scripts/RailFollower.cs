using System;
using UnityEngine;

namespace RailShooter
{
    /// <summary>
    /// 轨道跟随器。
    /// 通常用于摄像机，使其平滑地跟随目标移动，并同步玩家的旋转。
    /// </summary>
    public class RailFollower : MonoBehaviour
    {
        [SerializeField] private Transform player; // 玩家变换（用于同步旋转）
        [SerializeField] private Transform followTarget; // 跟随的目标（通常是玩家或空物体）
        [SerializeField] private float followDistance = 22f; // 与目标的跟随距离
        [SerializeField] private float smoothTime = 0.2f; // 平滑过渡时间（越小越快到达目标）

        private Vector3 velocity; // 平滑阻尼用的速度缓存

        private void Update()
        {
            // 1. 计算目标位置：目标后方指定距离处
            Vector3 targetPos = followTarget.position + followTarget.forward * -followDistance;
            
            // 2. 使用 SmoothDamp 平滑移动摄像机位置
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
            
            // 3. 设置摄像机的旋转以匹配玩家的旋转
            // 这样摄像机不仅跟随位置，还会跟随玩家的朝向（如转弯时的倾斜）
            transform.rotation = player.rotation;
        }
    }
}