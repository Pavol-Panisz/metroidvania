using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Level editor specific stuff logic for the enemy, such as "having no brain" 
 * (disabling several components) while in edit mode, etc.
 */

#region what is LIMBO? 
/*
*NOTE: "Limbo" means when the enemy dies. Since we're in an "editor",
 * we can't afford to let the enemy destroy its gameobjects in the Death method.
 * Therefore, the Death method has a non-destructive branch inside of it.
 * In the method OnFinishedNotDestructiveDeath, we simulate, what we're trying
 * to achieve in the destructive branch. So, for example, if in the destructive
 * branch we're trying to destroy the whole gameobject, then here, we'll try
 * to instead disable all components which give the enemy its logic, as well
 * as visual stuff (which is visible in edit mode though), such as sprite
 * renderers.
 */
#endregion what is LIMBO?

public class ShootingEnemyEE : EditorEntity
{
    [Header("Get disabled in different stages")]
    [Tooltip("Components enabled only when in play mode and also not in limbo")]
    [SerializeField] private Behaviour[] enabledOnlyInPlayModeAlive;

    [SerializeField] private Behaviour[] enabledOnlyInEditMode;

    [Header("Get disabled when in limbo")]
    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private SpriteRenderer shootingArmRenderer;
    [SerializeField] private SpriteRenderer leftFootRenderer;
    [SerializeField] private SpriteRenderer rightFootRenderer;

    [Space]
    [Tooltip("If this isn't null, its simulation gets turned off in edit mode")]
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private SFXPlayer sfx;

    [SerializeField] private HealthSystem healthSystem; // so that when we die, we can respawn in edit 

    [SerializeField] private ShootingEnemy shootingEnemy;

    [Tooltip("Should be equal to ShootingEnemy.dyingInterval")]
    [SerializeField] private float dyingDuration = 2f;

    Coroutine destroyingBodyCorot;

    private Sprite ogBodySprite;

    Vector3 ogPosition;
    Quaternion ogRotation;

    private void OnEnable()
    {
        shootingEnemy.OnFinishedNonDestructiveDeath += OnFinishedNotDestructiveDeath;
        entityPlacement.OnDropped += UpdateTransform;
    }
    private void OnDisable()
    {
        shootingEnemy.OnFinishedNonDestructiveDeath -= OnFinishedNotDestructiveDeath;
        entityPlacement.OnDropped -= UpdateTransform;
    }


    private void Awake()
    {
        ogBodySprite = bodyRenderer.sprite;
        //saveSystemId = "shooting_enemy";
    }

    public override void OnEnterEditMode()
    {
        ResetTransformAndHeal();
        EnableVisuals();
        sfx.TryStopPlayLooped("Walk");
        foreach (var c in enabledOnlyInPlayModeAlive) c.enabled = false;
        foreach (var c in enabledOnlyInEditMode) c.enabled = true;
        rb.simulated = false;
    }
    public override void OnEnterPlayMode()
    {
        ResetTransformAndHeal();
        EnableVisuals();
        sfx.TryStopPlayLooped("Walk");
        sfx.TryStartPlayLooped("Walk");
        foreach (var c in enabledOnlyInPlayModeAlive) c.enabled = true;
        foreach (var c in enabledOnlyInEditMode) c.enabled = false;
        rb.simulated = true;
    }
    public void Respawn() // when you restart the level in play mode
    {
        OnEnterPlayMode();
    }

    public override void UpdateTransform() // called in edit mode when placing the player somewhere else
    {
        // update edit mode position
        ogPosition = transform.position;
        ogRotation = transform.rotation;
    }

    // Called from the Death method of the Shooting Enemy
    public void OnFinishedNotDestructiveDeath()
    {
        // do something visually similar to what the Death method's
        // destructive branch is supposed to do
        shootingArmRenderer.enabled = false;
        leftFootRenderer.enabled = false;
        rightFootRenderer.enabled = false;
        if (destroyingBodyCorot != null) StopCoroutine(destroyingBodyCorot); // stop the corot just in case
        destroyingBodyCorot = StartCoroutine(DestroyBodyCorot());

        foreach (var c in enabledOnlyInPlayModeAlive) c.enabled = false;
    }

    private IEnumerator DestroyBodyCorot() // cause the carcass stays visible for a while after death
    {
        yield return new WaitForSeconds(dyingDuration);
        bodyRenderer.enabled = false;
    }

    private void EnableVisuals()
    {
        // re-enable the visual stuff, such as the renderers
        if (destroyingBodyCorot != null) StopCoroutine(destroyingBodyCorot);
        bodyRenderer.enabled = true;
        
        shootingArmRenderer.enabled = true;
        leftFootRenderer.enabled = true;
        rightFootRenderer.enabled = true;
    }

    // Puts the enemy at its original (edit mode) position and sets full health
    public void ResetTransformAndHeal()
    {
        transform.position = ogPosition;
        transform.rotation = ogRotation;

        healthSystem.Heal(healthSystem.GetMaxHealth() - healthSystem.GetHealth());
        bodyRenderer.sprite = ogBodySprite;
        shootingEnemy.currentDamageSpIndex = -1;
    }

}
