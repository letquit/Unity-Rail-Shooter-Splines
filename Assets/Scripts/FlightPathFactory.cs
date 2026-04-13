using UnityEngine;
using UnityEngine.Splines;

namespace RailShooter
{
    /// <summary>
    /// 飞行路径工厂。
    /// 负责根据一组圆环区域（Annulus）动态生成随机的样条曲线路径。
    /// 生成的路径将作为敌机的飞行轨道。
    /// </summary>
    public static class FlightPathFactory
    {
        /// <summary>
        /// 生成一条随机的飞行路径。
        /// </summary>
        /// <param name="annuli">定义路径关键点的圆环区域数组</param>
        /// <param name="parent">路径的父节点，路径将在其局部空间内生成</param>
        /// <returns>包含生成样条曲线的 SplineContainer 组件</returns>
        public static SplineContainer GenerateFlightPath(Annulus[] annuli, Transform parent)
        {
            // 1. 创建一个新的 GameObject 作为路径容器
            var flightPath = new GameObject("Flight Path");
            
            // 2. 设置层级关系和 Transform 属性
            if (parent != null)
            {
                // 设置父节点（false 表示保持世界坐标不变，但这里紧接着重置了局部坐标）
                flightPath.transform.SetParent(parent, false);
                
                // 强制重置局部位置和旋转，确保路径从父节点的 (0,0,0) 开始且无旋转
                flightPath.transform.localPosition = Vector3.zero;
                flightPath.transform.localRotation = Quaternion.identity;
            }

            // 3. 添加 SplineContainer 组件并创建一个新的样条
            var container = flightPath.AddComponent<SplineContainer>();
            var spline = container.AddSpline();

            // 4. 准备贝塞尔节点数组
            var knots = new BezierKnot[annuli.Length];
            
            for (int i = 0; i < annuli.Length; i++)
            {
                // 从当前圆环区域获取一个随机点
                Vector3 localPoint = annuli[i].GetRandomPoint();
                
                // 创建贝塞尔节点
                // 参数说明：
                // localPoint: 节点位置
                // -30 * Vector3.forward: 入切线（控制曲线进入该点的曲率）
                // 30 * Vector3.forward: 出切线（控制曲线离开该点的曲率）
                // 这里硬编码 Z 轴切线是为了保证路径在深度方向上的平滑性
                knots[i] = new BezierKnot(localPoint, -30 * Vector3.forward, 30 * Vector3.forward);
            }

            // 5. 将节点数组赋值给样条，完成路径生成
            spline.Knots = knots;
            
            return container;
        }
    }
}