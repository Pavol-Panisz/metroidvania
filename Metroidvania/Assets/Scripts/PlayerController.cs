#region README
/* Gives the player the ability to run, jump and climb.
 * Delegates announce when each of these actions commence
 * or stop.
 */
 #endregion README

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{  
    //config
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpSpeed = 7f;
    [SerializeField] private float climbSpeed = 1f;
    
    //states
    bool hasControl = true;
    bool hadControlLastF = true;
    bool isOnGround = false; 
    bool wasOnGroundLastF = false;
    bool isInLadder = false;
    bool wasInLadderLastF = false;
    bool movingInLadderThisF = false;
    bool movingInLadderLastF = false;
    bool holdingVertical = false;
    
    //input states
    bool holdingRunLastF = false;
    bool holdingRun=false;
    bool holdingJump=false;
    float xAxis, yAxis;

    //cached variables
    float originalGravitySc;

    //cached component references
    private Rigidbody2D rb;
    private Transform tr;
    public Animator anim;
    private Collider2D col;
    public Collider2D feetCol;
    
    //delegates
    public delegate void emptyDlg();
    public event emptyDlg OnStartedRunning;
    public event emptyDlg OnStoppedRunning;
    public event emptyDlg OnHitGround;
    public event emptyDlg OnStartedClimbing;
    public event emptyDlg OnStoppedClimbing;
    public event emptyDlg OnJumped;
    public event emptyDlg OnClimbedOnce;
    public event emptyDlg OnRegainedControl;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        
        originalGravitySc = rb.gravityScale;
    }

    void FixedUpdate() {
        
        isOnGround = feetCol.IsTouchingLayers(LayerMask.GetMask("Ground"));
        isInLadder = col.IsTouchingLayers(LayerMask.GetMask("Ladder"));
        
        fuClimbing();
        fuRunning();
        fuJumping(jumpSpeed); 
        
        //if the player hit the ground this frame, stop his x- & y-velocity. 
        #region explanation
        //We need to do this because otherwise the player would keep on sliding, because he has no friction.
        //Why no friction, you ask? So that he can't wall- jump. Why is he be able to wall-jump with
        //friction? -> Example: When you jump to the right and hit a wall and keep holding the "go right"
        //button, the player's and wall's colliders are touching every single frame. Since there's 
        //friction between them, the rigidbody inhibits movement. You could, for example, make the player
        //slide down walls if you apply just a little friction. 

        //added 13.6.: Why stop the y-vel aswell? It's because while jumping and hugging a wall, once
        //the player would hit that wall's corner, the feet collider would touch that corner, hence
        //setting isOnGround to true. Since the player is holding the jump button at this time,
        //they'd jump again. Jumping is just adding jump-velocity to the current velocity, so the 
        //player would jump really high.
        #endregion 
        if (!wasOnGroundLastF && isOnGround) {
            rb.velocity = Vector2.zero;
            if (hasControl) OnHitGround?.Invoke(); //the hasControl check is to make respawning silent
        }

        #region start_stop_delegates

        //if started moving in the ladder this frame
        if (movingInLadderThisF && !movingInLadderLastF && hasControl) OnStartedClimbing?.Invoke();

        //if we latched onto the ladder but didn't move
        if (isInLadder && ! wasInLadderLastF) OnClimbedOnce?.Invoke();

        //if stopped moving in the ladder this frame
        if (!movingInLadderThisF && movingInLadderLastF) OnStoppedClimbing?.Invoke();

        //if started holding run this frame while on ground OR hit ground while holding run
        if (hasControl) {
            if ((holdingRun && !holdingRunLastF && isOnGround) || 
                (holdingRun && isOnGround && !wasOnGroundLastF)) OnStartedRunning?.Invoke(); 
        }

        //if stopped holding run this frame while on ground OR stopped being on ground this frame
        if ((!holdingRun && holdingRunLastF && isOnGround) || 
        (!isOnGround && wasOnGroundLastF)) OnStoppedRunning?.Invoke();
        #endregion start_stop_delegates

        holdingRunLastF = holdingRun;
        wasOnGroundLastF = isOnGround;
        wasInLadderLastF = isInLadder;
        hadControlLastF = hasControl;
        movingInLadderLastF = movingInLadderThisF;
    }


    void Update()
    {
        //update input info
        xAxis = Input.GetAxis("Horizontal"); //fetched in update, utilized also in fixed update
        yAxis = Input.GetAxis("Vertical");

        if (Mathf.Abs(xAxis) > Mathf.Epsilon) { //---running
            holdingRun = true;
        }
        else {
            holdingRun = false;
        }

        holdingVertical = false;
        if (yAxis > Mathf.Epsilon) { //--------------jumping
            holdingJump = true;
            holdingVertical = true;
        }
        else {
            holdingJump = false;
            holdingVertical = true;
        }

        if (isInLadder) { //-------------------------climbing
            anim.SetBool("isClimbing", true);
        }
        else {
            anim.SetBool("isClimbing", false);
        }
        
        //update Update()-neccessary stuff
        uRunning();
        uClimbing();
        uJumping();

    }

    private void TryFlipSprite() {
        if (!hasControl) {return;} 
        
        Vector3 scaleVect = tr.localScale; 
        float dirSign = Mathf.Sign(xAxis);
        tr.localScale = new Vector3(dirSign * Mathf.Abs(scaleVect.x), 
                                    scaleVect.y, 
                                    scaleVect.z);
    }

    private void uRunning() {
        if (holdingRun) {
            anim.SetBool("isRunning", true);
            TryFlipSprite();
        }
        else {
            anim.SetBool("isRunning", false);
        }

        anim.SetBool("isOnGround", isOnGround);
    }

    private void fuRunning() {

        if (hasControl) {
            float xVel = xAxis * speed; //no need to get xAxis in FixedUpdate

            //if running this frame
            if (holdingRun) {
                rb.velocity = new Vector2(xVel, rb.velocity.y);  
            }
            //if stopped running this frame
            else if (!holdingRun && holdingRunLastF) { 
                rb.velocity = new Vector2(0f, rb.velocity.y);
            }
        }
    }

    private void uJumping() {
        //nothing to do here 
    }

    private void fuJumping(float jumpSpeed, bool doForceJump=false) {
        if (hasControl) {
            //still not entirely sure, why this gets called twice per jump 
            //pressed - has to do with how its calculated
            if (holdingJump && (isOnGround || doForceJump)) { 
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                OnJumped?.Invoke();
            }

            //TODO Why is the player floaty when gravityScale = 1 ???
            //I wanna change it to 1 so that the launches-away from the
            //enemy are good.
            //Alternatively, I could increase the launch-vector's y
            //component.
        }
    }

    private void uClimbing() { //change the animation here
        if (rb.velocity == new Vector2(0f, 0f)) {
                anim.SetBool("isClimbIdle", true);
            }
            else {
                anim.SetBool("isClimbIdle", false);
            }
    }

    
    private void fuClimbing() {
        
        //TODO document this discovery!!!
        //does not work when collider is part of composite collider
        //isInLadder = ladderCol.OverlapPoint(col.bounds.center);
        
        //setting isInLadder in OnTriggerEnter/Exit2D would also not
        //work. Solution? Change the ladders' physics shape so that the
        //player's collider is always touching some ladder's surface,
        //meaning it can never be 'fully' in it. 
        
        if (isInLadder & hasControl) {
            rb.gravityScale = 0f;

            //if the player entered the ladder this frame or regained control this frame
            if (!wasInLadderLastF || !hadControlLastF) {
                rb.velocity = Vector2.zero;
            }
            //otherwise, handle the input
            else { 
                //if holding up or down
                if (Mathf.Abs(yAxis) > Mathf.Epsilon) {
                    rb.velocity = new Vector2(rb.velocity.x, yAxis*climbSpeed);
                }
                else { //if not holding up or down
                    rb.velocity = new Vector2(rb.velocity.x, 0f);
                }
            }

        }
        
        //if exit ladder this frame
        else if (wasInLadderLastF && !isInLadder) {
            fuJumping(jumpSpeed*0.75f, true); //make the player able to do a off-the-ladder jump.
        }
        else {
            rb.gravityScale = originalGravitySc;
        }

        movingInLadderThisF = true;
        //if not in the ladder OR in the ladder but not moving
        if ((!isInLadder) || (isInLadder && rb.velocity == Vector2.zero)) {
            movingInLadderThisF = false;
        }
    }

    public void StopControl() {
        hasControl = false;
    }

    public void RegainControl() {
        hasControl = true;
        anim.SetBool("isHurt", false);
        OnRegainedControl?.Invoke();
    }

}
