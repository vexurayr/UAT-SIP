/* Courtesy of Color, Unity Tool for Game Devs
 * Creator - Austin Foulks
 * Date of Conception - November 30, 2022
 * Edit the code however you need to fit your project
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorController : MonoBehaviour
{
    [SerializeField] private bool isActive;

    [SerializeField] [Range(0f, 360f)] private float targetHue;
    [SerializeField] [Range(0f, 100f)] private float targetSaturation;
    [SerializeField] [Range(0, 100)] private float targetValue;

    [SerializeField] [Range(0f, 360f)] private float hueRange;
    [SerializeField] [Range(0f, 100f)] private float saturationRange;
    [SerializeField] [Range(0f, 100f)] private float valueRange;

    [SerializeField] [Range(0f, 1000f)] private float hueInfluencePercent;
    [SerializeField] [Range(0f, 1000f)] private float saturationInfluencePercent;
    [SerializeField] [Range(0f, 1000f)] private float valueInfluencePercent;

    private ControllerOutput output;
}