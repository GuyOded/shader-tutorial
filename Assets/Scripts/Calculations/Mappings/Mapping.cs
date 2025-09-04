using System.Collections.Generic;
using Unity.Mathematics;

interface Mapping
{
    IEnumerable<float3> CalculatePoints(int pointsCount);
}
