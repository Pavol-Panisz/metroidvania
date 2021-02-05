using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShootingEnemy : MonoBehaviour
{
    #region variables

    [Header("Raycast")]

    [Tooltip("The transform from which we raycast towards the target")]
    [SerializeField] private Transform raycastBeginTr;

    [Tooltip("The gameObject we're trying to spot and shoot")]
    [SerializeField] private GameObject target;

    [Tooltip("The max distance over which the target can be spotted")]
    [SerializeField] private float sightDist;

    [Tooltip("The layers the search for player raycast collides with")]
    [SerializeField] private LayerMask sightLayerMask;

    [Header("Shooting")]

    [Tooltip("The transform which will be rotated so that it faces the target")]
    [SerializeField] private Transform armRotationPivot;

    [Tooltip("The transform where the bullet gets instantiated")]
    [SerializeField] private Transform bulletSpawnTr;

    [Tooltip("The projectile to shoot at the target")]
    [SerializeField] private GameObject bullet;

    [Tooltip("Time between consecutive shots")]
    [SerializeField] private float shootInterval=2f;

    [SerializeField] private float bulletSpeed;

    [Header("Visuals")]

    [Tooltip("The hand sprite renderer")]
    [SerializeField] private SpriteRenderer handSpriteRenderer;

    [Tooltip("The script which makes the legs move like crazy")]
    [SerializeField] private WavyTransforms feetAnimator;

    [Tooltip("When we shoot, the hand switches to this sprite for a split second")]
    [SerializeField] private Sprite fireHandSprite;

    [Tooltip("Upon taking damage, switch to the next sprite from this array")]
    [SerializeField] private Sprite[] damageSprites;

    [Header("SFX")]
    [SerializeField] private SFXPlayer sfx;

    [Tooltip("The minimal time between consecutive idle sounds")]
    [SerializeField] [Range(0f, 10f)] private float idleSoundMin;

    [Tooltip("The maximum time between consecutive idle sounds")]
    [SerializeField] [Range(0f, 10f)] private float idleSoundMax;

    [Header("Misc")]
    [SerializeField] private float dyingDuration=2f;

    public int currentDamageSpIndex=-1;
    private bool canSeeTarget=false;
    private float shootTimer = 0f;
    private bool justSpottedTarget=true;
    private Coroutine runningCorot;
    private Vector3 armHomeRotation;
    private float patrollingLookDir;
    private enum States {Patrolling, Shooting}
    private States state = States.Patrolling;
    private States stateLastF = States.Patrolling;
    private float nextIdleSoundT=0f;

    private Transform tr;
    private Transform targetTr;
    private RaycastHit2D rh;
    private Rigidbody2D targetRb;
    private PingPongWalk pingPongWalk;
    private Coroutines coroutines;
    private HealthSystem hs;
    private SpriteRenderer sr;

    public Action OnFinishedNonDestructiveDeath;

    #endregion variables

    private void Awake()
    {
        coroutines = Coroutines.GetInstance();
        hs = GetComponent<HealthSystem>();
        sr = GetComponent<SpriteRenderer>();
        tr = GetComponent<Transform>();
        pingPongWalk = GetComponent<PingPongWalk>();
    }

    private void Start() {

        if (!target) {
            target = GameObject.FindGameObjectWithTag("Player");
        }
        targetTr = target.GetComponent<Transform>();
        targetRb = target.GetComponent<Rigidbody2D>();

        hs.OnDeath += Death;
        hs.OnTakenDamage += ChangeSprite;
        hs.OnTakenDamage += TryPlayDmgSfx;
    
        armHomeRotation = armRotationPivot.eulerAngles;

        nextIdleSoundT = Time.time + UnityEngine.Random.Range(idleSoundMin, idleSoundMax);
        sfx.TryStartPlayLooped("Walk");
    }

    public void OnDrawGizmosSelected() {
        if (targetTr == null) {return;}
        
        if (canSeeTarget) 
        {
            Gizmos.color = Color.green;
        }
        else {
            Gizmos.color = Color.red;
        }
        
        Gizmos.DrawLine(raycastBeginTr.position, targetTr.position);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(raycastBeginTr.position, sightDist);
    }

    private void FixedUpdate() {

        if (hs.GetHealth() <= 0) { return; } //if dead, do nothing

        Vector2 dir = targetTr.position - raycastBeginTr.position;
        rh = Physics2D.Raycast(raycastBeginTr.position, dir, sightDist, sightLayerMask);
        
        if (ReferenceEquals(rh.transform, targetTr)) {
            canSeeTarget = true;
        } 
        else {
            canSeeTarget = false;
        }
    }

    private void Update() {

        if (hs.GetHealth() <= 0) {return;} //if dead, do nothing

        //idle sound
        if (Time.time >= nextIdleSoundT) {
            sfx.TryPlayOnce("Idle");
            
            float waitPeriodT = UnityEngine.Random.Range(idleSoundMin, idleSoundMax);
            nextIdleSoundT = Time.time + waitPeriodT;
        }
        

        //patrolling state
        if (!canSeeTarget) {
            state = States.Patrolling;

            armRotationPivot.eulerAngles = armHomeRotation;
            
        }
        
        //shooting state
        else { 
            state = States.Shooting;

            pingPongWalk.enabled = false;
            GunFaceTarget();
            FaceTarget();

            if (shootTimer >= shootInterval) {
                Shoot();
                shootTimer = 0f;
            }

            shootTimer += Time.deltaTime;
        }

        if (stateLastF != state) 
        {
            OnStateChangedTo(state);
        }
        stateLastF = state;
    }

    private void GunFaceTarget() {
        if (tr.localScale.x > 0) { //if looking right
                TransformUtility.RotateTransformTowards(armRotationPivot, targetTr.position);
            }

            //if looking left, create a new position vector, which is a projection of 
            //targetTr.position through armRotationPivot's position. 
            #region further_explanation
            //imagine a line from targetTr.position to armRotPiv.position. Now
            //imagine a line that is perpendicular to this line. This new line
            //acts as a mirror and the projected point is the mirror image of
            //targetTr.position 
            #endregion further_explanation
            else { 
                Vector3 proj = 2*armRotationPivot.position - targetTr.position; 
                TransformUtility.RotateTransformTowards(armRotationPivot, proj);
            }
    }

    private void FaceTarget() {
        if (targetTr.position.x > tr.position.x) { //if target on our right
            tr.localScale = new Vector2(Mathf.Abs(tr.localScale.x),
                                                tr.localScale.y);
        }
        else { //if target on our left
            tr.localScale = new Vector2(-Mathf.Abs(tr.localScale.x),
                                                tr.localScale.y);
        }
    }

    private void Shoot() {

        GameObject bGo = Instantiate(bullet, bulletSpawnTr.position, Quaternion.identity) as GameObject;
        TransformUtility.RotateTransformTowards(bGo.transform, targetTr.position);
        Rigidbody2D bRb = bGo.GetComponent<Rigidbody2D>();
        bRb.velocity = (targetTr.position - bulletSpawnTr.position).normalized * bulletSpeed;

        Sprite original = handSpriteRenderer.sprite;
        handSpriteRenderer.sprite = fireHandSprite;
        coroutines.WaitThenExecute(0.1f, () => {
            if (handSpriteRenderer) //if we didn't die in the meantime, so the rendered still exists
            handSpriteRenderer.sprite = original;
        });

        sfx.TryPlayOnce("Shoot");
    }

    private void Death(bool doDestroy) {
        sfx.TryPlayOnce("Dead");
        sfx.TryStopPlayLooped("Walk");
        pingPongWalk.enabled = false;

        if (doDestroy)
        {
            Destroy(feetAnimator?.gameObject);
            Destroy(armRotationPivot.gameObject);
            Destroy(gameObject, dyingDuration);
        } else
        {
            OnFinishedNonDestructiveDeath?.Invoke(); // ShootingEnemyEE leverages this
        }
    }

    private void OnStateChangedTo(States st) {

        switch (state)
        {
            case States.Patrolling:
                //Debug.Log("state changed to patrolling");
                shootTimer = 0f;
                pingPongWalk.enabled = true;
                feetAnimator?.StartWaving();
                sfx.TryStopPlayLooped("Walk"); // hack bugfix
                sfx.TryStartPlayLooped("Walk");
                break;

            case States.Shooting:
                feetAnimator?.StopWaving(); 
                justSpottedTarget = true;
                sfx.TryStopPlayLooped("Walk");
                break;

            default:
                break;
        } 
    }

    private void TryPlayDmgSfx(float f) {
        if ((hs.GetHealth() > 1) && (f > 0)) {
            sfx.TryPlayOnce("Damaged");
        } 
    }

    private void ChangeSprite(float f) {

        if (hs.GetHealth() <= 0) { return; } //if dead, do nothing

        currentDamageSpIndex += 1;
        if (currentDamageSpIndex < damageSprites.Length) {
            sr.sprite = damageSprites[currentDamageSpIndex];
        }
        else
        {
            Debug.LogError($"the damage sprite index was {currentDamageSpIndex}");
        }
    }

    ~ShootingEnemy() {
        hs.OnTakenDamage -= ChangeSprite;
        hs.OnTakenDamage -= TryPlayDmgSfx;
        hs.OnDeath -= Death;
    }

}
