using UnityEngine;
using UnityEngine.Splines;

namespace RailShooter
{
    public static class FlightPathFactory
    {
        // parent 必传：轨道在该父节点局部空间生成
        public static SplineContainer GenerateFlightPath(Annulus[] annuli, Transform parent)
        {
            var flightPath = new GameObject("Flight Path");
            if (parent != null)
            {
                flightPath.transform.SetParent(parent, false);
                flightPath.transform.localPosition = Vector3.zero;
                flightPath.transform.localRotation = Quaternion.identity;
            }

            var container = flightPath.AddComponent<SplineContainer>();
            var spline = container.AddSpline();

            var knots = new BezierKnot[annuli.Length];
            for (int i = 0; i < annuli.Length; i++)
            {
                Vector3 localPoint = annuli[i].GetRandomPoint();
                knots[i] = new BezierKnot(localPoint, -30 * Vector3.forward, 30 * Vector3.forward);
            }

            spline.Knots = knots;
            return container;
        }
    }
}