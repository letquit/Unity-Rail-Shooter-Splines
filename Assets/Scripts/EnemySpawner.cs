using System;
using Shapes;
using UnityEngine;
using Utilities;

namespace RailShooter
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Annulus[] annuli;
        [SerializeField] private Disc[] discs;
        [SerializeField] private Enemy enemyPrefab;
        [SerializeField] private float spawnInterval = 5f;

        [SerializeField] private Transform enemyParent;
        [SerializeField] private Transform flightPathParent;

        private float spawnTimer;
        
        // 仅使用形状进行调试
        private void Start()
        {
            for (var i = 0; i < annuli.Length; i++)
            {
                discs[i].transform.position = transform.position.With(z: annuli[i].distance);
                discs[i].Radius = annuli[i].outerRadius;
                discs[i].Thickness = annuli[i].outerRadius - annuli[i].innerRadius;
            }
        }

        private void Update()
        {
            if (spawnTimer > spawnInterval)
            {
                spawnTimer = 0f;
                SpawnEnemy();
            }

            spawnTimer += Time.deltaTime;
        }

        private void SpawnEnemy()
        {
            var flightPath = FlightPathFactory.GenerateFlightPath(annuli);
            EnemyFactory.GenerateEnemy(enemyPrefab, flightPath, enemyParent, flightPathParent);
        }
    }
}