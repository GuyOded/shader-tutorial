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
    [SerializeField, Range(1, 20)] private int weierstrassIterations = 10;

    [Header("Graph Point Props")]
    [Range(0.001f, 1)]
    [SerializeField] private float scale = 0.1f;

    private List<GraphPointEncapsulator> graphPointArray;
    private int currentIterations;

    private void Awake()
    {
        graphPointArray = new List<GraphPointEncapsulator>(resolution);
        if (parent == null)
        {
            parent = transform;
        }

        currentIterations = weierstrassIterations;
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

        if (currentIterations != weierstrassIterations)
        {
            currentIterations = weierstrassIterations;
            UpdateIterations();
        }
    }

    private void InstantiateGraphPoints()
    {
        float[] samples = MathematicalFunctions.Linspace(Consts.DEFAULT_RANGE.x, Consts.DEFAULT_RANGE.y, resolution);
        int currentlyVisualizedPoints = graphPointArray.Count;

        for (int i = 0; i < samples.Length; i++)
        {
            if (i <= currentlyVisualizedPoints - 1)
            {
                graphPointArray[i].GraphPoint = new float2(samples[i], MathematicalFunctions.Weierstrass(samples[i], weierstrassIterations));
                graphPointArray[i].VisualPoint.transform.localPosition = new Vector3(graphPointArray[i].GraphPoint.x, graphPointArray[i].GraphPoint.y, 0);
            }
            else
            {
                float2 newGraphPoint = new(samples[i], MathematicalFunctions.Weierstrass(samples[i], weierstrassIterations));
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

    private void UpdateScale()
    {
        graphPointArray.ForEach(gpe =>
        {
            gpe.VisualPoint.transform.localScale = Vector3.one * scale;
        });
    }

    private void UpdateIterations()
    {
        graphPointArray.ForEach(gpe =>
        {
            gpe.GraphPoint = new float2(gpe.GraphPoint.x, MathematicalFunctions.Weierstrass(gpe.GraphPoint.x, weierstrassIterations));
            gpe.VisualPoint.transform.localPosition = new Vector3(gpe.GraphPoint.x, gpe.GraphPoint.y);
        });
    }

    private class GraphPointEncapsulator
    {
        public GraphPointEncapsulator(float2 point, GameObject visualPoint)
        {
            VisualPoint = visualPoint;
            GraphPoint = point;
        }

        public GameObject VisualPoint
        {
            get;
            private set;
        }

        public float2 GraphPoint
        {
            get; set;
        }
    }
}
