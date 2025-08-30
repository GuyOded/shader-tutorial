using System;
using System.Collections.Generic;
using System.Linq;
using Calculations;
using Unity.Mathematics;
using UnityEngine;

public class GraphGeneration : MonoBehaviour
{
    [Header("Graph Props")]
    [SerializeField] private int resolution = 10;
    [SerializeField] private Transform parent;
    [SerializeField] private GameObject pointPrefab;

    [Header("Graph Point Props")]
    [Range(0.001f, 1)]
    [SerializeField] private float scale = 0.1f;

    private List<GraphPointEncapsulator> graphPointArray;
    private bool functionTypeChanged = false;
    private int lastResolution = 0;

    private Func<float, float, float> current2DFunction = (x, t) => MathematicalFunctions.Weierstrass(x, phase: t);
    private Func<float, float, float, float> current3DFunction = (x, y, t) => MathematicalFunctions.TwoDimensionalRipple(x, y, t);
    private Func<float, float, float, float3> current3DSurface = (x, y, t) => MathematicalFunctions.WavingSphere(x, y, t, Consts.DEFAULT_SPHERE_RADIUS);
    private FunctionType currentType = FunctionType.TwoDScalar;

    public Func<float, float, float> Current2dFunction { get => current2DFunction; set => current2DFunction = value; }
    public Func<float, float, float, float> Current3dFunction { get => current3DFunction; set => current3DFunction = value; }
    public Func<float, float, float, float3> Current3dSurface { get => current3DSurface; set => current3DSurface = value; }
    public FunctionType CurrentType
    {
        get => currentType;
        set
        {
            if (value != currentType)
            {
                functionTypeChanged = true;
            }
            currentType = value;
        }
    }

    private void Awake()
    {
        graphPointArray = new List<GraphPointEncapsulator>(resolution);
        if (parent == null)
        {
            parent = transform;
        }
    }

    private void Update()
    {
        if (lastResolution != resolution || functionTypeChanged)
        {
            if (currentType == FunctionType.TwoDScalar)
                Instantiate2DGraphPoints();
            else if (currentType == FunctionType.ThreeDScalar)
                Instantiate3DScalarGraphPoints();
            else if (currentType == FunctionType.ThreeDSurface)
                Instantiate3DSurfacePoints();

            functionTypeChanged = false;
            lastResolution = resolution;
        }

        if (graphPointArray.Count > 0 && graphPointArray.First().VisualPoint.transform.localScale.x != scale)
        {
            UpdateScale();
        }

        RecalculatePositions();
    }

    private void Instantiate2DGraphPoints()
    {
        float[] samples = MathematicalFunctions.Linspace(Consts.DEFAULT_RANGE.x, Consts.DEFAULT_RANGE.y, resolution);
        int currentlyVisualizedPoints = graphPointArray.Count;

        for (int i = 0; i < samples.Length; i++)
        {
            float x = samples[i] + Time.deltaTime;

            if (i <= currentlyVisualizedPoints - 1)
            {
                graphPointArray[i].GraphPoint = new float3(x, current2DFunction(x, Time.time), 0);
                graphPointArray[i].VisualPoint.transform.localPosition = new Vector3(graphPointArray[i].GraphPoint.x, graphPointArray[i].GraphPoint.y, 0);
            }
            else
            {
                float3 newGraphPoint = new(x, current2DFunction(x, Time.time), 0);
                GameObject newVisualPoint = Instantiate(pointPrefab, parent);
                newVisualPoint.transform.localPosition = new Vector3(newGraphPoint.x, newGraphPoint.y);
                GraphPointEncapsulator gpe = new(newGraphPoint, newVisualPoint);

                graphPointArray.Add(gpe);
            }

            graphPointArray[i].VisualPoint.transform.localScale = Vector3.one * scale;
            graphPointArray[i].VisualPoint.SetActive(true);
        }

        for (int i = samples.Length; i < currentlyVisualizedPoints; i++)
        {
            graphPointArray[i].VisualPoint.SetActive(false);
        }
    }

    private void Instantiate3DScalarGraphPoints()
    {
        int lengthResolution = Mathf.RoundToInt(Mathf.Sqrt(resolution));
        float2[] range = MathematicalFunctions.Linspace2D(Consts.DEFAULT_RANGE.x,
                                                        Consts.DEFAULT_RANGE.y,
                                                        Consts.DEFAULT_RANGE.x,
                                                        Consts.DEFAULT_RANGE.y,
                                                        lengthResolution);

        float3 current3DScalarGraphFunc(float x, float y) => new(x, current3DFunction(x, y, Time.time), y);
        Instantiate3DGraphPoints(range, current3DScalarGraphFunc);
    }

    private void Instantiate3DSurfacePoints()
    {
        int lengthResolution = Mathf.RoundToInt(Mathf.Sqrt(resolution));
        float2[] range = MathematicalFunctions.Linspace2D(Consts.AZIMUTHAL_RANGE.x,
                                                        Consts.AZIMUTHAL_RANGE.y,
                                                        Consts.ELEVATION_RANGE.x,
                                                        Consts.ELEVATION_RANGE.y,
                                                        lengthResolution);

        float3 current3DSurfaceFunc(float azimuth, float elevation) => current3DSurface(azimuth, elevation, Time.time);
        Instantiate3DGraphPoints(range, current3DSurfaceFunc);
        var min = graphPointArray.Min(gpe => gpe.GraphPoint.y);
        var max = graphPointArray.Max(gpe => gpe.GraphPoint.y);

        // Debug.Log($"Min y: {min}, Max y: {max}");
    }

    private void Instantiate3DGraphPoints(float2[] range, Func<float, float, float3> threeDGraphFunc)
    {
        foreach ((float2 domainPoint, GraphPointEncapsulator graphPoint) in range.Zip(graphPointArray, (calculatedPoint, graphPoint) => (calculatedPoint, graphPoint)))
        {
            graphPoint.GraphPoint = threeDGraphFunc(domainPoint.x, domainPoint.y);
            graphPoint.VisualPoint.transform.localPosition = new Vector3(graphPoint.GraphPoint.x, graphPoint.GraphPoint.z, graphPoint.GraphPoint.y);
            graphPoint.VisualPoint.SetActive(true);
        }

        if (graphPointArray.Count < range.Length)
        {
            foreach (Vector2 domainPoint in range.Skip(graphPointArray.Count))
            {
                float3 graphPoint = threeDGraphFunc(domainPoint.x, domainPoint.y);
                Vector3 visualPointPosition = new(graphPoint.x, graphPoint.z, graphPoint.y);
                GameObject newVisualPoint = Instantiate(pointPrefab, parent);
                newVisualPoint.transform.localPosition = visualPointPosition;
                newVisualPoint.transform.localScale = Vector3.one * scale;
                GraphPointEncapsulator graphPointEncapsulator = new(graphPoint, newVisualPoint);
                graphPointArray.Add(graphPointEncapsulator);
            }
        }
        else if (graphPointArray.Count > range.Length)
        {
            foreach (GraphPointEncapsulator gpe in graphPointArray.Skip(range.Length))
            {
                gpe.VisualPoint.SetActive(false);
            }
        }
    }

    private void RecalculatePositions()
    {
        graphPointArray.ForEach(gpe =>
        {
            float3 coordinate = ConvertCoordinatesByCurrentFunctionType(gpe.GraphPoint);

            if (currentType == FunctionType.TwoDScalar)
            {
                gpe.GraphPoint = new float3(coordinate.x, current2DFunction(coordinate.x, Time.time), 0);
            }
            else if (currentType == FunctionType.ThreeDScalar)
            {
                gpe.GraphPoint = new float3(coordinate.x, current3DFunction(coordinate.x, coordinate.z, Time.time), gpe.GraphPoint.z);
            }
            else if (currentType == FunctionType.ThreeDSurface)
            {
                gpe.GraphPoint = current3DSurface(coordinate.z, coordinate.y, Time.time);
                // gpe.GraphPoint = new float3(gpe.GraphPoint.x, gpe.GraphPoint.z, gpe.GraphPoint.y);
            }
            gpe.VisualPoint.transform.localPosition = new Vector3(gpe.GraphPoint.x, gpe.GraphPoint.y, gpe.GraphPoint.z);
        });
    }

    private void UpdateScale()
    {
        graphPointArray.ForEach(gpe =>
        {
            gpe.VisualPoint.transform.localScale = Vector3.one * scale;
        });
    }

    private float3 ConvertCoordinatesByCurrentFunctionType(float3 cartesianSurfacePoint)
    {
        return currentType switch
        {
            FunctionType.ThreeDSurface => MathematicalFunctions.CartesianToSpherical(cartesianSurfacePoint),
            _ => cartesianSurfacePoint
        };
    }

    private class GraphPointEncapsulator
    {
        public GraphPointEncapsulator(float3 point, GameObject visualPoint)
        {
            VisualPoint = visualPoint;
            GraphPoint = point;
        }

        public GameObject VisualPoint
        {
            get;
            private set;
        }

        public float3 GraphPoint
        {
            get; set;
        }
    }
}
