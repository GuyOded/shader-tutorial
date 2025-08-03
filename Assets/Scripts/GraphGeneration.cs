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

    private Func<float, float, float> currentFunction = (x, t) => MathematicalFunctions.Weierstrass(x, phase: t);

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
        if (graphPointArray.Count != resolution)
        {
            InstantiateGraphPoints();
        }

        if (graphPointArray.Count > 0 && graphPointArray.First().VisualPoint.transform.localScale.x != scale)
        {
            UpdateScale();
        }

        RecalculatePositions();
    }

    public void SetFunction(Func<float, float, float> func)
    {
        currentFunction = func;
    }

    private void InstantiateGraphPoints()
    {
        float[] samples = MathematicalFunctions.Linspace(Consts.DEFAULT_RANGE.x, Consts.DEFAULT_RANGE.y, resolution);
        int currentlyVisualizedPoints = graphPointArray.Count;

        for (int i = 0; i < samples.Length; i++)
        {
            float x = samples[i] + Time.deltaTime;

            if (i <= currentlyVisualizedPoints - 1)
            {
                graphPointArray[i].GraphPoint = new float3(x, currentFunction(x, Time.time), 0);
                graphPointArray[i].VisualPoint.transform.localPosition = new Vector3(graphPointArray[i].GraphPoint.x, graphPointArray[i].GraphPoint.y, 0);
            }
            else
            {
                float3 newGraphPoint = new(x, currentFunction(x, Time.time), 0);
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

        int rangeCount = range.Length;
        int currentlyVisualizedPointsCount = graphPointArray.Count;

        range.Zip(graphPointArray);

    }

    private void RecalculatePositions()
    {
        graphPointArray.ForEach(gpe =>
        {
            float x = gpe.GraphPoint.x;

            gpe.GraphPoint = new float3(x, currentFunction(x, Time.time), 0);
            gpe.VisualPoint.transform.localPosition = new Vector3(gpe.GraphPoint.x, gpe.GraphPoint.y, 0);
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
