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
            if (splineAnimate != null && splineAnimate.Duration > 0f &&
                splineAnimate.ElapsedTime >= splineAnimate.Duration)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Projectile")) return;

            var explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(other.gameObject);
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