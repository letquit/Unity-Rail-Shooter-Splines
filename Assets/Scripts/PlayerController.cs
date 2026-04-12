using System;
using DG.Tweening;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.Serialization;

namespace RailShooter
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private InputReader input;
        
        [SerializeField] private Transform followTarget;
        [SerializeField] private Transform playerModel;
        [SerializeField] private float followDistance = 2f;
        [SerializeField] private Vector2 movementLimit = new Vector2(2f, 2f);
        [SerializeField] private float movementRange = 5f;
        [SerializeField] private float movementSpeed = 10f;
        [SerializeField] private float smoothTime = 0.2f;

        [SerializeField] private float maxRoll;
        [SerializeField] private float rollSpeed = 2f;
        [SerializeField] private float rollDuration = 1f;
        
        private Vector3 velocity;
        private float roll;

        private void Awake()
        {
            input.LeftTab += OnLeftTab;
            input.RightTab += OnRightTab;
        }

        private void Update()
        {
            // 将玩家放置在目标后方指定距离处
            Vector3 targetPos = followTarget.position + followTarget.forward * -followDistance;
            
            // 使用 SmoothDamp 实现平滑的跟随效果
            Vector3 smoothedPos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
            
            // 将平滑后的世界坐标转换为相对于跟随目标的局部坐标
            Vector3 localPos = transform.InverseTransformPoint(smoothedPos);
            localPos.x += input.Move.x * movementSpeed * Time.deltaTime * movementRange;
            localPos.y += input.Move.y * movementSpeed * Time.deltaTime * movementRange;
            
            // 确保玩家不会移出指定的移动范围
            localPos.x = Mathf.Clamp(localPos.x, -movementLimit.x, movementLimit.x);
            localPos.y = Mathf.Clamp(localPos.y, -movementLimit.y, movementLimit.y);
            
            // 将限制后的局部坐标转换回世界坐标
            transform.position = transform.TransformPoint(localPos);
            
            // 将玩家旋转与跟随目标的旋转对齐
            transform.rotation = followTarget.rotation;
            
            // 使用 Lerp 平滑过渡到目标倾斜角度
            roll = Mathf.Lerp(roll, input.Move.x * maxRoll, Time.deltaTime * rollSpeed);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, roll);
        }

        private void OnLeftTab() => BarrelRoll();
        
        private void OnRightTab() => BarrelRoll(1);

        private void BarrelRoll(int direction = -1)
        {
            if (!DOTween.IsTweening(playerModel))
            {
                playerModel
                    .DOLocalRotate(
                        new Vector3(playerModel.localEulerAngles.x, playerModel.localEulerAngles.y, 360 * direction), rollDuration,
                        RotateMode.LocalAxisAdd).SetEase(Ease.OutCubic);
            }
        }
        
        private void OnDestroy()
        {
            input.LeftTab -= OnLeftTab;
            input.RightTab -= OnRightTab;
        }
    }
}
