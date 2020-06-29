/* OwnUtility
 * A script containing various methods which I
 * frequently reuse
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OwnUtility
{
    /// <summary>
    /// Returns the angle (0 - 2*PI) between the two vectors.
    /// 0 rads is "east" and the angle grows counter-clockwise.
    /// </summary>
    /// <param name="from">Starting point</param>
    /// <param name="to">Ending point</param>
    public static float Angle(Vector2 from, Vector2 to)
    {
        float rad = Mathf.Asin((to.y - from.y)/Vector2.Distance(from, to));
        
        //quadrant II
        if (to.x <= from.x && to.y >= from.y) {
            rad = Mathf.PI - rad;
        }
        //quadrant III
        else if (to.x <= from.x && to.y <= from.y) {
            rad = -rad + Mathf.PI;
        }
        //quadrant IV
        else if (to.x >= from.x && to.y <= from.y) {
            rad = 2*Mathf.PI + rad;
        }

        return(rad);
    }

}
