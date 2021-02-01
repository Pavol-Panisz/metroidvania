using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class New2DmgRecv : MonoBehaviour
{
    [SerializeField] private Collider2D dmgRecvColl;

    private void OnTriggerEnter2D() {
        Debug.Log("trigger");
    }
}
