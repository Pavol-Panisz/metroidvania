using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointLE : MonoBehaviour, IInteractable
{
    public int priority; // set in CheckpointEE
    [SerializeField] private Sprite notCapturedSprite;
    [SerializeField] private Sprite capturedSprite;
    [SerializeField] private SFXPlayer sfx;
    public static int stCurrentPriority;
    public Vector2 respawnPoint;

    private PlayerManager plM;
    private HealthSystem playerHealth;

    [SerializeField] private SpriteRenderer sr;
    private bool hasBeenTouched = false;

    private delegate void eventDlg();
    private static event eventDlg OnReachedCheckpoint;

    private void Awake()
    {
        plM = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        playerHealth = plM.gameObject.GetComponent<HealthSystem>();
    }

    void Start()
    {

        stCurrentPriority = -1; // Resets the current largest priority
    }

    public void interact()
    {

        if (!hasBeenTouched)
        {
            sfx.TryPlayOnce("Ding");
            SetHasBeenTouched(true);
            playerHealth.FullHealth();
        }

        if (priority > stCurrentPriority)
        {
            plM.SetRespawnPoint(respawnPoint);
            stCurrentPriority = priority;
        }
    }

    public void SetHasBeenTouched(bool b)
    {
        hasBeenTouched = b;
        if (hasBeenTouched)
        {
            sr.sprite = capturedSprite;
        } else
        {
            sr.sprite = notCapturedSprite;
        }
    }
}
