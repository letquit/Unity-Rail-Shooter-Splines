using System;
using Shapes;
using UnityEngine;
using Utilities;

namespace RailShooter
{
    /// <summary>
    /// 敌机生成管理器。
    /// 负责根据预设的圆环区域（Annulus）定时生成敌机及其飞行路径。
    /// 包含用于在 Scene 视图中可视化生成区域的调试代码。
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Annulus[] annuli; // 定义敌机生成的圆环区域数组（逻辑数据）
        [SerializeField] private Disc[] discs; // 对应的调试圆盘数组（用于可视化显示）

        [SerializeField] private Enemy enemyPrefab; // 敌机预制体
        [SerializeField] private float spawnInterval = 5f; // 生成间隔时间（秒）

        [SerializeField] private Transform enemyParent; // 生成的敌机的父节点
        [SerializeField] private Transform flightPathParent; // 生成的飞行路径的父节点

        private float spawnTimer; // 生成计时器
        
        // 仅使用形状进行调试
        private void Start()
        {
            // 将逻辑数据（Annulus）同步到调试组件（Disc）上，以便在编辑器中可视化
            for (var i = 0; i < annuli.Length; i++)
            {
                // 设置圆盘位置：使用扩展方法 With 仅修改 Z 轴，保持 X/Y 与生成器一致
                discs[i].transform.position = transform.position.With(z: annuli[i].distance);
                
                // 设置圆盘半径：外半径决定圆盘大小
                discs[i].Radius = annuli[i].outerRadius;
                
                // 设置圆盘厚度：外半径减内半径，形成圆环视觉效果
                discs[i].Thickness = annuli[i].outerRadius - annuli[i].innerRadius;
            }
        }

        private void Update()
        {
            // 检查计时器是否超过设定间隔
            if (spawnTimer > spawnInterval)
            {
                spawnTimer = 0f; // 重置计时器
                SpawnEnemy(); // 触发敌机生成
            }

            spawnTimer += Time.deltaTime; // 累加时间
        }

        /// <summary>
        /// 生成单个敌机。
        /// 协调路径工厂和敌机工厂来完成生成工作。
        /// </summary>
        private void SpawnEnemy()
        {
            // 1. 使用路径工厂基于圆环区域生成随机的飞行路径
            var flightPath = FlightPathFactory.GenerateFlightPath(annuli, flightPathParent);
            
            // 2. 使用敌机工厂沿着该路径生成敌机
            EnemyFactory.GenerateEnemy(enemyPrefab, flightPath, enemyParent, flightPathParent);
        }
    }
}