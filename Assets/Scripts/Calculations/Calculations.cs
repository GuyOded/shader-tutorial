using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Calculations
{
    public static class Consts
    {
        public static readonly float2 DEFAULT_RANGE = new float2(-2, 2);
    }

    public static class MathematicalFunctions
    {
        public static float Weierstrass(float x, int iterations = 10, float a = .3f, float b = 8f, float phase = 0)
        {
            float sum = 0;
            for (int i = 0; i < iterations; i++)
            {
                sum += Mathf.Pow(a, i) * Mathf.Cos(Mathf.Pow(b, i) * Mathf.PI * x + phase);
            }

            return sum;
        }

        public static float Beat(float x, float f1, float f2, float phase)
        {
            return 0.5f * (Mathf.Cos(2 * Mathf.PI * f1 * x + phase) + Mathf.Cos(2 * Mathf.PI * f2 * x + phase));
        }

        public static float[] Linspace(float min, float max, int pointsCount)
        {
            if (min >= max)
            {
                throw new ArgumentException($"min has to be smaller than max: ({min}, {max})");
            }

            return Enumerable.Range(0, pointsCount).Select(index => min + (max - min) / pointsCount * index).ToArray();
        }

        public static Vector2[] Linspace2D(float xMin, float xMax, float yMin, float yMax, int length)
        {
            IEnumerable<float> xRange = Enumerable.Range(0, length).Select(i => xMin + i * (xMax - xMin) / length);
            IEnumerable<float> yRange = Enumerable.Range(0, length).Select(i => yMin + i * (yMax - yMin) / length);

            return xRange.SelectMany(y => yRange, (x, y) => new Vector2(x, y)).ToArray();
        }

        public static float TwoDimensionalRipple(float x, float y, float phase, float distanceClamp = .1f, float frequenecy = 1)
        {
            float distance = Mathf.Sqrt(x * x + y * y);
            return Mathf.Sin(frequenecy * distance + phase) / (distance + distanceClamp);
        }

        public static float CirclingDecayingGaussians(float x, float y, float phase, float decayConstant = 1)
        {
            float2 firstCenter = new(Mathf.Sin(phase), Mathf.Cos(phase));
            float2 secondCenter = new(Mathf.Sin(phase + Mathf.PI), Mathf.Cos(phase + Mathf.PI));

            float2 evaluationPoint = new(x, y);
            float firstExponent = -decayConstant * Square2D(evaluationPoint - firstCenter);
            float secondExponent = -decayConstant * Square2D(evaluationPoint - secondCenter);

            return Mathf.Exp(firstExponent) + Mathf.Exp(secondExponent);
        }

        private static float Square2D(float2 vector)
        {
            return vector.x * vector.x + vector.y * vector.y;
        }
    }
}
