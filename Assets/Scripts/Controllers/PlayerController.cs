using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Allows objects of this class to appear in the editor
[System.Serializable]
public class PlayerController : Controller
{
    public GameObject characterSprite;
    public ParticleSystem sparkParticles;

    // Creates a new field representing a button that can be set in the editor
    public KeyCode keyUp = KeyCode.W;
    public KeyCode keyDown = KeyCode.S;
    public KeyCode keyLeft = KeyCode.A;
    public KeyCode keyRight = KeyCode.D;
    public KeyCode keyLeftClick = KeyCode.Mouse0;
    public bool isCursorVisible;

    private Animator characterAnimator;
    private float animationSpeed;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        if (isCursorVisible)
        {
            // Makes the cursor invisible
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
        }

        if (characterSprite.GetComponent<Animator>())
        {
            characterAnimator = characterSprite.GetComponent<Animator>();
        }
        else
        {
            Debug.Log("Check characterSprite for animator component.");
        }

        animationSpeed = characterAnimator.speed;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        ProcessInputs();
    }

    // Function that uses the keycodes to access the pawn referrence in the parent class
    // to run the functions in the PlayerPawn class
    public override void ProcessInputs()
    {
        // Get Key runs as long as it's held, Get Key Down runs once on being pressed, Get Key Up runs once on being released
        if (Input.GetKey(keyUp))
        {
            // Tells the given pawn to move forward, whatever pawn this script is attached to
            pawn.MoveUp();
            characterAnimator.speed = pawn.GetCurrentSpeed() / 4;
            characterAnimator.SetBool("isMoving", true);
        }
        if (Input.GetKey(keyDown))
        {
            pawn.MoveDown();
            characterAnimator.speed = pawn.GetCurrentSpeed() / 4;
            characterAnimator.SetBool("isMoving", true);
        }
        if (Input.GetKey(keyLeft))
        {
            pawn.MoveLeft();
            characterAnimator.speed = pawn.GetCurrentSpeed() / 4;
            characterAnimator.SetBool("isMoving", true);
        }
        if (Input.GetKey(keyRight))
        {
            pawn.MoveRight();
            characterAnimator.speed = pawn.GetCurrentSpeed() / 4;
            characterAnimator.SetBool("isMoving", true);
        }
        if (Input.GetKeyDown(keyLeftClick))
        {
            Shoot();
        }

        if (!Input.anyKey)
        {
            characterAnimator.SetBool("isMoving", false);
        }

        var main = sparkParticles.main;
        main.startSpeed = pawn.GetCurrentSpeed() / 4;
    }

    private void Shoot()
    {
        if (GetComponent<SoldierShooter>() != null)
        {
            GetComponent<SoldierShooter>().Shoot();
        }
    }
}