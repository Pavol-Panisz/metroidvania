using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audiosourc : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip;

    private void Start() {

        audioSource.Stop();
        audioSource.clip = null;


       audioSource.clip = clip; 
       audioSource.Play();
    }
}
