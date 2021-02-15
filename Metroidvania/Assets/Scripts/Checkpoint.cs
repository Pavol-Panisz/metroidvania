using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Checkpoint : MonoBehaviour, IInteractable
{
    [SerializeField] private int priority; 
    [SerializeField] private Sprite capturedSprite;
    [SerializeField] private SFXPlayer sfx;
    public static int stCurrentPriority;
    private Vector2 respawnPoint;
    private PlayerManager plM;
    private SpriteRenderer sr;
    private bool hasBeenTouched=false;
    

    private delegate void eventDlg();
    private static event eventDlg OnReachedCheckpoint;

    void Start() {
        try {
            plM = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        }
        catch (NullReferenceException) {
            plM = null;
        }
        
        sr = GetComponent<SpriteRenderer>();

        respawnPoint = transform.position + Vector3.up;
        stCurrentPriority = -1; // Resets the current largest priority
    }

    public void interact() {

        sr.sprite = capturedSprite;
        if (!hasBeenTouched) {
            sfx.TryPlayOnce("Ding");
            hasBeenTouched = true;
        }

        if (priority > stCurrentPriority) {
            plM?.SetRespawnPoint(respawnPoint);
            stCurrentPriority = priority;
        }
    }

}
