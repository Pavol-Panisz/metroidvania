using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEE : EditorEntity
{
    [SerializeField] private Behaviour[] enabledOnlyInPlayModeAlive;

    [SerializeField] private Behaviour[] enabledOnlyInEditMode;

    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private PlayerManager playerManager;

    [SerializeField] private HealthSystem healthSystem;

    private Vector3 ogPosition;
    private Quaternion ogRotation;

    private void Awake()
    {
        saveSystemId = "player"; // unlike shooting enemy and co., I haven't implemented setting the saveSysId from the editor here
    }

    private void Start()
    {
        UpdateTransform();
    }

    private void OnEnable()
    {
        entityPlacement.OnDropped += UpdateTransform;
    }
    private void OnDisable()
    {
        entityPlacement.OnDropped -= UpdateTransform;
    }

    public override void OnEnterEditMode()
    {
        foreach (var c in enabledOnlyInPlayModeAlive) c.enabled = false;
        foreach (var c in enabledOnlyInEditMode) c.enabled = true;
        rb.simulated = false;

        transform.position = ogPosition;
        transform.rotation = ogRotation;
    }

    public override void OnEnterPlayMode()
    {
        foreach (var c in enabledOnlyInPlayModeAlive) c.enabled = true;
        foreach (var c in enabledOnlyInEditMode) c.enabled = false;
        rb.simulated = true;

        playerManager.SetRespawnPoint(new Vector2(transform.position.x, transform.position.y));

        healthSystem.FullHealth();
    }

    public override void UpdateTransform()
    {
        ogPosition = new Vector3(transform.position.x, transform.position.y, 0f);
        ogRotation = transform.rotation;
        playerManager.SetRespawnPoint(new Vector2(transform.position.x, transform.position.y));
    }

}
