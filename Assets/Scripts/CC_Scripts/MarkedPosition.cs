/* Courtesy of Color, Unity Tool for Game Devs
 * Creator - Austin Foulks
 * Date of Conception - November 30, 2022
 * Edit the code however you need to fit your project
*/

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MarkedPosition : MonoBehaviour
{
    [Tooltip("Set false if the MP is not in use.")] [SerializeField] private bool isUpdating;
    [Tooltip("Set true if this object will never move.")] [SerializeField] private bool isStatic;
    [Tooltip("Enables drawing the ray and debug messages.")] [SerializeField] private bool isDebugging;
    [Tooltip("Set false if you want to get the sprite's texture instead.")] [SerializeField] private bool isGettingMaterial;

    [Tooltip("Get the color of collider X from the array of colliders.")] [SerializeField] [Range(1, 100)] private int collisionToGet;

    private enum State { Forward, Backward, Up, Down, Left, Right };
    [Tooltip("Forward/Backward is Z axis, Up/Down is Y axis, Left/Right is X axis.")] [SerializeField] private State directionOfRay;

    [Tooltip("The number of units the ray will travel.")] [SerializeField] [Range(1f, 100000f)] private float rayDistance;

    private Ray2D ray;
    private RaycastHit2D[] rayCollisions;
    private Color rGB;
    private float hue;
    private float saturation;
    private float brightness;
    
    private void Start()
    {
        ray.origin = gameObject.transform.position;

        switch (directionOfRay)
        {
            case State.Forward:
                ray.direction = gameObject.transform.forward;
                break;
            case State.Backward:
                ray.direction = gameObject.transform.forward * -1;
                break;
            case State.Up:
                ray.direction = gameObject.transform.up;
                break;
            case State.Down:
                ray.direction = gameObject.transform.up * -1;
                break;
            case State.Left:
                ray.direction = gameObject.transform.right * -1;
                break;
            case State.Right:
                ray.direction = gameObject.transform.right;
                break;
            default:
                Debug.Log("Ray direction is unclear. Defaulting to Forward.");
                ray.direction = gameObject.transform.forward;
                break;
        }
    }

    private void Update()
    {
        if (!isStatic)
        {
            ray.origin = gameObject.transform.position;
        }

        if (isDebugging)
        {
            Debug.DrawRay(ray.origin, ray.direction, Color.red, 0.1f);
        }

        if (isUpdating)
        {
            PerformRaycast();
        }
    }

    // Shoots ray, gets all collisions, get material of collision, or get texture/pixel hit and get color of pixel
    private void PerformRaycast()
    {
        // Work within the bounds of array.length
        int tempCollisionToGet = collisionToGet - 1;

        // To get exact pixel on the colliders texture that the ray collided with
        Vector2 pixelCoordinates = ray.origin;

        // Get an array of all collisions
        rayCollisions = Physics2D.RaycastAll(pixelCoordinates, ray.direction, rayDistance);
        
        // Bail if there is nothing being hit by the ray
        if (rayCollisions.Length == 0)
        {
            if (isDebugging)
            {
                Debug.Log("Ray did not hit anything.");
            }
            return;
        }

        if (isDebugging)
        {
            foreach (RaycastHit2D collision in rayCollisions)
            {
                Debug.Log("Hit Collider: " + collision.collider.name);
            }
        }
        
        // Default to first collision if value is outside the array
        if (tempCollisionToGet >= rayCollisions.Length)
        {
            tempCollisionToGet = 0;

            if (isDebugging)
            {
                Debug.Log("collisionToGet was out of index. Getting first collision.");
            }
        }
        
        // Get sprite -> texture -> pixel color at coords
        Sprite sprite = null;
        Material material = null;

        // Check for sprite on parent and child objects
        if (rayCollisions[tempCollisionToGet].transform.GetComponent<SpriteRenderer>())
        {
            sprite = rayCollisions[tempCollisionToGet].transform.GetComponent<SpriteRenderer>().sprite;
            material = rayCollisions[tempCollisionToGet].transform.GetComponent<SpriteRenderer>().material;
        }
        else if (rayCollisions[tempCollisionToGet].transform.GetComponentInChildren<SpriteRenderer>())
        {
            sprite = rayCollisions[tempCollisionToGet].transform.GetComponentInChildren<SpriteRenderer>().sprite;
            material = rayCollisions[tempCollisionToGet].transform.GetComponentInChildren<SpriteRenderer>().material;
        }

        if (isDebugging && !isGettingMaterial)
        {
            Debug.Log("Sprite: " + sprite);
        }
        else if (isDebugging && isGettingMaterial)
        {
            Debug.Log("Material: " + material);
        }

        // If a sprite was found, get its texture's color at ray.origin when the ray was cast
        if (sprite != null && !isGettingMaterial)
        {
            rGB = sprite.texture.GetPixel((int)pixelCoordinates.x, (int)pixelCoordinates.y);
        }
        // Expecting the material to be a solid color
        else if (material != null & isGettingMaterial)
        {
            rGB = material.color;
        }
            
        if (isDebugging)
        {
            Debug.Log("R: " + rGB.r * 255 + ", G: " + rGB.g * 255 + ", B: " + rGB.b * 255);
        }

        ConvertRGBToHSB();
    }

    private void ConvertRGBToHSB()
    {
        // RGB and HSB are on a scale from 0 to 1, but that's hard to interpret
        Color.RGBToHSV(rGB, out float h, out float s, out float b);

        // Setting HSB to relevant values
        hue = h * 360;
        saturation = s * 100;
        brightness = b * 100;

        if (isDebugging)
        {
            Debug.Log("H: " + hue + ", S: " + saturation + ", B: " + brightness);
        }
    }

    public float GetHue()
    {
        return hue;
    }

    public float GetSaturation()
    {
        return saturation;
    }

    public float GetBrightness()
    {
        return brightness;
    }
}