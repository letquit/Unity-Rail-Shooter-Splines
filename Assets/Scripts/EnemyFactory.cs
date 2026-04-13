using UnityEngine;
using UnityEngine.Splines;

namespace RailShooter
{
    public static class EnemyFactory
    {
        public static Enemy GenerateEnemy(Enemy enemyPrefab, SplineContainer flightPath, Transform enemyParent,
            Transform flightPathParent)
        {
            return new EnemyBuilder()
                .WithPrefab(enemyPrefab)
                .WithFlightPath(flightPath)
                .WithFlightPathParent(flightPathParent)
                .Build(enemyParent);
        }
    }
}