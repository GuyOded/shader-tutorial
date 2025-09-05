using System.Collections.Generic;
using Calculations.Mappings;
using UnityEngine;

public class FunctionSelector : MonoBehaviour
{
    [SerializeField] private FunctionEnumeration selectedFunction;
    [SerializeField] private GraphGeneration graphGenerator;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform threeDViewPoint;
    [SerializeField] private Transform twoDViewPoint;

    // Function configuration
    // Beat config
    [SerializeField] private float freq1 = 2.85f;
    [SerializeField] private float freq2 = 2.5f;

    private static readonly Dictionary<FunctionEnumeration, FunctionType> functionToTypeMap = new()
    {
        {FunctionEnumeration.Beat, FunctionType.TwoDScalar},
        {FunctionEnumeration.Weierstrass, FunctionType.TwoDScalar},
        {FunctionEnumeration.Ripple, FunctionType.ThreeDScalar},
        {FunctionEnumeration.CirclingDecayingExponents, FunctionType.ThreeDScalar},
        {FunctionEnumeration.WavingSphere, FunctionType.ThreeDSurface},
        {FunctionEnumeration.Donut, FunctionType.ThreeDSurface}
    };

    private FunctionEnumeration currentFunction = FunctionEnumeration.Weierstrass;

    private float currentFreq1;
    private float currentFreq2;

    private void Start()
    {
        currentFreq1 = freq1;
        currentFreq2 = freq2;
    }

    private void Update()
    {
        if (selectedFunction != currentFunction)
        {
            currentFunction = selectedFunction;
            UpdateCameraViewPoint();
            UpdateFunction();
        }

        if (currentFreq1 != freq1)
        {
            currentFreq1 = freq1;
            UpdateFunction();
        }

        if (currentFreq2 != freq2)
        {
            currentFreq2 = freq2;
            UpdateFunction();
        }
    }

    private void UpdateFunction()
    {
        IMapping currentMap = currentFunction switch
        {
            FunctionEnumeration.Weierstrass => new WirestrassMap(),
            FunctionEnumeration.Beat => new BeatMap(freq1, freq2),
            FunctionEnumeration.Ripple => new RippleMap(freq1),
            FunctionEnumeration.CirclingDecayingExponents => new CirclingDecayingGaussiansMap(3),
            FunctionEnumeration.WavingSphere => new WavingSphereMap(),
            FunctionEnumeration.Donut => new DonutMap(),
            _ => new WirestrassMap()
        };

        graphGenerator.CurrentMap = currentMap;
    }

    private void UpdateCameraViewPoint()
    {
        switch (functionToTypeMap[currentFunction])
        {
            case FunctionType.ThreeDScalar:
            case FunctionType.ThreeDSurface:
                mainCamera.transform.position = threeDViewPoint.position;
                mainCamera.transform.rotation = threeDViewPoint.rotation;
                break;
            case FunctionType.TwoDScalar:
                mainCamera.transform.position = twoDViewPoint.position;
                mainCamera.transform.rotation = twoDViewPoint.rotation;
                break;
        }
    }

    private enum FunctionEnumeration
    {
        Weierstrass,
        Beat,
        Ripple,
        CirclingDecayingExponents,
        WavingSphere,
        Donut
    }
}

public enum FunctionType
{
    TwoDScalar,
    ThreeDScalar,
    ThreeDSurface,
}
