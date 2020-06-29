using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class Interactor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D coll) {
        IInteractable ie = coll.GetComponent<IInteractable>();

        if (ie != null) {
            ie.interact();
        }
    }
}
