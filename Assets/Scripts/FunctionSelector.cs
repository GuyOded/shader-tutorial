using System;
using Calculations;
using UnityEngine;

public class FunctionSelector : MonoBehaviour
{
    [SerializeField] private FunctionEnumeration selectedFunction;
    [SerializeField] private GraphGeneration graphGenerator;

    // Function configuration
    // Beat config
    [SerializeField] private float freq1 = 2.85f;
    [SerializeField] private float freq2 = 2.5f;

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
        Func<float, float, float> f = currentFunction switch
        {
            FunctionEnumeration.Beat => (x, t) => MathematicalFunctions.Beat(x, freq1, freq2, t),
            FunctionEnumeration.Weierstrass => (x, t) => MathematicalFunctions.Weierstrass(x, phase: t),
            _ => (x, t) => MathematicalFunctions.Weierstrass(x, phase: t)
        };

        graphGenerator.SetFunction(f);
    }

    private enum FunctionEnumeration
    {
        Weierstrass,
        Beat,
        Ripple
    }
}
