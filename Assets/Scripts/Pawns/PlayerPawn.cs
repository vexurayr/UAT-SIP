using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CourtesyOfColor;

public class PlayerPawn : Pawn
{
    // For UI
    public Text targetHSBText;
    public Text rangeHSBText;
    public Text influenceHSBText;
    public Text currentHSBText;
    public Text controllerOutputText;
    public Text currentSpeedText;
    public ColorController colorController;

    private float currentSpeedUp;

    // Start is called before the first frame update
    public override void Start()
    {
        // Use base."Name"(); if you want to call the parent's function
        base.Start();
    }

    public override void Update()
    {
        base.Update();

        if (targetHSBText != null)
        {
            targetHSBText.text = "Target H: " + colorController.GetTargetHue() + ", S: " + colorController.GetTargetSaturation()
                + ", B: " + colorController.GetTargetBrightness();
        }
        if (rangeHSBText != null)
        {
            rangeHSBText.text = "Range H: " + colorController.GetHueRange() + ", S: " + colorController.GetSaturationRange()
                + ", B: " + colorController.GetBrightnessRange();
        }
        if (influenceHSBText != null)
        {
            influenceHSBText.text = "Influence H: " + colorController.GetHueInfluence() + ", S: " + colorController.GetSaturationInfluence()
                + ", B: " + colorController.GetBrightnessInfluence();
        }
        if (currentHSBText != null)
        {
            currentHSBText.text = "Current H: " + colorController.GetMPHue() + ", S: " + colorController.GetMPSaturation()
                + ", B: " + colorController.GetMPBrightness();
        }
        if (controllerOutputText != null)
        {
            controllerOutputText.text = "Controller Output: " + colorController.GetFinalOutput();
        }
        if (currentSpeedText != null)
        {
            currentSpeedText.text = "Current Speed: " + moveSpeed;
        }
    }

    public override void MoveUp()
    {
        // Calls the Move function in the mover class, but
        // the function in PlayerMover is run because that is
        // the script attached to the pawn
        mover.Move(transform.up, moveSpeed);
    }

    public override void MoveDown()
    {
        mover.Move(-transform.up, moveSpeed);
    }

    public override void MoveLeft()
    {
        mover.Move(-transform.right, moveSpeed);
    }

    public override void MoveRight()
    {
        mover.Move(transform.right, moveSpeed);
    }

    // Takes in float between 0-1, meaning the speed boost is somewhere between 0-100% active
    public override void ChangeSpeed(float colorControllerOutput)
    {
        currentSpeedUp = speedBoost * colorControllerOutput;

        moveSpeed = normalMoveSpeed + currentSpeedUp;
    }
}