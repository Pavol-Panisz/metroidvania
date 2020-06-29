using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageDealer, ILaunchesAway
{
    [SerializeField] float m_damage = 2f;
    [SerializeField] float dyingDuration=1f;
    [SerializeField] SFXPlayer sfx;

    private HealthSystem hs;
    private Animator anim;
    private PingPongWalk walkSys;

    private void Awake() {

        hs = GetComponent<HealthSystem>();
        anim = GetComponent<Animator>();
        walkSys = GetComponent<PingPongWalk>();
        hs.OnDeath += Death;
    }

    private void Start() {
        sfx.TryStartPlayLooped("Walk");
    }

    public float GetDamage() {
        return m_damage;
    }

    private void Death() {
        sfx.TryStopPlayLooped("Walk");
        sfx.TryPlayOnce("Death");
        m_damage = 0f;
        Debug.Log("enemy dying");
        anim.SetBool("isDead", true);
        walkSys.enabled = false;
        Destroy(gameObject, dyingDuration);
    }
}
