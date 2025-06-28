using System;
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
    }
}
