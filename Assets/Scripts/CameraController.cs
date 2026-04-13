using System;
using UnityEngine;

namespace RailShooter
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Transform followTarget;
        [SerializeField] private float followDistance = 22f;
        [SerializeField] private float smoothTime = 0.2f;

        private Vector3 velocity;

        private void Update()
        {
            Vector3 targetPos = followTarget.position + followTarget.forward * -followDistance;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
            
            // 设置摄像机的旋转以匹配玩家的旋转
            transform.rotation = player.rotation;
        }
    }
}