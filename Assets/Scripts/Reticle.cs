using KBCore.Refs;
using UnityEngine;

namespace RailShooter
{
    /// <summary>
    /// 准星控制器。
    /// 负责将 UI 准星绑定到 3D 空间中某一点的屏幕投影位置上。
    /// </summary>
    public class Reticle : ValidatedMonoBehaviour
    {
        [SerializeField] private Transform targetPoint; // 目标点（准星要对齐的 3D 位置）
        [SerializeField, Self] private RectTransform rectTransform; // 准星自身的 RectTransform 组件（自动获取）

        private void Update() 
        {
            // 将 3D 目标点转换为屏幕坐标，并赋值给 UI 准星的位置
            rectTransform.position = Camera.main.WorldToScreenPoint(targetPoint.position);
        }
    }
}