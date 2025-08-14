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

    private Func<float, float, float> current2DFunction = (x, t) => MathematicalFunctions.Weierstrass(x, phase: t);
    private Func<float, float, float, float> current3DFunction = (x, y, t) => MathematicalFunctions.TwoDimensionalRipple(x, y, t);
    private FunctionType currentType = FunctionType.TwoDScalar;

    public Func<float, float, float> Current2dFunction { get => current2DFunction; set => current2DFunction = value; }
    public Func<float, float, float, float> Current3dFunction { get => current3DFunction; set => current3DFunction = value; }
    public FunctionType CurrentType { get => currentType;
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
        if (graphPointArray.Count != resolution || functionTypeChanged)
        {
            if (currentType == FunctionType.TwoDScalar)
                Instantiate2DGraphPoints();
            else if (currentType == FunctionType.ThreeDScalar)
                Instantiate3DGraphPoints();
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

    private void Instantiate3DGraphPoints()
    {
        int lengthResolution = Mathf.RoundToInt(Mathf.Sqrt(resolution));
        Vector2[] range = MathematicalFunctions.Linspace2D(Consts.DEFAULT_RANGE.x, Consts.DEFAULT_RANGE.y, Consts.DEFAULT_RANGE.x, Consts.DEFAULT_RANGE.y, lengthResolution);

        foreach ((Vector2 calculatedPoint, GraphPointEncapsulator graphPoint) in range.Zip(graphPointArray, (calculatedPoint, graphPoint) => (calculatedPoint, graphPoint)))
        {
            float currentValue = current3DFunction(calculatedPoint.x, calculatedPoint.y, Time.time);
            graphPoint.GraphPoint = new float3(calculatedPoint.x, currentValue, calculatedPoint.y);
            graphPoint.VisualPoint.transform.localPosition = new Vector3(graphPoint.GraphPoint.x, graphPoint.GraphPoint.z, graphPoint.GraphPoint.y);
            graphPoint.VisualPoint.SetActive(true);
        }

        if (graphPointArray.Count < range.Length)
        {
            foreach (Vector2 domainPoint in range.Skip(graphPointArray.Count))
            {
                float currentValue = current3DFunction(domainPoint.x, domainPoint.y, Time.time);
                float3 graphPoint = new(domainPoint.x, currentValue, domainPoint.y);
                Vector3 visualPointPosition = new(graphPoint.x, graphPoint.z, graphPoint.y);
                GameObject newVisualPoint = Instantiate(pointPrefab, parent);
                newVisualPoint.transform.localPosition = visualPointPosition;
                GraphPointEncapsulator graphPointEncapsulator = new(graphPoint, newVisualPoint);
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
            float x = gpe.GraphPoint.x;

            if (currentType == FunctionType.TwoDScalar)
            {
                gpe.GraphPoint = new float3(x, current2DFunction(x, Time.time), 0);
            }
            else if (currentType == FunctionType.ThreeDScalar)
            {
                gpe.GraphPoint = new float3(x, current3DFunction(x, gpe.GraphPoint.z, Time.time), gpe.GraphPoint.z);
            }
            gpe.VisualPoint.transform.localPosition = new Vector3(gpe.GraphPoint.x, gpe.GraphPoint.y, gpe.GraphPoint.z);
        });
    }

    private float GetRangeClampedX(float x)
    {
        if (x > 2)
        {
            x = (x + 2) % (Consts.DEFAULT_RANGE.y - Consts.DEFAULT_RANGE.x);
            x -= Consts.DEFAULT_RANGE.y;
        }

        return x;
    }

    private void UpdateScale()
    {
        graphPointArray.ForEach(gpe =>
        {
            gpe.VisualPoint.transform.localScale = Vector3.one * scale;
        });
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
