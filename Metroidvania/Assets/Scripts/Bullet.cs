using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IDamageDealer, ILaunchesAway
{
    [SerializeField] private float damage=1f;
    [SerializeField] private float lifeTime=10f;
    [SerializeField] private Sprite explosionSprite;
    [SerializeField] private SFXPlayer sfx;

    SpriteRenderer sr;
    Rigidbody2D rb;
    ParticleSystem ps;
    Coroutines coroutines;

    public float GetDamage() {return damage;}

    private void Awake() {
        Destroy(gameObject, lifeTime);
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        ps = GetComponentInChildren<ParticleSystem>();
    }

    private void Start() {
        coroutines = Coroutines.GetInstance();
        sfx.TryStartPlayLooped("Flying");
    }
    private void OnTriggerEnter2D(Collider2D coll) {

        //let the particle system live for a while, so that its particles
        //have time to finish their lifecycle
        ps.Stop();
        ps.transform.SetParent(null);
        coroutines.WaitThenExecute(ps.main.startLifetime.constant, () => {
            if (ps) {
                Destroy(ps.gameObject);
            }
        });
        
        rb.velocity = Vector2.zero;
        sr.sprite = explosionSprite;
        sfx.TryPlayOnce("Impact");
        sfx.TryStopPlayLooped("Flying");

        Destroy(gameObject, 0.2f);
    }


}
