using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPawn : Pawn
{
    // For UI
    [SerializeField] public Text rgbText;
    [SerializeField] public Text hsvText;
    [SerializeField] public Text speedText;

    // Start is called before the first frame update
    public override void Start()
    {
        // Use base."Name"(); if you want to call the parent's function
        base.Start();
    }

    public override void Update()
    {
        base.Update();

        if (rgbText != null)
        {
            rgbText.text = "RGB: ";
        }
        if (hsvText != null)
        {
            hsvText.text = "HSV: ";
        }
        if (speedText != null)
        {
            speedText.text = "Speed Up: %";
        }

        // Speed is constantly fluctuating
        ChangeSpeed(1);
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

    public override void ChangeSpeed(float sliderOutput)
    {
        moveSpeed = normalMoveSpeed + (speedBoost * sliderOutput);
    }
}