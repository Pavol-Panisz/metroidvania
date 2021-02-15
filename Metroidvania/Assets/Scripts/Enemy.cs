using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour, IDamageDealer, ILaunchesAway
{
    [SerializeField] float m_damage = 2f;
    public float dyingDuration=1f;
    [SerializeField] SFXPlayer sfx;

    private HealthSystem hs;
    private Animator anim;
    private PingPongWalk walkSys;

    private float ogDamage = 0f;

    public Action OnFinishedNonDestructiveDeath;

    private void Awake() {

        hs = GetComponent<HealthSystem>();
        anim = GetComponent<Animator>();
        walkSys = GetComponent<PingPongWalk>();
        hs.OnDeath += Death;

        ogDamage = m_damage;
    }

    private void Start() {
        sfx.TryStartPlayLooped("Walk");
        anim.SetBool("isDead", false);
    }

    public float GetDamage() {
        return m_damage;
    }

    private void Death(bool doDestroy) {
        sfx.TryStopPlayLooped("Walk");
        sfx.TryPlayOnce("Death");
        m_damage = 0f;
        //Debug.Log("enemy dying");
        anim.SetBool("isDead", true);
        walkSys.enabled = false;

        if (doDestroy)
        {
            Destroy(gameObject, dyingDuration);
        } else
        {
            
            OnFinishedNonDestructiveDeath?.Invoke();
        }
    }

    public void Resurrect() // called when play mode starts
    {
        hs.FullHealth();
        m_damage = ogDamage;
        anim.SetBool("isDead", false);
    }
}
