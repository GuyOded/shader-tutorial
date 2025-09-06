using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Calculations.Mappings
{
    class WirestrassMap : IMapping
    {
        private readonly float2 domain;
        private readonly float z;

        public WirestrassMap()
        {
            domain = Consts.DEFAULT_RANGE;
            z = 0;
        }

        public IEnumerable<float3> CalculatePoints(int pointsCount)
        {
            return MathematicalFunctions.LinspaceEnumerator(domain.x, domain.y, pointsCount).Select(point => new float3(point, MathematicalFunctions.Weierstrass(point, phase: PhaseController.Phase), z));
        }
    }

    class BeatMap : IMapping
    {
        private readonly float2 domain;
        private readonly float z;
        private readonly float freq1;
        private readonly float freq2;

        public BeatMap(float freq1, float freq2)
        {
            domain = Consts.DEFAULT_RANGE;
            z = 0;
            this.freq1 = freq1;
            this.freq2 = freq2;
        }

        public IEnumerable<float3> CalculatePoints(int pointsCount)
        {
            return MathematicalFunctions.LinspaceEnumerator(domain.x, domain.y, pointsCount).Select(point => new float3(point, MathematicalFunctions.Beat(point, freq1, freq2, PhaseController.Phase), z));
        }
    }

    class RippleMap : IMapping
    {
        private readonly float2 domain;
        private readonly float freq1;
        private readonly float distanceClamp;

        public RippleMap(float freq1, float distanceClamp = 0.5f)
        {
            domain = Consts.DEFAULT_RANGE;
            this.freq1 = freq1;
            this.distanceClamp = distanceClamp;
        }

        public IEnumerable<float3> CalculatePoints(int pointsCount)
        {
            int perAxisLength = Mathf.FloorToInt(Mathf.Sqrt(pointsCount));
            return MathematicalFunctions.Linspace2DEnumerator(domain.x, domain.y, domain.x, domain.y, perAxisLength).Select((point) =>
            {
                float value = MathematicalFunctions.TwoDimensionalRipple(point.x, point.y, PhaseController.Phase, distanceClamp, freq1);
                return new float3(point.x, value, point.y);
            });
        }
    }

    class CirclingDecayingGaussiansMap : IMapping
    {
        private readonly float2 domain;
        private readonly float decayConstant;

        public CirclingDecayingGaussiansMap(float decayConstant = 1f)
        {
            domain = Consts.DEFAULT_RANGE;
            this.decayConstant = decayConstant;
        }

        public IEnumerable<float3> CalculatePoints(int pointsCount)
        {
            int perAxisLength = Mathf.FloorToInt(Mathf.Sqrt(pointsCount));
            return MathematicalFunctions.Linspace2DEnumerator(domain.x, domain.y, domain.x, domain.y, perAxisLength).Select(point =>
            {
                float value = MathematicalFunctions.CirclingDecayingGaussians(point.x, point.y, PhaseController.Phase, decayConstant);
                return new float3(point.x, value, point.y);
            });
        }
    }

    class WavingSphereMap : IMapping
    {
        private readonly float2 azimuthalDomain;
        private readonly float2 elevationDomain;
        private readonly float radius;

        public WavingSphereMap(float radius)
        {
            azimuthalDomain = Consts.AZIMUTHAL_RANGE;
            elevationDomain = Consts.ELEVATION_RANGE;
            this.radius = radius;
        }

        public WavingSphereMap() : this(Consts.DEFAULT_SPHERE_RADIUS)
        {
        }

        public IEnumerable<float3> CalculatePoints(int pointsCount)
        {
            int perAxisLength = Mathf.FloorToInt(Mathf.Sqrt(pointsCount));
            return MathematicalFunctions.Linspace2DEnumerator(
                azimuthalDomain.x, azimuthalDomain.y,
                elevationDomain.x, elevationDomain.y,
                perAxisLength
            ).Select(point =>
            {
                float azimuth = point.x;
                float elevation = point.y;
                return MathematicalFunctions.WavingSphere(azimuth, elevation, PhaseController.Phase, radius);
            });
        }
    }

    class DonutMap : IMapping
    {
        private readonly float primaryRadius;
        private readonly float secondaryRadius;

        public DonutMap(float primaryRadius = 2, float secondaryRadius = 0.5f)
        {
            this.primaryRadius = primaryRadius;
            this.secondaryRadius = secondaryRadius;
        }

        public IEnumerable<float3> CalculatePoints(int pointsCount)
        {
            float countSqrt = Mathf.Sqrt(pointsCount);
            int mainAxisLength = Mathf.FloorToInt(2 * countSqrt);
            int secondaryAxisLength = Mathf.FloorToInt(0.5f * countSqrt);

            return MathematicalFunctions.Linspace2DEnumerator(Consts.AZIMUTHAL_RANGE.x,
                    Consts.AZIMUTHAL_RANGE.y,
                    Consts.AZIMUTHAL_RANGE.x,
                    Consts.AZIMUTHAL_RANGE.y,
                    mainAxisLength, secondaryAxisLength).Select(point => MathematicalFunctions.Donut(primaryRadius, secondaryRadius, point.x, point.y, PhaseController.Phase));
        }
    }

    class TwistedTorusMap : IMapping
    {
        private readonly float primaryRadius;
        private readonly float secondaryRadius;
        private readonly float frequency;

        public TwistedTorusMap(float primaryRadius = 2, float secondaryRadius = 0.5f, float frequency = 2)
        {
            this.primaryRadius = primaryRadius;
            this.secondaryRadius = secondaryRadius;
            this.frequency = frequency;
        }

        public IEnumerable<float3> CalculatePoints(int pointsCount)
        {
            float countSqrt = Mathf.Sqrt(pointsCount);
            int mainAxisLength = Mathf.FloorToInt(2 * countSqrt);
            int secondaryAxisLength = Mathf.FloorToInt(0.5f * countSqrt);

            return MathematicalFunctions.Linspace2DEnumerator(Consts.AZIMUTHAL_RANGE.x,
                    Consts.AZIMUTHAL_RANGE.y,
                    Consts.AZIMUTHAL_RANGE.x,
                    Consts.AZIMUTHAL_RANGE.y,
                    mainAxisLength, secondaryAxisLength).Select(point => MathematicalFunctions.TwistedTorus(primaryRadius, secondaryRadius, point.x, point.y, PhaseController.Phase, frequency));
        }
    }
}
