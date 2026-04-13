using System;
using KBCore.Refs;
using UnityEngine;

namespace RailShooter
{
    public class WeaponSystem : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private InputReader input;

        [SerializeField] private Transform targetPoint;
        [SerializeField] private float targetDistance = 50f;
        [SerializeField] private float smoothTime = 0.2f;
        [SerializeField] private Vector2 aimLimit = new Vector2(50f, 20f);
        [SerializeField] private float aimSpeed = 10f;
        [SerializeField] private float aimReturnSpeed = 0.2f;

        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform firePoint;
        
        private Vector3 velocity;
        private Vector2 aimOffset;

        private void Awake()
        {
            input.Fire += OnFire;
        }

        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            // 将目标位置设置在玩家当前位置前方，距离为 targetDistance
            Vector3 targetPosition = transform.position + transform.forward * targetDistance;
            Vector3 localPos = transform.InverseTransformPoint(targetPosition);
            
            // 如果有瞄准输入
            if (input.Aim != Vector2.zero)
            {
                // 将瞄准输入添加到瞄准偏移中
                aimOffset += input.Aim * aimSpeed * Time.deltaTime;
                
                // 限制瞄准偏移的范围
                aimOffset.x = Mathf.Clamp(aimOffset.x, -aimLimit.x, aimLimit.x);
                aimOffset.y = Mathf.Clamp(aimOffset.y, -aimLimit.y, aimLimit.y);
            }
            else
            {
                // 否则，将瞄准偏移逐渐归零
                aimOffset = Vector2.Lerp(aimOffset, Vector2.zero, Time.deltaTime * aimReturnSpeed);
            }
            
            // 将瞄准偏移应用到局部位置
            localPos.x += aimOffset.x;
            localPos.y += aimOffset.y;
            
            var desiredPosition = transform.TransformPoint(localPos);
            
            // 平滑地移动到目标位置
            targetPoint.position = Vector3.SmoothDamp(targetPoint.position, desiredPosition, ref velocity, smoothTime);
        }

        private void OnFire()
        {
            var projectile = Instantiate(projectilePrefab, firePoint.position,
                Quaternion.LookRotation(targetPoint.position - firePoint.position));
            Destroy(projectile, 5f);
        }

        private void OnDestroy()
        {
            input.Fire -= OnFire;
        }
    }
}