using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollDetectTest : MonoBehaviour
{
    public Collider2D dmgRecvColl;

    private void OnTriggerEnter2D(Collider2D coll) {
        Debug.Log(coll.gameObject.name);
    }
}
