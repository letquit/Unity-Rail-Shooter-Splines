using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

namespace RailShooter
{
    /// <summary>
    /// 样条路径数据容器。
    /// 用于在 Inspector 中配置路径的切片信息。
    /// </summary>
    [Serializable]
    public class SplinePathData
    {
        public SliceData[] slices; // 路径切片数组
    }

    /// <summary>
    /// 切片数据。
    /// 定义了样条曲线中某一段的范围和状态。
    /// </summary>
    [Serializable]
    public class SliceData
    {
        public int splineIndex; // 对应的样条索引（如果容器中有多个样条）
        public SplineRange range; // 该切片在样条上的范围（0-1）
        
        // 可以存储更多有用的信息
        public bool isEnabled = true; // 是否启用该切片
        public float sliceLength; // 切片长度（运行时计算）
        public float distanceFromStart; // 距离路径起点的累积距离（运行时计算）
    }
    
    /// <summary>
    /// 玩家跟随控制器。
    /// 负责让物体沿着由多个切片组成的样条路径平滑移动。
    /// </summary>
    internal class PlayerFollow : MonoBehaviour
    {
        [SerializeField] private SplineContainer container; // 样条容器组件
        // [SerializeField] private float speed = 0.04f;    // 百分比使用
        [SerializeField] private float speed = 40f; // 移动速度（单位/秒）

        [SerializeField] private SplinePathData pathData; // 路径配置数据

        private SplinePath path; // 当前生成的组合路径

        private float progressRatio; // 当前路径进度 (0.0 到 1.0)
        private float progress; // 当前行进的物理距离
        private float totalLength; // 路径总长度

        private void Start()
        {
            // 1. 计算并生成初始路径
            path = new SplinePath(CalculatePath());
            
            // 2. 启动跟随协程
            StartCoroutine(FollowCoroutine());
        }

        /// <summary>
        /// 计算并组合路径。
        /// 将所有启用的切片拼接成一个完整的路径列表。
        /// </summary>
        /// <returns>样条切片列表</returns>
        private List<SplineSlice<Spline>> CalculatePath()
        {
            // 获取容器的变换矩阵，用于将局部坐标转换为世界坐标
            var localToWorldMatrix = container.transform.localToWorldMatrix;
            
            // 使用 LINQ 筛选出所有启用的切片
            var enabledSlices = pathData.slices.Where(slice => slice.isEnabled).ToList();

            var slices = new List<SplineSlice<Spline>>();

            totalLength = 0f;
            foreach (var sliceData in enabledSlices)
            {
                // 获取对应的样条曲线
                var spline = container.Splines[sliceData.splineIndex];
                
                // 创建样条切片
                var slice = new SplineSlice<Spline>(spline, sliceData.range, localToWorldMatrix);
                slices.Add(slice);
                
                // 计算切片详情（用于调试或逻辑判断）
                sliceData.distanceFromStart = totalLength;
                sliceData.sliceLength = slice.GetLength();
                totalLength += sliceData.sliceLength;
            }

            return slices;
        }

        /// <summary>
        /// 跟随协程。
        /// 负责每一帧更新物体的位置和朝向。
        /// </summary>
        private IEnumerator FollowCoroutine()
        {
            // 无限循环，用于路径重置后的重复播放
            for (var n = 0;; ++n)
            {
                progressRatio = 0f; // 重置进度

                // 单次路径播放循环
                while (progressRatio <= 1f)
                {
                    // 1. 获取当前进度在路径上的世界坐标
                    var pos = path.EvaluatePosition(progressRatio);
                    
                    // 2. 获取当前点的切线方向（用于朝向）
                    var direction = path.EvaluateTangent(progressRatio);

                    // 3. 更新位置
                    transform.position = pos;
                    
                    // 4. 更新朝向，看向移动方向
                    transform.LookAt(pos + direction);
                    
                    // 路径百分比
                    // progressRatio += speed * Time.deltaTime;
                    
                    // 5. 增加进度比率
                    // 公式：(速度 / 总长度) * 时间增量
                    // 这确保了无论路径多长，物体的移动速度（米/秒）是恒定的
                    progressRatio += (speed / totalLength) * Time.deltaTime;
                    
                    // 计算当前行进的物理距离
                    progress = progressRatio * totalLength;
                    
                    yield return null; // 等待下一帧
                }
                
                // --- 路径播放完毕 ---

                // 1. 重置所有切片状态（为下一次循环做准备）
                foreach (var sliceData in pathData.slices)
                {
                    sliceData.isEnabled = true;
                }
                
                // 2. 重新计算路径（如果逻辑中有动态禁用切片的操作，这里会生效）
                path = new SplinePath(CalculatePath());
            }
        }
    }
}