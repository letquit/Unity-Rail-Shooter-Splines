using System;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.Splines;

namespace RailShooter
{
    public class Enemy : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private SplineAnimate splineAnimate;
        [SerializeField] private GameObject explosionPrefab;

        private SplineContainer flightPath;

        public SplineContainer FlightPath
        {
            get => flightPath;
            set => flightPath = value;
        }
        
        private void Update()
        {
            if (splineAnimate != null && splineAnimate.ElapsedTime >= splineAnimate.Duration)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
            Destroy(explosion, 5f);
        }

        private void OnDestroy()
        {
            if (flightPath != null)
                Destroy(flightPath.gameObject);
        }
    }
}