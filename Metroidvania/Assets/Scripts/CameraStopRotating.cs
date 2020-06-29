using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStopRotating : MonoBehaviour
{
    private void Start() {
        this.transform.rotation.eulerAngles.Set(0f, 0f, 0f);
    }

    void Update()
    {
        
    }
}
