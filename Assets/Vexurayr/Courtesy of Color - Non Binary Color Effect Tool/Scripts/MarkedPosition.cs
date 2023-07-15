/* Courtesy of Color, Unity Tool for Game Devs
 * Creator - Austin Foulks
 * Date of Conception - November 30, 2022
*/

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CourtesyOfColor
{
    public class MarkedPosition : MonoBehaviour
    {
        #region Variables
        [Tooltip("Reference an object for MP to use its position if MP can't be attached as a child object.")]
        [SerializeField] private GameObject objectReference;

        [Tooltip("Set false if the MP is not in use.")][SerializeField] private bool isUpdating;
        [Tooltip("Set true if this object will never move.")][SerializeField] private bool isStatic;
        [Tooltip("Enables the debug ray (make sure Gizmos are enabled in the Game view) and debug messages.")]
        [SerializeField] private bool isDebugging;

        // Cut TriMaterial method - not suitable with sprites and hard to test in 2D project
        private enum Renderer { Texture, WholeMaterial };
        [Tooltip("Texture = The color of the pixel, WholeMaterial = The primary color of the sprite.")]
        [SerializeField] private Renderer renderMethod;

        [Tooltip("Get the color of collider X from the array of colliders.")][SerializeField][Range(1, 100)] private int collisionToGet;
        [Tooltip("If false, the first collision will be default.")][SerializeField] private bool isLastCollisionDefault;
        [Tooltip("If true, the ray will always return the last collider in a series of collisions.")]
        [SerializeField] private bool alwaysGetLastCollision;

        private enum Direction { Forward, Backward, Up, Down, Left, Right };
        [Tooltip("Forward/Backward = Z axis, Up/Down = Y axis, Left/Right = X axis.")][SerializeField] private Direction directionOfRay;

        [Tooltip("The number of units the ray will travel.")][SerializeField][Range(0f, 100000f)] private float rayDistance;

        private Ray2D ray;
        private RaycastHit2D[] rayCollisions;
        private int tempCollisionToGet2D;
        // Get sprite -> texture -> pixel color at coords
        private Sprite sprite = null;
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
                Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red, 0.1f, false);
            }

            if (isUpdating)
            {
                PerformRaycast2D();
            }
        }

        #endregion MonoBehaviours

        #region PrivateFunctions
        // Shoots ray, gets all collisions, gets material of tri hit or gets texture/pixel hit, get color of tri/pixel
        private void PerformRaycast2D()
        {
            // Work within the bounds of array.length
            tempCollisionToGet2D = collisionToGet - 1;

            // Get an array of all collisions
            rayCollisions = Physics2D.RaycastAll(ray.origin, ray.direction, rayDistance);

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

            if (alwaysGetLastCollision)
            {
                tempCollisionToGet2D = rayCollisions.Length - 1;
            }
            else
            {
                // Default to first/last collision if collisionToGet is outside the array
                if (tempCollisionToGet2D >= rayCollisions.Length)
                {
                    if (isLastCollisionDefault)
                    {
                        tempCollisionToGet2D = rayCollisions.Length;

                        if (isDebugging)
                        {
                            Debug.Log("collisionToGet was out of index. Getting last collision.");
                        }
                    }
                    else
                    {
                        tempCollisionToGet2D = 0;

                        if (isDebugging)
                        {
                            Debug.Log("collisionToGet was out of index. Getting first collision.");
                        }
                    }
                }
            }

            switch (renderMethod)
            {
                case Renderer.Texture:
                    // Make sure to have read/write enabled on your textures in the Inspector
                    GetTexture();

                    break;
                case Renderer.WholeMaterial:
                    GetWholeMaterial();

                    break;
                default:
                    Debug.Log("Render method unclear. Defaulting to WholeMaterial.");
                    GetWholeMaterial();

                    break;
            }

            ConvertRGBToHSB();
        }

        private void GetTexture()
        {
            Collider2D hitCollider = rayCollisions[tempCollisionToGet2D].collider;

            // Check for sprite on parent and child objects
            if (hitCollider.GetComponent<SpriteRenderer>())
            {
                sprite = hitCollider.GetComponent<SpriteRenderer>().sprite;
            }
            else if (hitCollider.GetComponentInChildren<SpriteRenderer>())
            {
                sprite = hitCollider.GetComponentInChildren<SpriteRenderer>().sprite;
            }

            if (isDebugging)
            {
                Debug.Log("Sprite: " + sprite);
            }

            // If a sprite was found, get its texture's color at the pixel the ray hit the texture
            if (sprite != null)
            {
                // The texture's pivot point is most likely in its center, using top right
                float rightX = hitCollider.bounds.max.x;
                float leftX = hitCollider.bounds.min.x;
                float topY = hitCollider.bounds.max.y;
                float bottomY = hitCollider.bounds.min.y;

                // 0-1 scale current position & topLeftCornerPos using the hitCollider's size
                float scaleX01 = Mathf.InverseLerp(leftX, rightX, ray.origin.x);
                float scaleY01 = Mathf.InverseLerp(bottomY, topY, ray.origin.y);

                scaleX01 = Mathf.Clamp01(scaleX01);
                scaleY01 = Mathf.Clamp01(scaleY01);

                // Multiply 0-1 scale by the size of the texture for a pixel
                float pixelCoordX = scaleX01 * sprite.texture.width;
                float pixelCoordY = scaleY01 * sprite.texture.height;

                rGB = sprite.texture.GetPixel((int)pixelCoordX, (int)pixelCoordY);

                if (isDebugging)
                {
                    Debug.Log("Middle X: " + hitCollider.transform.position.x + ", Right X: " + rightX + ", Left X: " + leftX +
                    "\nMiddle Y: " + hitCollider.transform.position.y + ", Top Y: " + topY + ", Bottom Y: " + bottomY);

                    Debug.Log("Pixel X: " + (int)pixelCoordX + ", Pixel Y: " + (int)pixelCoordY);
                }
            }
        }

        private void GetWholeMaterial()
        {
            // Check for sprite on parent and child objects
            if (rayCollisions[tempCollisionToGet2D].collider.GetComponent<SpriteRenderer>())
            {
                material = rayCollisions[tempCollisionToGet2D].collider.GetComponent<SpriteRenderer>().material;
            }
            else if (rayCollisions[tempCollisionToGet2D].collider.GetComponentInChildren<SpriteRenderer>())
            {
                material = rayCollisions[tempCollisionToGet2D].collider.GetComponentInChildren<SpriteRenderer>().material;
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
        public float GetRed()
        {
            return rGB.r;
        }

        public float GetGreen()
        {
            return rGB.g;
        }

        public float GetBlue()
        {
            return rGB.b;
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
}