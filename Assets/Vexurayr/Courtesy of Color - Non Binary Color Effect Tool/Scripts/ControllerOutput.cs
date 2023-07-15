/* Courtesy of Color, Unity Tool for Game Devs
 * Creator - Austin Foulks
 * Date of Conception - November 30, 2022
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CourtesyOfColor
{
    [Serializable]
    public class ControllerOutput : UnityEvent<float>
    { }
}