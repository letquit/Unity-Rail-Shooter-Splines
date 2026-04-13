using UnityEngine;
using UnityEngine.Splines;

namespace RailShooter
{
    public class EnemyBuilder
    {
        private Transform flightPathParent;
        private Enemy enemyPrefab;
        private SplineContainer flightPath;
        private SplineAnimate.LoopMode loopMode = SplineAnimate.LoopMode.Once;

        public EnemyBuilder WithFlightPathParent(Transform flightPathParent)
        {
            this.flightPathParent = flightPathParent;
            return this;
        }
        
        public EnemyBuilder WithPrefab(Enemy enemyPrefab)
        {
            this.enemyPrefab = enemyPrefab;
            return this;
        }
        
        public EnemyBuilder WithFlightPath(SplineContainer flightPath)
        {
            this.flightPath = flightPath;
            return this;
        }
        
        public EnemyBuilder WithLoopMode(SplineAnimate.LoopMode loopMode)
        {
            this.loopMode = loopMode;
            return this;
        }

        public Enemy Build(Transform enemyParent)
        {
            var enemy = Object.Instantiate(enemyPrefab, enemyParent);

            enemy.FlightPath = flightPath;

            if (flightPath != null)
            {
                var splineAnimate = enemy.GetComponent<SplineAnimate>();
                splineAnimate.Container = flightPath;
                splineAnimate.Loop = loopMode;
                splineAnimate.ElapsedTime = 0f;
            }

            if (flightPathParent != null && flightPath != null)
            {
                Transform transform;
                (transform = flightPath.transform).SetParent(flightPathParent);
                // 重置局部位置和旋转
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }

            return enemy;
        }
    }
}