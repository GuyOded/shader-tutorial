using System.Collections.Generic;
using System.Linq;
using Calculations;
using Calculations.Mappings;
using DG.Tweening;
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
    private bool inAnimation = false;

    private float currentFreq1;
    private float currentFreq2;

    private void Start()
    {
        currentFreq1 = freq1;
        currentFreq2 = freq2;
    }

    private void Update()
    {
        if (selectedFunction != currentFunction && !inAnimation)
        {
            currentFunction = selectedFunction;
            UpdateCameraViewPoint();
            UpdateFunction();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {

            List<FunctionEnumeration> keysList = functionToTypeMap.Keys.ToList();
            int index = keysList.IndexOf(selectedFunction);
            int nextIndex = (index + 1) % functionToTypeMap.Keys.Count();
            FunctionEnumeration nextFunction = keysList[nextIndex];
            IMapping nextMap = GetMapByType(nextFunction);

            Sequence sequence = DOTween.Sequence();
            inAnimation = true;
            sequence.Append(graphGenerator.SetMappingWithAnimation(nextMap));
            Transform viewPoint = GetCameraViewPointFromFunction(nextFunction);

            sequence.Join(mainCamera.transform.DOMove(viewPoint.position, Consts.ANIMATION_DURATION_SECONDS));
            sequence.Join(mainCamera.transform.DORotateQuaternion(viewPoint.rotation, Consts.ANIMATION_DURATION_SECONDS));

            sequence.OnComplete(() =>
            {
                selectedFunction = nextFunction;
                currentFunction = nextFunction;
                inAnimation = false;
            });
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
        IMapping currentMap = GetMapByType(currentFunction);

        graphGenerator.CurrentMap = currentMap;
    }

    private IMapping GetMapByType(FunctionEnumeration functionType)
    {
        return functionType switch
        {
            FunctionEnumeration.Weierstrass => new WirestrassMap(),
            FunctionEnumeration.Beat => new BeatMap(freq1, freq2),
            FunctionEnumeration.Ripple => new RippleMap(freq1),
            FunctionEnumeration.CirclingDecayingExponents => new CirclingDecayingGaussiansMap(3),
            FunctionEnumeration.WavingSphere => new WavingSphereMap(),
            FunctionEnumeration.Donut => new DonutMap(),
            _ => new WirestrassMap()
        };
    }

    private void UpdateCameraViewPoint()
    {
        Transform viewPoint = GetCameraViewPointFromFunction(currentFunction);
        mainCamera.transform.position = viewPoint.position;
        mainCamera.transform.rotation = viewPoint.rotation;
    }

    private Transform GetCameraViewPointFromFunction(FunctionEnumeration function) => functionToTypeMap[function] switch
    {
        FunctionType.ThreeDScalar or FunctionType.ThreeDSurface => threeDViewPoint,
        FunctionType.TwoDScalar => twoDViewPoint,
        _ => twoDViewPoint
    };

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
