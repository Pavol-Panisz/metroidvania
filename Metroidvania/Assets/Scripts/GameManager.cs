using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private void Update() {
        if (Application.targetFrameRate != 60) {
             Application.targetFrameRate = 60;
        }
    }

}
