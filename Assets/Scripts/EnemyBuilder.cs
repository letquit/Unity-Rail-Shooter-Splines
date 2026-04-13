using UnityEngine;
using UnityEngine.Splines;

namespace RailShooter
{
    /// <summary>
    /// 敌机构建器。
    /// 采用建造者模式（Builder Pattern），通过链式调用流畅地配置并生成敌机。
    /// 封装了复杂的组件初始化逻辑（如路径动画设置）。
    /// </summary>
    public class EnemyBuilder
    {
        private Transform flightPathParent; // 飞行路径的父节点（用于层级管理）
        private Enemy enemyPrefab; // 敌机预制体
        private SplineContainer flightPath; // 飞行路径数据（样条曲线）
        private SplineAnimate.LoopMode loopMode = SplineAnimate.LoopMode.Once; // 路径循环模式（默认为只走一次）

        /// <summary>
        /// 设置飞行路径的父节点。
        /// </summary>
        /// <param name="flightPathParent">父节点变换</param>
        /// <returns>返回当前构建器实例，支持链式调用</returns>
        public EnemyBuilder WithFlightPathParent(Transform flightPathParent)
        {
            this.flightPathParent = flightPathParent;
            return this;
        }

        /// <summary>
        /// 设置敌机预制体。
        /// </summary>
        /// <param name="enemyPrefab">敌机预制体</param>
        /// <returns>返回当前构建器实例，支持链式调用</returns>
        public EnemyBuilder WithPrefab(Enemy enemyPrefab)
        {
            this.enemyPrefab = enemyPrefab;
            return this;
        }

        /// <summary>
        /// 设置飞行路径。
        /// </summary>
        /// <param name="flightPath">样条曲线容器</param>
        /// <returns>返回当前构建器实例，支持链式调用</returns>
        public EnemyBuilder WithFlightPath(SplineContainer flightPath)
        {
            this.flightPath = flightPath;
            return this;
        }

        /// <summary>
        /// 设置路径循环模式。
        /// </summary>
        /// <param name="loopMode">循环模式（如 Once, Loop, PingPong）</param>
        /// <returns>返回当前构建器实例，支持链式调用</returns>
        public EnemyBuilder WithLoopMode(SplineAnimate.LoopMode loopMode)
        {
            this.loopMode = loopMode;
            return this;
        }

        /// <summary>
        /// 执行构建：实例化敌机并配置其组件。
        /// </summary>
        /// <param name="enemyParent">敌机的父节点（通常是生成管理器或空物体）</param>
        /// <returns>配置完成的敌机实例</returns>
        public Enemy Build(Transform enemyParent)
        {
            // 1. 实例化敌机预制体，并设置父节点
            var enemy = Object.Instantiate(enemyPrefab, enemyParent);
            
            // 2. 将飞行路径数据赋值给敌机脚本
            enemy.FlightPath = flightPath;

            // 3. 如果配置了飞行路径，则初始化动画组件
            if (flightPath != null)
            {
                var splineAnimate = enemy.GetComponent<SplineAnimate>();
                
                // 指定路径容器
                splineAnimate.Container = flightPath;
                
                // 设置循环模式
                splineAnimate.Loop = loopMode;
                
                // 重置动画时间到起点（确保每次生成都是从0开始）
                splineAnimate.ElapsedTime = 0f;
                
                // 开始播放路径动画
                splineAnimate.Play();
            }

            return enemy;
        }
    }
}