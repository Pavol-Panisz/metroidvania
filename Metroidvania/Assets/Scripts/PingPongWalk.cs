#region README
/* A simple script that makes the gameObject
 * walk in one direction. When it hits a wall
 * or is about to fall off, it flips the
 * direction.
 */

#endregion README

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//needs a rigidbody to drive its velocity and detect collisions with the
//floor.
[RequireComponent(typeof(Rigidbody2D))] 

public class PingPongWalk : MonoBehaviour
{
    //variables
    [SerializeField] private float walkSpeed;
    [SerializeField] [Range(-1, 1)] private int direction=1; 

    //states
    private bool wasTouchingLastF=false;

    //cached components
    [Tooltip("The trigger collider which detects when the gameObject is about to fall off a ledge or hit a wall. "+
        "It has to be in front of the gameObject, touching the floor. Works only on flat ground.")]
    [SerializeField] private Collider2D detectCol;
    private Rigidbody2D rb;
    private Transform tr;

    private void Awake() {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable() {
        rb.velocity = new Vector3(walkSpeed*direction, 0f, 0f);
    }
    private void OnDisable() {
        rb.velocity = new Vector3(0f, 0f, 0f);
    }

    void FixedUpdate() {
        bool isTouching = detectCol.IsTouchingLayers(LayerMask.GetMask("Ground"));
        //Debug.Log("is touching this f " + isTouching.ToString());

        //if stopped touching the ground this frame, change the walk direction
        if (wasTouchingLastF && (!isTouching)) {
            
            direction *= -1;
            rb.velocity = new Vector3(walkSpeed*(float)direction, 0f, 0f);

            //not sure why we need to set it to true
            wasTouchingLastF = true;
            return;
        }
        wasTouchingLastF = isTouching;

        tr.localScale = new Vector3(Mathf.Sign(rb.velocity.x)*Mathf.Abs(tr.localScale.x),
                                            tr.localScale.y,
                                            tr.localScale.z);
    }

}
