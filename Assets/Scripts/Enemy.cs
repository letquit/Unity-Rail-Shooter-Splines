using KBCore.Refs;
using UnityEngine;
using UnityEngine.Splines;

namespace RailShooter
{
    /// <summary>
    /// 敌机类。
    /// 处理敌机的路径移动、生命周期管理以及被击中时的爆炸逻辑。
    /// </summary>
    public class Enemy : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private SplineAnimate splineAnimate; // 路径动画组件（自动获取自身组件）
        [SerializeField] private GameObject explosionPrefab; // 爆炸特效预制体

        private SplineContainer flightPath; // 关联的飞行路径容器（用于清理）

        /// <summary>
        /// 飞行路径属性。
        /// 允许外部设置或获取敌机的飞行路径数据。
        /// </summary>
        public SplineContainer FlightPath
        {
            get => flightPath;
            set => flightPath = value;
        }

        private void Update()
        {
            // 检查动画是否播放完毕
            // 如果存在动画组件，且总时长大于0，且当前已播放时间超过了总时长
            if (splineAnimate != null && splineAnimate.Duration > 0f &&
                splineAnimate.ElapsedTime >= splineAnimate.Duration)
            {
                // 敌机已到达路径终点，销毁自身
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 触发器碰撞检测。
        /// 处理敌机被子弹击中的逻辑。
        /// </summary>
        /// <param name="other">碰撞体信息</param>
        private void OnTriggerEnter(Collider other)
        {
            // 检查碰撞物体的层级是否为 "Projectile"（子弹）
            // 如果不是子弹，则忽略碰撞
            if (other.gameObject.layer != LayerMask.NameToLayer("Projectile")) return;

            // 1. 实例化爆炸特效
            var explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            
            // 2. 销毁子弹
            Destroy(other.gameObject);
            
            // 3. 销毁敌机自身
            Destroy(gameObject);
            
            // 4. 延迟销毁爆炸特效（5秒后），避免特效常驻内存
            Destroy(explosion, 5f);
        }

        /// <summary>
        /// 对象销毁时的清理工作。
        /// 确保关联的动态路径对象也被清理。
        /// </summary>
        private void OnDestroy()
        {
            // 如果关联了飞行路径对象，则将其销毁
            // 这通常用于动态生成的路径，防止敌机死后路径残留
            if (flightPath != null)
                Destroy(flightPath.gameObject);
        }
    }
}