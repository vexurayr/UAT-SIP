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
    #region Variables
    [Tooltip("Reference an object for MP to use its position if MP can't be attached as a child object.")]
        [SerializeField] private GameObject objectReference;

    [Tooltip("Set false if the MP is not in use.")] [SerializeField] private bool isUpdating;
    [Tooltip("Set true if this object will never move.")] [SerializeField] private bool isStatic;
    [Tooltip("Enables drawing the ray and debug messages.")] [SerializeField] private bool isDebugging;

    private enum Renderer { Texture, TriMaterial, WholeMaterial };
    [Tooltip("Texture = The color of the pixel, TriMaterial = The primary color of the tri on a mesh (NOT READY, INCOMPATIBLE WITH SPRITES), " +
        "WholeMaterial = The primary color of the sprite.")] [SerializeField] private Renderer renderMethod;

    [Tooltip("Get the color of collider X from the array of colliders.")] [SerializeField] [Range(1, 100)] private int collisionToGet;

    private enum Direction { Forward, Backward, Up, Down, Left, Right };
    [Tooltip("Forward/Backward = Z axis, Up/Down = Y axis, Left/Right = X axis.")] [SerializeField] private Direction directionOfRay;

    [Tooltip("The number of units the ray will travel.")] [SerializeField] [Range(0f, 100000f)] private float rayDistance;

    private Ray2D ray;
    private RaycastHit2D[] rayCollisions;
    private int tempCollisionToGet;
    // Get sprite -> texture -> pixel color at coords
    private Sprite sprite = null;
    private Mesh mesh = null;
    private Material material = null;
    private Color rGB;
    private float hue;
    private float saturation;
    private float brightness;

    #endregion Variables

    #region MonoBehaviours
    private void Start()
    {
        if (objectReference)
        {
            ray.origin = objectReference.transform.position;
        }
        else
        {
            ray.origin = gameObject.transform.position;
        }

        switch (directionOfRay)
        {
            case Direction.Forward:
                ray.direction = gameObject.transform.forward;
                break;
            case Direction.Backward:
                ray.direction = gameObject.transform.forward * -1;
                break;
            case Direction.Up:
                ray.direction = gameObject.transform.up;
                break;
            case Direction.Down:
                ray.direction = gameObject.transform.up * -1;
                break;
            case Direction.Left:
                ray.direction = gameObject.transform.right * -1;
                break;
            case Direction.Right:
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
            if (objectReference)
            {
                ray.origin = objectReference.transform.position;
            }
            else
            {
                ray.origin = gameObject.transform.position;
            }
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

    #endregion MonoBehaviours

    #region PrivateFunctions
    // Shoots ray, gets all collisions, gets material of tri hit or gets texture/pixel hit, get color of tri/pixel
    private void PerformRaycast()
    {
        // Work within the bounds of array.length
        tempCollisionToGet = collisionToGet - 1;

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

        switch (renderMethod)
        {
            case Renderer.Texture:
                GetTexture(pixelCoordinates);

                break;
            case Renderer.TriMaterial:
                GetTriMaterial();

                break;
            case Renderer.WholeMaterial:
                GetWholeMaterial();

                break;
            default:
                Debug.Log("Render method unclear. Defaulting to WholeMaterial.");
                GetWholeMaterial();

                break;
        }
            
        if (isDebugging)
        {
            Debug.Log("R: " + rGB.r * 255 + ", G: " + rGB.g * 255 + ", B: " + rGB.b * 255);
        }

        ConvertRGBToHSB();
    }

    private void GetTexture(Vector2 pixelCoordinates)
    {
        // Check for sprite on parent and child objects
        if (rayCollisions[tempCollisionToGet].collider.GetComponent<SpriteRenderer>())
        {
            sprite = rayCollisions[tempCollisionToGet].collider.GetComponent<SpriteRenderer>().sprite;
        }
        else if (rayCollisions[tempCollisionToGet].collider.GetComponentInChildren<SpriteRenderer>())
        {
            sprite = rayCollisions[tempCollisionToGet].collider.GetComponentInChildren<SpriteRenderer>().sprite;
        }

        if (isDebugging)
        {
            Debug.Log("Sprite: " + sprite);
        }

        // If a sprite was found, get its texture's color at the pixel the ray hit the texture
        if (sprite != null)
        {
            rGB = sprite.texture.GetPixel((int)pixelCoordinates.x, (int)pixelCoordinates.y);
        }
    }

    private void GetTriMaterial()
    {
        // Get the mesh of the target collider
        mesh = GetMeshOf(rayCollisions[tempCollisionToGet].collider.gameObject);

        if (!mesh)
        {
            return;
        }

        // Get the tris of each mesh
        int[] trisHit = new int[]
        {
            mesh.triangles[rayCollisions[tempCollisionToGet].collider.shapeCount * 3],
            mesh.triangles[rayCollisions[tempCollisionToGet].collider.shapeCount * 3 + 1],
            mesh.triangles[rayCollisions[tempCollisionToGet].collider.shapeCount * 3 + 2]
        };

        // Get the tris of each submesh
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            int[] subMeshTris = mesh.GetTriangles(i);

            for (int j = 0; j < subMeshTris.Length; j += 3)
            {
                if (subMeshTris[j] == trisHit[0] &&
                    subMeshTris[j + 1] == trisHit[1] &&
                    subMeshTris[j + 2] == trisHit[2])
                {
                    if (isDebugging)
                    {
                        Debug.Log("Tri index: " + rayCollisions[tempCollisionToGet].collider.shapeCount + ", SubMesh index: " +
                            i + ", SubMesh Tri index: " + j / 3);
                    }

                    rGB = mesh.colors32[i];
                }
            }
        }
    }

    private void GetWholeMaterial()
    {
        // Check for sprite on parent and child objects
        if (rayCollisions[tempCollisionToGet].collider.GetComponent<SpriteRenderer>())
        {
            material = rayCollisions[tempCollisionToGet].collider.GetComponent<SpriteRenderer>().material;
        }
        else if (rayCollisions[tempCollisionToGet].collider.GetComponentInChildren<SpriteRenderer>())
        {
            material = rayCollisions[tempCollisionToGet].collider.GetComponentInChildren<SpriteRenderer>().material;
        }

        if (isDebugging)
        {
            Debug.Log("Material: " + material);
        }

        // If a sprite was found, get its primary color
        if (material != null)
        {
            rGB = material.color;
        }
    }

    private Mesh GetMeshOf(GameObject obj)
    {
        MeshFilter meshFilter;

        if (!obj)
        {
            if (isDebugging)
            {
                Debug.Log("No valid object provided.");
            }

            return (Mesh)null;
        }

        if (!obj.GetComponent<MeshFilter>())
        {
            if (isDebugging)
            {
                Debug.Log("No MeshFilter component could be found.");
            }

            return (Mesh)null;
        }

        meshFilter = obj.GetComponent<MeshFilter>();

        Mesh mesh = meshFilter.sharedMesh;

        if (!mesh)
        {
            mesh = meshFilter.mesh;
        }

        if (!mesh)
        {
            if (isDebugging)
            {
                Debug.Log("No Mesh could be found.");
            }

            return (Mesh)null;
        }

        return mesh;
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

    #endregion PrivateFunctions

    #region PublicGetters
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

    #endregion PublicGetters

    #region PublicSetters
    public void SetIsUpdating(bool newState)
    {
        isUpdating = newState;
    }

    public void SetIsStatic(bool newState)
    {
        isStatic = newState;
    }

    public void SetIsDebugging(bool newState)
    {
        isDebugging = newState;
    }

    public void SetRenderMethod(string newMethod)
    {
        if (newMethod == "Texture")
        {
            renderMethod = Renderer.Texture;
        }
        else if (newMethod == "TriMaterial")
        {
            renderMethod = Renderer.TriMaterial;
        }
        else if (newMethod == "WholeMaterial")
        {
            renderMethod = Renderer.WholeMaterial;
        }
        else
        {
            Debug.Log("Render method (" + newMethod + ") is not a valid option.");
        }
    }

    public void SetCollisionToGet(int newCollision)
    {
        if (newCollision < 0)
        {
            newCollision = 0;
        }
        else if (newCollision > 100)
        {
            newCollision = 100;
        }

        collisionToGet = newCollision;
    }

    public void SetDirectionOfRay(string newDirection)
    {
        if (newDirection == "Forward")
        {
            directionOfRay = Direction.Forward;
        }
        else if (newDirection == "Backward")
        {
            directionOfRay = Direction.Backward;
        }
        else if (newDirection == "Up")
        {
            directionOfRay = Direction.Up;
        }
        else if (newDirection == "Down")
        {
            directionOfRay = Direction.Down;
        }
        else if (newDirection == "Left")
        {
            directionOfRay = Direction.Left;
        }
        else if (newDirection == "Right")
        {
            directionOfRay = Direction.Right;
        }
        else
        {
            Debug.Log("Ray direction (" + newDirection + ") is not a valid option.");
        }
    }

    public void SetRayDistance(float newDistance)
    {
        if (newDistance < 0)
        {
            newDistance = 0;
        }
        else if (newDistance > 100000)
        {
            newDistance = 100000;
        }

        rayDistance = newDistance;
    }

    #endregion PublicSetters
}