using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Calculations;
using Unity.Collections;
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
        {FunctionEnumeration.CirclingDecayingExponents, FunctionType.ThreeDScalar}
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
        if (functionToTypeMap[currentFunction] == FunctionType.TwoDScalar)
        {
            Func<float, float, float> f = currentFunction switch
            {
                FunctionEnumeration.Beat => (x, t) => MathematicalFunctions.Beat(x, freq1, freq2, t),
                FunctionEnumeration.Weierstrass => (x, t) => MathematicalFunctions.Weierstrass(x, phase: t),
                _ => (x, t) => MathematicalFunctions.Weierstrass(x, phase: t)
            };
            graphGenerator.Current2dFunction = f;
            graphGenerator.CurrentType = FunctionType.TwoDScalar;
        }
        else if (functionToTypeMap[currentFunction] == FunctionType.ThreeDScalar)
        {
            Func<float, float, float, float> f = currentFunction switch
            {
                FunctionEnumeration.Ripple => (x, y, time) => MathematicalFunctions.TwoDimensionalRipple(x, y, time, 0.5f, freq1),
                FunctionEnumeration.CirclingDecayingExponents => (x, y, time) => MathematicalFunctions.CirclingDecayingGaussians(x, y, time, 4),
                _ => (x, y, time) => MathematicalFunctions.TwoDimensionalRipple(x, y, time, 0.5f, freq1)
            };

            graphGenerator.Current3dFunction = f;
            graphGenerator.CurrentType = FunctionType.ThreeDScalar;
        }

    }

    private void UpdateCameraViewPoint()
    {
        switch (functionToTypeMap[currentFunction])
        {
            case FunctionType.ThreeDScalar:
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
    }
}

public enum FunctionType
{
    TwoDScalar,
    ThreeDScalar,
}
