using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Controller : MonoBehaviour
{
    // Reference to a pawn object
    public Pawn pawn;

    // Start is called before the first frame update
    public virtual void Start()
    {}

    // Update is called once per frame
    public virtual void Update()
    {}

    // Used to process player/AI input
    public virtual void ProcessInputs()
    {}
}