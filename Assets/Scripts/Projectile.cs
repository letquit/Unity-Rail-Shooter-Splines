using System;
using UnityEngine;

namespace RailShooter
{
    /// <summary>
    /// 子弹/抛射物类。
    /// 负责处理子弹的直线飞行逻辑。
    /// 假设子弹的 Z 轴正方向为其飞行方向。
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 10f; // 飞行速度（单位/秒）

        private void Update()
        {
            // 沿物体自身的 Z 轴正方向移动
            // speed * Time.deltaTime 计算这一帧应该移动的距离，确保移动与帧率无关
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
    }
}