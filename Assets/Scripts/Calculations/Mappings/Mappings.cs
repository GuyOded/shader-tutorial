using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Calculations.Mappings
{
    class WirestrassMap : Mapping
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
            return MathematicalFunctions.LinspaceEnumerator(domain.x, domain.y, pointsCount).Select(point => new float3(point, MathematicalFunctions.Weierstrass(point, phase: Time.time), z));
        }
    }

    class BeatMap : Mapping
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
            return MathematicalFunctions.LinspaceEnumerator(domain.x, domain.y, pointsCount).Select(point => new float3(point, MathematicalFunctions.Beat(point, freq1, freq2, Time.time), z));
        }
    }

    class RippleMap : Mapping
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
            int perAxisLength = Mathf.RoundToInt(Mathf.Sqrt(pointsCount));
            return MathematicalFunctions.Linspace2DEnumerator(domain.x, domain.y, domain.x, domain.y, pointsCount).Select((point) =>
            {
                float value = MathematicalFunctions.TwoDimensionalRipple(point.x, point.y, Time.time, distanceClamp, freq1);
                return new float3(point.x, point.y, value);
            });
        }
    }

    class CirclingDecayingGaussiansMap : Mapping
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
            int perAxisLength = Mathf.RoundToInt(Mathf.Sqrt(pointsCount));
            return MathematicalFunctions.Linspace2DEnumerator(domain.x, domain.y, domain.x, domain.y, pointsCount).Select(point =>
            {
                float value = MathematicalFunctions.CirclingDecayingGaussians(point.x, point.y, Time.time, decayConstant);
                return new float3(point.x, point.y, value);
            });
        }
    }

    class WavingSphereMap : Mapping
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
            int perAxisLength = Mathf.RoundToInt(Mathf.Sqrt(pointsCount));
            return MathematicalFunctions.Linspace2DEnumerator(
                azimuthalDomain.x, azimuthalDomain.y,
                elevationDomain.x, elevationDomain.y,
                perAxisLength
            ).Select(point =>
            {
                float azimuth = point.x;
                float elevation = point.y;
                return MathematicalFunctions.WavingSphere(azimuth, elevation, Time.time, radius);
            });
        }
    }
}
