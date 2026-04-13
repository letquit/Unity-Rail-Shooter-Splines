using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RailShooter
{
    [Serializable]
    public class Annulus
    {
        public float distance;  // 圆环中心距离生成器的距离
        public float innerRadius;   // 圆环的内半径
        public float outerRadius;   // 圆环的外半径

        public Vector3 GetRandomPoint()
        {
            // 0 到 180 度之间的随机角度
            float angle = Random.Range(0f, Mathf.PI);   // 完整圆圈为 2PI
            
            // 内半径和外半径之间的随机半径
            float radius = Random.Range(innerRadius, outerRadius);
            
            // 计算点的 x 和 y 坐标
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);
            
            // 返回该点
            return new Vector3(x, y, distance);
        }
    }
}