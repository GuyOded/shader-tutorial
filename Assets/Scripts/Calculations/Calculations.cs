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
        public static float Weierstrass(float x, int iterations = 10, float a = .3f, float b = 8f)
        {
            float sum = 0;
            for (int i = 0; i < iterations; i++)
            {
                sum += Mathf.Pow(a, i) * Mathf.Cos(Mathf.Pow(b, i) * Mathf.PI * x);
            }

            return sum;
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
