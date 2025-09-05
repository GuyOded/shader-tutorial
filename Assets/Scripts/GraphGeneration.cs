using System;
using System.Collections.Generic;
using System.Linq;
using Calculations.Mappings;
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

    private List<GameObject> graphPointArray;
    private bool functionTypeChanged = false;
    private int lastResolution = 0;

    private IMapping currentMap = new WirestrassMap();

    public IMapping CurrentMap { get => currentMap; set => currentMap = value; }

    private void Awake()
    {
        graphPointArray = new(resolution);
        if (parent == null)
        {
            parent = transform;
        }
    }

    private void Update()
    {
        InstantiateGraphPoints();

        if (graphPointArray.Count > 0 && graphPointArray.First().transform.localScale.x != scale)
        {
            UpdateScale();
        }
    }

    private void InstantiateGraphPoints()
    {
        foreach ((float3 calculateGraphPoint, GameObject pointGameObject) in currentMap.CalculatePoints(resolution).Zip(graphPointArray, (calculatedPoint, graphPoint) => (calculatedPoint, graphPoint)))
        {
            pointGameObject.transform.localPosition = new(calculateGraphPoint.x, calculateGraphPoint.y, calculateGraphPoint.z);
            pointGameObject.SetActive(true);
        }

        if (graphPointArray.Count < currentMap.CalculatePoints(resolution).Count())
        {
            foreach (float3 calculatedGraphPoint in currentMap.CalculatePoints(resolution).Skip(graphPointArray.Count))
            {
                Vector3 visualPointPosition = new(calculatedGraphPoint.x, calculatedGraphPoint.z, calculatedGraphPoint.y);
                GameObject newVisualPoint = Instantiate(pointPrefab, parent);
                newVisualPoint.transform.localPosition = visualPointPosition;
                newVisualPoint.transform.localScale = Vector3.one * scale;
                graphPointArray.Add(newVisualPoint);
            }
        }
        else if (graphPointArray.Count > currentMap.CalculatePoints(resolution).Count())
        {
            foreach (GameObject visualGraphPoint in graphPointArray.Skip(currentMap.CalculatePoints(resolution).Count()))
            {
                visualGraphPoint.SetActive(false);
            }
        }
    }

    private void UpdateScale()
    {
        graphPointArray.ForEach(gpe =>
        {
            gpe.transform.localScale = Vector3.one * scale;
        });
    }
}
