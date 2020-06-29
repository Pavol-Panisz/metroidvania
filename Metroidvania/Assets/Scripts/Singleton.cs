using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{
    static GameObject instance; //used in singleton pattern

    public void Awake() {

        //singleton pattern
        if (instance == null) {
            instance = gameObject;
            DontDestroyOnLoad(gameObject);


            Application.targetFrameRate = 60;
        }
        else {
            Destroy(gameObject);
            //Debug.Log("destoryed the game manager gameobject");
        }
    }
}
