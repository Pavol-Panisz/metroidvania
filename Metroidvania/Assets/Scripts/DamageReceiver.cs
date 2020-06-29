using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    [SerializeField] private HealthSystem healthSystem;
    [Tooltip("The collider used for detecting damage. NOTE: The layer on which "+
    "this collider's gameobject resides is the one used for collision detection.")]
    [SerializeField] private Collider2D dmgDetectionCollider;
    private bool isInvincible=false;
    [SerializeField] private float invincibleDuration=0f;
    private Coroutines coroutines;
    
    //the layers with which the dmgDetectionCollider's gameObject can collide with.
    private LayerMask collideableLayersMask;

    private Rigidbody2D rb;

    public delegate void goArgDlg(GameObject go);
    public event goArgDlg OnDamagedBy;
    public delegate void emptyDlg();
    public event emptyDlg OnTakenDamage;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        coroutines = Coroutines.GetInstance();

        collideableLayersMask = Physics2D.GetLayerCollisionMask(dmgDetectionCollider.gameObject.layer);
        //Debug.Log(LayerMask.LayerToName(dmgDetectionCollider.gameObject.layer));
    }


    private void CheckDamage(GameObject damagerGo) {

        if (!rb.IsTouchingLayers(collideableLayersMask) || isInvincible) {return;}

        var dd = damagerGo.GetComponent<IDamageDealer>();
        if (dd != null && dd.GetDamage() > 0f) {
            if (healthSystem.GetHealth() > 0) { OnTakenDamage?.Invoke();}
            healthSystem.Damage(dd.GetDamage());
            OnDamagedBy?.Invoke(damagerGo);
            isInvincible = true;
            coroutines.WaitThenExecute(invincibleDuration, () => StopInvincibility());

        }
    }

    public void SetInvincible(bool b) {
        isInvincible = b;
    }
    public bool IsInvincible() {
        return isInvincible;
    }

    private void OnCollisionStay2D(Collision2D coll) => CheckDamage(coll.gameObject);
    private void OnTriggerStay2D(Collider2D coll) => CheckDamage(coll.gameObject);

    public void StopInvincibility() { isInvincible = false;}
}
