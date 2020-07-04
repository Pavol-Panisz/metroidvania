using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(HealthSystem))]

public class PlayerManager : MonoBehaviour
{
    [Tooltip("the direction in which the player will be launched upon taking damage from a gameobject inheriting from ILaunchesAway")]
    [SerializeField] private Vector2 launchVect;
       
    [Tooltip("The SFXPlayer associated with the player")]
    [SerializeField] private SFXPlayer sfx;

    [Tooltip("The particle system played upon entering lava")]
    [SerializeField] ParticleSystem smokeParticleSys;

    [Tooltip("The clip played when being hurt")]
    [SerializeField] private AnimationClip hurtClip;

    private Vector2 respawnPoint;

    PlayerController pc;
    ScreenFader sf;
    Transform tr;
    SpriteRenderer sr;
    Coroutines coroutines;
    Animator anim;
    HealthSystem hs;
    DamageReceiver dr;
    Lava lava;
    Rigidbody2D rb;

    public delegate void emptyDlg();
    public event emptyDlg OnDamagedByLava;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        hs = GetComponent<HealthSystem>();
        dr = GetComponent<DamageReceiver>();
        lava = FindObjectOfType<Lava>();
        coroutines = Coroutines.GetInstance();
        sf = FindObjectOfType<ScreenFader>();
        anim = GetComponent<Animator>();
        pc = GetComponent<PlayerController>();
        sr = GetComponent<SpriteRenderer>();
        tr = GetComponent<Transform>();
        
        SetRespawnPoint(tr.position);

        #region delegate_subscriptions
        //I am fully aware of the fact that had I used unityEvents, I could do this in the inspector:
        pc.OnStartedRunning += (() => {sfx.TryStartPlayLooped("Run");});
        pc.OnStoppedRunning += (() => {sfx.TryStopPlayLooped("Run");});
        pc.OnStartedClimbing += (() => {sfx.TryStartPlayLooped("ClimbLadder");});
        pc.OnStoppedClimbing += (() => {sfx.TryStopPlayLooped("ClimbLadder");});
        pc.OnJumped += (() => {sfx.TryPlayOnce("Jump");});
        pc.OnHitGround += (() => {sfx.TryPlayOnce("GroundImpact");});
        pc.OnClimbedOnce += (() => {sfx.TryPlayOnce("GrabbedLadder");});
        dr.OnTakenDamage += (() => { sfx.TryPlayOnce("Damaged");});
        OnDamagedByLava += (() => {sfx.TryPlayOnce("TouchLava");});

        hs.OnDeath += Death;
        dr.OnDamagedBy += ProcessDamager;
        #endregion delegate_subscriptions
    }




    public void ProcessDamager(GameObject go) {

        //if we were damaged by lava
        if (go.GetComponent<Lava>() != null) {

            //if we still are alive
            if (hs.GetHealth() > 0f ) {
            TransformUtility.ThrowVertically(ref rb, 30f);
            OnDamagedByLava?.Invoke();
            smokeParticleSys.Play();
            Debug.Log("damaged by lava. health is "+hs.GetHealth().ToString());
            }
            //if not alive anymore
            else {
                rb.velocity = new Vector2(0f, rb.velocity.y); //stops from sliding on lava
                Debug.Log("in lava, health is "+hs.GetHealth().ToString());
            }
        }

        //otherwise, we've been damaged by something else
        else if ((go.GetComponent<ILaunchesAway>() != null) && (hs.GetHealth() > 0f)) {
            TransformUtility.LaunchFromEnemy(rb, go.GetComponent<Transform>().position, launchVect);
            pc.StopControl();
            anim.SetTrigger("isHurt");
            pc.StopControl();
            coroutines.WaitThenExecute(hurtClip.length, () => {

                //regain control only if during the hurt animation we didn't die.
                //otherwise, control will be regained in the Death() method
                if (hs.GetHealth() > 0) { 
                    pc.RegainControl();
                }
            });
        }
    }

    private void Death() {
        
        anim.SetBool("isDead", true);
        pc.StopControl(); 
        Debug.Log("stopped control");
        rb.velocity = Vector2.zero; //prevent sliding
        coroutines.WaitThenExecute(1f, () => {
            sf.FadeIn();
        });
        coroutines.WaitThenExecute(2f, () => {
            hs.FullHealth();
            tr.position = respawnPoint; 
        });
        coroutines.WaitThenExecute(3f, () => {
            sf.FadeOut();
        });
        coroutines.WaitThenExecute(3.5f, () => {
            anim.SetBool("isDead", false);
        });
        coroutines.WaitThenExecute(3.8f, () => {
            pc.RegainControl();
            Debug.Log("regained control");
        });
        //TODO finesse this

    }

    public void SetRespawnPoint(Vector2 v) {
        respawnPoint = v;
    }

    ~PlayerManager() {
        #region delegate_unsubscriptions
        
        //TODO: Check if unsubscribing lambda functions from delegates actually works

        pc.OnStartedRunning -= (() => {sfx.TryStartPlayLooped("Run");});
        pc.OnStoppedRunning -= (() => {sfx.TryStopPlayLooped("Run");});
        pc.OnStartedClimbing -= (() => {sfx.TryStartPlayLooped("ClimbLadder");});
        pc.OnStoppedClimbing -= (() => {sfx.TryStopPlayLooped("ClimbLadder");});
        pc.OnJumped -= (() => {sfx.TryPlayOnce("Jump");});
        pc.OnHitGround -= (() => {sfx.TryPlayOnce("GroundImpact");});
        pc.OnClimbedOnce -= (() => {sfx.TryPlayOnce("GrabbedLadder");});
        dr.OnTakenDamage -= (() => {sfx.TryPlayOnce("Damaged");});
        OnDamagedByLava -= (() => {sfx.TryPlayOnce("TouchLava");});

        hs.OnDeath -= Death;
        dr.OnDamagedBy -= ProcessDamager;
        #endregion delegate_unsubscriptions
    }


}