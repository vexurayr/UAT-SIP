/* Courtesy of Color, Unity Tool for Game Devs
 * Creator - Austin Foulks
 * Date of Conception - November 30, 2022
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.VirtualTexturing;

public class ColorController : MonoBehaviour
{
    #region Variables
    [Tooltip("Give this one of the game objects you have added the MarkedPosition to.")]
        [SerializeField] private GameObject markedPosition;

    [Tooltip("Set false if the controller is not in use.")] [SerializeField] private bool isUpdating;
    [Tooltip("Enables debug messages.")] [SerializeField] private bool isDebugging;

    [Tooltip("The hue, saturation, and brightness that will output a 1.")]
        [ColorUsage(true, true)] [SerializeField] private Color targetColor;

    [Tooltip("The distance from the target hue before outputting 0.")]
        [SerializeField] [Range(0f, 360f)] private float hueRange;
    [Tooltip("The distance from the target saturation before outputting 0.")]
        [SerializeField] [Range(0f, 100f)] private float saturationRange;
    [Tooltip("The distance from the target brightness before outputting 0.")]
        [SerializeField] [Range(0f, 100f)] private float brightnessRange;

    [Tooltip("How much hue factors into the final output. All 3 at 100 means all 3 have 33% control of the output.")]
        [SerializeField] [Range(0f, 100f)] private float hueInfluencePercent;
    [Tooltip("How much saturation factors into the final output. All 3 at 100 means all 3 have 33% control of the output.")]
        [SerializeField] [Range(0f, 100f)] private float saturationInfluencePercent;
    [Tooltip("How much brightness factors into the final output. All 3 at 100 means all 3 have 33% control of the output.")]
        [SerializeField] [Range(0f, 100f)] private float brightnessInfluencePercent;

    public ControllerOutput controllerOutput;

    private MarkedPosition mP;
    private float targetHue;
    private float targetSaturation;
    private float targetBrightness;
    private float mPHue;
    private float mPSaturation;
    private float mPBrightness;
    private float finalOutput;

    #endregion Variables

    #region MonoBehaviours
    private void Start()
    {
        if (markedPosition.GetComponent<MarkedPosition>() != null)
        {
            mP = markedPosition.GetComponent<MarkedPosition>();
        }
        else if (isDebugging)
        {
            Debug.Log("Check marked position game object for the MarkedPosition component.");
        }

        if (isDebugging)
        {
            Debug.Log("Target Color: " + targetColor);
        }

        ConvertRGBToHSB();
    }

    private void Update()
    {
        if (isUpdating)
        {
            UpdateOutput();

            controllerOutput.Invoke(finalOutput);
        }
    }

    #endregion MonoBehaviours

    #region PrivateFunctions

    private void UpdateOutput()
    {
        if (!mP)
        {
            return;
        }

        // Get the current HSB from the Marked Position
        GetMPHSB();

        // Compare MP's HSB to target HSB using the range values, output for each is 0 to 1
        float hueRatio = CompareHues();
        float saturationRatio = CompareSaturations();
        float brightnessRatio = CompareBrightnesses();

        // Calculate final output based on the influence values, 0 to 1
        CalculateFinalOutput(hueRatio, saturationRatio, brightnessRatio);
    }

    private void GetMPHSB()
    {
        mPHue = mP.GetHue();
        mPSaturation = mP.GetSaturation();
        mPBrightness = mP.GetBrightness();
    }

    private float CompareHues()
    {
        float difference;
        float ratio;

        difference = Mathf.Abs(targetHue - mPHue);

        if (difference > hueRange)
        {
            difference = Mathf.Abs(difference - 360);
            
            // Check again for cases with 360 looping back around to 0
            if (difference > hueRange)
            {
                ratio = 0;
            }
            else
            {
                ratio = difference / hueRange;
                // Inverted so that a difference of 0 would mean an output of 1
                ratio = 1 - ratio;
            }
        }
        else
        {
            ratio = difference / hueRange;
            // Inverted so that a difference of 0 would mean an output of 1
            ratio = 1 - ratio;
        }

        return ratio;
    }

    private float CompareSaturations()
    {
        float difference;
        float ratio;

        difference = Mathf.Abs(targetSaturation - mPSaturation);

        if (difference > saturationRange)
        {
            ratio = 0;
        }
        else
        {
            ratio = difference / saturationRange;
            // Inverted so that a difference of 0 would mean an output of 1
            ratio = 1 - ratio;
        }

        return ratio;
    }

    private float CompareBrightnesses()
    {
        float difference;
        float ratio;

        difference = Mathf.Abs(targetBrightness - mPBrightness);

        if (difference > brightnessRange)
        {
            ratio = 0;
        }
        else
        {
            ratio = difference / brightnessRange;
            // Inverted so that a difference of 0 would mean an output of 1
            ratio = 1 - ratio;
        }

        return ratio;
    }

    private void CalculateFinalOutput(float hueRatio, float saturationRatio, float brightnessRatio)
    {
        // Rebalance the distrubution from the default 33%
        float combinedInfluence = hueInfluencePercent + saturationInfluencePercent + brightnessInfluencePercent;

        float newHueInfluence = hueInfluencePercent / combinedInfluence;
        float newSaturationInfluence = saturationInfluencePercent / combinedInfluence;
        float newBrightnessInfluence = brightnessInfluencePercent / combinedInfluence;

        // Give each value its portion of control over the final outcome
        hueRatio = hueRatio * newHueInfluence;
        saturationRatio = saturationRatio * newSaturationInfluence;
        brightnessRatio = brightnessRatio * newBrightnessInfluence;

        finalOutput = hueRatio + saturationRatio + brightnessRatio;

        if (isDebugging)
        {
            Debug.Log("Hue influence: " + newHueInfluence * 100 + "%, Saturation influence: " + newSaturationInfluence * 100 + "%" +
                ", Brightness influence: " + newBrightnessInfluence * 100 + "%\nHueFraction: " + hueRatio + ", SaturationFraction: " +
                saturationRatio + ", BrightnessFraction: " + brightnessRatio + "\nFinal output: " + finalOutput);
        }
    }

    private void ConvertRGBToHSB()
    {
        // Get HSB from RGB
        Color.RGBToHSV(targetColor, out float h, out float s, out float b);

        // Set target HSB for comparisons
        targetHue = h * 360;
        targetSaturation = s * 100;
        targetBrightness = b * 100;

        if (isDebugging)
        {
            Debug.Log("Target Hue: " + targetHue + ", Target Saturation: " + targetSaturation + ", Target Brightness: " + targetBrightness);
        }
    }

    #endregion PrivateFunctions

    #region PublicGetters
    public float GetMPHue()
    {
        return mPHue;
    }

    public float GetMPSaturation()
    {
        return mPSaturation;
    }

    public float GetMPBrightness()
    {
        return mPBrightness;
    }

    public float GetTargetHue()
    {
        return targetHue;
    }

    public float GetTargetSaturation()
    {
        return targetSaturation;
    }

    public float GetTargetBrightness()
    {
        return targetBrightness;
    }

    public float GetHueRange()
    {
        return hueRange;
    }

    public float GetSaturationRange()
    {
        return saturationRange;
    }

    public float GetBrightnessRange()
    {
        return brightnessRange;
    }

    public float GetHueInfluence()
    {
        return hueInfluencePercent;
    }

    public float GetSaturationInfluence()
    {
        return saturationInfluencePercent;
    }

    public float GetBrightnessInfluence()
    {
        return brightnessInfluencePercent;
    }

    public float GetFinalOutput()
    {
        return finalOutput;
    }

    #endregion

    #region PublicSetters
    public void SetMarkedPosition(GameObject obj)
    {
        markedPosition = obj;
    }

    public void SetIsUpdating(bool newState)
    {
        isUpdating = newState;
    }

    public void SetIsDebugging(bool newState)
    {
        isDebugging = newState;
    }

    public void SetColor(Color newColor)
    {
        targetColor = newColor;
    }

    public void SetRangeValues(float newHueRange, float newSaturationRange, float newBrightnessRange)
    {
        if (newHueRange < 0)
        {
            newHueRange = 0;
        }
        else if (newHueRange > 360)
        {
            newHueRange = 360;
        }

        hueRange = newHueRange;

        if (newSaturationRange < 0)
        {
            newSaturationRange = 0;
        }
        else if (newSaturationRange > 100)
        {
            newSaturationRange = 100;
        }

        saturationRange = newSaturationRange;

        if (newBrightnessRange < 0)
        {
            newBrightnessRange = 0;
        }
        else if (newBrightnessRange > 100)
        {
            newBrightnessRange = 100;
        }

        brightnessRange = newBrightnessRange;
    }

    public void SetInfluenceValues(float newHueInfluence, float newSaturationInfluence, float newBrightnessInfluence)
    {
        if (newHueInfluence < 0)
        {
            newHueInfluence = 0;
        }
        else if (newHueInfluence > 1)
        {
            newHueInfluence = 1;
        }

        hueInfluencePercent = newHueInfluence;

        if (newSaturationInfluence < 0)
        {
            newSaturationInfluence = 0;
        }
        else if (newSaturationInfluence > 1)
        {
            newSaturationInfluence = 1;
        }

        hueInfluencePercent = newSaturationInfluence;

        if (newBrightnessInfluence < 0)
        {
            newBrightnessInfluence = 0;
        }
        else if (newBrightnessInfluence > 1)
        {
            newBrightnessInfluence = 1;
        }

        hueInfluencePercent = newBrightnessInfluence;
    }

    #endregion PublicSetters
}