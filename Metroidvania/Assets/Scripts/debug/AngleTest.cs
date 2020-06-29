using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleTest : MonoBehaviour
{
    [SerializeField] private Transform enemyTransform;

    Transform myTransform;

    private void Awake() {
        myTransform = transform;
    }

    private void Update() {
        /*Debug.Log(OwnUtility.Angle(enemyTransform.position,
            myTransform.position)* Mathf.Rad2Deg);*/
    }
}
