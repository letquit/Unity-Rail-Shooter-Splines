using UnityEngine;
using UnityEngine.Splines;

namespace RailShooter
{
    /// <summary>
    /// 敌机工厂类。
    /// 提供静态方法用于快速生成标准配置的敌机。
    /// 封装了 EnemyBuilder 的使用细节，简化了调用流程。
    /// </summary>
    public static class EnemyFactory
    {
        /// <summary>
        /// 生成一个标准敌机。
        /// 使用默认配置（如单次路径播放）快速实例化并初始化敌机。
        /// </summary>
        /// <param name="enemyPrefab">敌机预制体</param>
        /// <param name="flightPath">飞行路径（样条曲线）</param>
        /// <param name="enemyParent">敌机在层级中的父节点</param>
        /// <param name="flightPathParent">飞行路径的父节点</param>
        /// <returns>生成好的敌机实例</returns>
        public static Enemy GenerateEnemy(Enemy enemyPrefab, SplineContainer flightPath, Transform enemyParent,
            Transform flightPathParent)
        {
            // 使用敌机构建器进行链式配置
            return new EnemyBuilder()
                .WithPrefab(enemyPrefab)           // 设置预制体
                .WithFlightPath(flightPath)        // 设置飞行路径
                .WithFlightPathParent(flightPathParent) // 设置路径父节点
                .Build(enemyParent);               // 执行构建并返回敌机实例
        }
    }
}