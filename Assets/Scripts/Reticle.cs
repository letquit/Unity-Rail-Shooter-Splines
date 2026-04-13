using KBCore.Refs;
using UnityEngine;

namespace RailShooter
{
    public class Reticle : ValidatedMonoBehaviour
    {
        [SerializeField] private Transform targetPoint;
        [SerializeField, Self] private RectTransform rectTransform;

        private void Update() => rectTransform.position = Camera.main.WorldToScreenPoint(targetPoint.position);
    }
}