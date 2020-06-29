using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{  
    #region variables_n_delegates
    //config
    [SerializeField] private float speed = 5f;
    [SerializeField] private float m_jumpSpeed = 7f;
    [SerializeField] private float climbSpeed = 1f;
    
    //states
    bool m_hasControl = true;
    bool m_hadControlLastF = true;
    bool m_isOnGround = false; 
    bool m_wasOnGroundLastF = false;
    bool m_isInLadder = false;
    bool m_wasInLadderLastF = false;
    bool m_movingInLadderThisF = false;
    bool m_movingInLadderLastF = false;
    bool m_holdingVertical = false;
    
    //input states
    bool m_holdingRun=false;
    bool m_holdingRunLastF = false;
    bool m_holdingJump=false;
    float m_xAxis, m_yAxis;

    //cached variables
    float m_originalGravitySc;

    //cached component references
    private Rigidbody2D rb;
    private Transform tr;
    public Animator anim;
    private Collider2D m_col;
    public Collider2D m_feetCol;
    
    //delegates
    public delegate void emptyDlg();
    public event emptyDlg OnStartedRunning;
    public event emptyDlg OnStoppedRunning;
    public event emptyDlg OnHitGround;
    public event emptyDlg OnStartedClimbing;
    public event emptyDlg OnStoppedClimbing;
    public event emptyDlg OnJumped;
    public event emptyDlg OnClimbedOnce;
    #endregion variables_n_delegates

    void FixedUpdate() {
        
        m_isOnGround = m_feetCol.IsTouchingLayers(LayerMask.GetMask("Ground"));
        m_isInLadder = m_col.IsTouchingLayers(LayerMask.GetMask("Ladder"));
        
        fuClimbing();
        fuRunning();
        fuJumping(m_jumpSpeed); //make this "speed-set" principle for all action-functions
        
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
        //setting m_isOnGround to true. Since the player is holding the jump button at this time,
        //they'd jump again. Jumping is just adding jump-velocity to the current velocity, so the 
        //player would jump really high.
        #endregion 

        if (!m_wasOnGroundLastF && m_isOnGround) {
            rb.velocity = new Vector2(0f, 0f/*rb.velocity.y*/);
            if (m_hasControl) OnHitGround?.Invoke(); //the m_hasControl check is to make respawning silent
        }

        #region start_stop_delegates

        //if started moving in the ladder this frame
        if (m_movingInLadderThisF && !m_movingInLadderLastF && m_hasControl) OnStartedClimbing?.Invoke();

        //if we latched onto the ladder but didn't move
        if (m_isInLadder && ! m_wasInLadderLastF) OnClimbedOnce?.Invoke();

        //if stopped moving in the ladder this frame
        if (!m_movingInLadderThisF && m_movingInLadderLastF) OnStoppedClimbing?.Invoke();

        //if started holding run this frame while on ground OR hit ground while holding run
        if (m_hasControl) {
            if ((m_holdingRun && !m_holdingRunLastF && m_isOnGround) || 
                (m_holdingRun && m_isOnGround && !m_wasOnGroundLastF)) OnStartedRunning?.Invoke(); 
        }

        //if stopped holding run this frame while on ground OR stopped being on ground this frame
        if ((!m_holdingRun && m_holdingRunLastF && m_isOnGround) || 
        (!m_isOnGround && m_wasOnGroundLastF)) OnStoppedRunning?.Invoke();
        #endregion start_stop_delegates

        m_holdingRunLastF = m_holdingRun;
        m_wasOnGroundLastF = m_isOnGround;
        m_wasInLadderLastF = m_isInLadder;
        m_hadControlLastF = m_hasControl;
        m_movingInLadderLastF = m_movingInLadderThisF;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tr = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        m_col = GetComponent<Collider2D>();
        
        m_originalGravitySc = rb.gravityScale;
    }

    void Update()
    {
        //update input info
        m_xAxis = Input.GetAxis("Horizontal"); //fetched in update, utilized also in fixed update
        m_yAxis = Input.GetAxis("Vertical");

        if (Mathf.Abs(m_xAxis) > Mathf.Epsilon) { //---running
            m_holdingRun = true;
        }
        else {
            m_holdingRun = false;
        }

        m_holdingVertical = false;
        if (m_yAxis > Mathf.Epsilon) { //--------------jumping
            m_holdingJump = true;
            m_holdingVertical = true;
        }
        else {
            m_holdingJump = false;
            m_holdingVertical = true;
        }

        if (m_isInLadder) { //-------------------------climbing
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
        if (!m_hasControl) {return;} 
        
        Vector3 scaleVect = tr.localScale; 
        float dirSign = Mathf.Sign(m_xAxis);
        tr.localScale = new Vector3(dirSign * Mathf.Abs(scaleVect.x), 
                                    scaleVect.y, 
                                    scaleVect.z);
    }

    private void uRunning() {
        if (m_holdingRun) {
            anim.SetBool("isRunning", true);
            TryFlipSprite();
        }
        else {
            anim.SetBool("isRunning", false);
        }

        anim.SetBool("isOnGround", m_isOnGround);
    }

    private void fuRunning() {

        if (m_hasControl) {
            float xVel = m_xAxis * speed; //no need to get xAxis in FixedUpdate

            //if running this frame
            if (m_holdingRun) {
                rb.velocity = new Vector2(xVel, rb.velocity.y);  
            }
            //if stopped running this frame
            else if (!m_holdingRun && m_holdingRunLastF) { 
                rb.velocity = new Vector2(0f, rb.velocity.y);
            }
        }
    }

    private void uJumping() {
        //nothing to do here 
    }

    private void fuJumping(float jumpSpeed, bool doForceJump=false) {
        if (m_hasControl) {
            //still not entirely sure, why this gets called twice per jump 
            //pressed - has to do with how its calculated
            if (m_holdingJump && (m_isOnGround || doForceJump)) { 
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
        //m_isInLadder = ladderCol.OverlapPoint(m_col.bounds.center);
        
        //setting m_isInLadder in OnTriggerEnter/Exit2D would also not
        //work. Solution? Change the ladders' physics shape so that the
        //player's collider is always touching some ladder's surface,
        //meaning it can never be 'fully' in it. 
        
        if (m_isInLadder & m_hasControl) {
            rb.gravityScale = 0f;

            //if the player entered the ladder this frame or regained control this frame
            if (!m_wasInLadderLastF || !m_hadControlLastF) {
                rb.velocity = Vector2.zero;
            }
            //otherwise, handle the input
            else { 
                //if holding up or down
                if (Mathf.Abs(m_yAxis) > Mathf.Epsilon) {
                    rb.velocity = new Vector2(rb.velocity.x, m_yAxis*climbSpeed);
                }
                else { //if not holding up or down
                    rb.velocity = new Vector2(rb.velocity.x, 0f);
                }
            }

        }
        
        //if exit ladder this frame
        else if (m_wasInLadderLastF && !m_isInLadder) {
            fuJumping(m_jumpSpeed*0.75f, true); //make the player able to do a off-the-ladder jump.
        }
        else {
            rb.gravityScale = m_originalGravitySc;
        }

        m_movingInLadderThisF = true;
        //if not in the ladder OR in the ladder but not moving
        if ((!m_isInLadder) || (m_isInLadder && rb.velocity == Vector2.zero)) {
            m_movingInLadderThisF = false;
        }
    }

    public void StopControl() {
        m_hasControl = false;
    }

    public void RegainControl() {
        m_hasControl = true;
        anim.SetBool("isHurt", false);
    }

}
