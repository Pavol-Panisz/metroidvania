using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewDmgRecv : MonoBehaviour
{
    public ContactFilter2D contactFilter;
    public Collider2D damageDetectionColl;

    private Collider2D[] colliders = new Collider2D[10];

    private void Awake() {
        int layer = gameObject.layer; //the physics layer this gameobject is on
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(layer));
        contactFilter.useTriggers = true;
    }

    private void FixedUpdate() {
        int n = damageDetectionColl.OverlapCollider(contactFilter, colliders);

        if (n == 0) {
            Debug.Log("not colliding with anything");
        }
        else {
            foreach (Collider2D col in colliders) {
                if (col != null) {
                    Debug.Log(col.gameObject.name);
                }
            }
        }

        for (int iii=0; iii < colliders.Length; iii++) {
            colliders[iii] = null;
        }
    }

}