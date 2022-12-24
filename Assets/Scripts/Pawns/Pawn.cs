using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Abstract means an object of this class will never be made or a function will never be called within that class
public abstract class Pawn : MonoBehaviour
{
    /// <summary>
    /// Variables that will determine how fast an inhereting object will move
    /// </summary>
    public float moveSpeed;

    // Amount that the speed will increase by at its maximum influence
    public float speedBoost;

    protected Mover mover;
    protected float normalMoveSpeed;

    // Start is called before the first frame update
    // Virtual means child classes can override this method
    // Protected keyword is necessary because no access keyword defaults to private
    // And we don't want outside scripts calling these functions
    public virtual void Start()
    {
        // Gives pawn objects a reference to the mover class
        mover = GetComponent<Mover>();

        normalMoveSpeed = moveSpeed;
    }

    public virtual void Update()
    {}

    public abstract void MoveUp();
    public abstract void MoveDown();
    public abstract void MoveLeft();
    public abstract void MoveRight();
    public abstract void ChangeSpeed(float sliderOutput);
}