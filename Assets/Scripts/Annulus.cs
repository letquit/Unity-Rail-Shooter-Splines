using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RailShooter
{
    /// <summary>
    /// 圆环区域生成器。
    /// 用于在指定的扇形环形区域内生成随机的三维坐标点。
    /// 常用于轨道射击游戏中控制敌机或道具的生成位置，
    /// 确保它们只出现在玩家前方（0-180度）且距离适中。
    /// </summary>
    [Serializable]
    public class Annulus
    {
        [Tooltip("圆环平面距离生成中心（通常是摄像机）的Z轴深度")]
        public float distance;

        [Tooltip("圆环的内半径，用于防止物体生成在中心点过近的位置")]
        public float innerRadius;

        [Tooltip("圆环的外半径，用于限制物体生成的最大范围")]
        public float outerRadius;

        /// <summary>
        /// 获取圆环区域内的一个随机世界坐标点。
        /// 使用极坐标转换计算 X 和 Y 轴，Z 轴固定为 distance。
        /// </summary>
        /// <returns>随机生成的 Vector3 坐标</returns>
        public Vector3 GetRandomPoint()
        {
            // 1. 生成随机角度
            // 范围 0 到 PI (180度)，对应屏幕前方的上半圆区域
            // 完整圆圈为 2 * PI (360度)
            float angle = Random.Range(0f, Mathf.PI);

            // 2. 生成随机半径
            // 在内半径和外半径之间取值，确保物体不会太靠近中心或超出边界
            float radius = Random.Range(innerRadius, outerRadius);

            // 3. 极坐标转笛卡尔坐标
            // 计算 X 和 Y 轴的平面位置
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);

            // 4. 返回最终的三维坐标
            // Z 轴固定为设定的深度距离
            return new Vector3(x, y, distance);
        }
    }
}