#region README
/* contains various useful methods which affect
 * the positions of gameObjects in various ways
 */
 #endregion README

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformUtility : MonoBehaviour
{
    /// <summary>
    /// Launches the rigidbody into the height (s) in (t) seconds
    /// </summary>
    public static void ThrowVertically(ref Rigidbody2D rb, float f)
    {
        Vector2 vel = new Vector2(0f, f);
        rb.velocity = vel;
    }

    public static void LaunchFromEnemy(Rigidbody2D rb, Vector3 enemyPos, Vector2 launchVect) {
        Transform tr = rb.GetComponent<Transform>();

        if ((tr.position - enemyPos).x <= 0f) { //if we're on the enemy's left

            //if we're looking left, look right (this makes the animation look better)
            if (tr.localScale.x < 0) {
                tr.localScale = new Vector3(-tr.localScale.x,
                                            tr.localScale.y,
                                            tr.localScale.z);
            }
            
            //we're on the enemy's left, so flip the vector x component
            Vector2 leftVect = new Vector2(-launchVect.x, launchVect.y);
            rb.velocity = leftVect;
        }
        else { //if we're on the enemy's right

            //if we're looking right, look left (this makes the animation look better)
            if (tr.localScale.x > 0) {
                tr.localScale = new Vector3(-tr.localScale.x,
                                            tr.localScale.y,
                                            tr.localScale.z);
            }

            rb.velocity = launchVect;
        }
    }

    public static void RotateTransformTowards(Transform tr, Vector3 v) {
        Vector2 dir = v - tr.position;
        tr.right = dir;
    }

    //TODO method for rotating transform towards target tr
}

