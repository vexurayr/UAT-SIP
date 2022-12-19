using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CC_ColorSlider : MonoBehaviour
{
    public bool isActive;

    [Range(0, 360)] public float targetHue;
    [Range(0, 100)] public float targetSaturation;
    [Range(0, 100)] public float targetValue;

    [Range(0, 360)] public float hueRange;
    [Range(0, 100)] public float saturationRange;
    [Range(0, 100)] public float valueRange;

    public float hueInfluencePercent;
    public float saturationInfluencePercent;
    public float valueInfluencePercent;
}