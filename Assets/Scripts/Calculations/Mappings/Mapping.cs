using System.Collections.Generic;
using Unity.Mathematics;

public interface IMapping
{
    IEnumerable<float3> CalculatePoints(int pointsCount);
}
