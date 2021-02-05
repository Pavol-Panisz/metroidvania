using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int targetFps = 60;

    private void Update() {
        if (Application.targetFrameRate != targetFps) {
             Application.targetFrameRate = targetFps;
        }
    }

}
