using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Vector3 扩展方法类。
    /// 提供用于简化 Vector3 结构体操作的辅助方法，
    /// 支持不可变数据风格的向量的修改与计算。
    /// </summary>
    public static class Vector3Extensions
    {
        /// <summary>
        /// 创建一个新的 Vector3，并选择性替换指定的轴数值。
        /// 未指定的轴将保持原向量的数值。
        /// 适用于函数式编程风格，避免直接修改原结构体。
        /// </summary>
        /// <param name="vector">原始向量（扩展方法的目标对象）</param>
        /// <param name="x">新的 X 轴数值（若为 null 则保持原值）</param>
        /// <param name="y">新的 Y 轴数值（若为 null 则保持原值）</param>
        /// <param name="z">新的 Z 轴数值（若为 null 则保持原值）</param>
        /// <returns>包含新数值的 Vector3</returns>
        /// <example>
        /// 使用示例：
        /// Vector3 original = new Vector3(1, 2, 3);
        /// Vector3 modified = original.With(y: 10); // 结果: (1, 10, 3)
        /// </example>
        public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
        {
            // 使用空合并运算符 (??)，如果传入参数为 null，则使用原向量的对应分量
            return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
        }
        
        /// <summary>
        /// 创建一个新的 Vector3，并在原向量基础上增加指定的数值。
        /// 仅对非空的参数轴进行加法运算，未指定的轴保持不变。
        /// </summary>
        /// <param name="vector">原始向量（扩展方法的目标对象）</param>
        /// <param name="x">X 轴的增量（若为 null 则不增加）</param>
        /// <param name="y">Y 轴的增量（若为 null 则不增加）</param>
        /// <param name="z">Z 轴的增量（若为 null 则不增加）</param>
        /// <returns>相加后的新 Vector3</returns>
        /// <example>
        /// 使用示例：
        /// Vector3 original = new Vector3(1, 2, 3);
        /// Vector3 added = original.Add(x: 5); // 结果: (6, 2, 3)
        /// </example>
        public static Vector3 Add(this Vector3 vector, float? x = null, float? y = null, float? z = null)
        {
            // 使用空合并运算符 (??)，如果传入参数为 null，则加 0（即保持不变）
            return new Vector3(vector.x + (x ?? 0), vector.y + (y ?? 0), vector.z + (z ?? 0));
        }
    }
}