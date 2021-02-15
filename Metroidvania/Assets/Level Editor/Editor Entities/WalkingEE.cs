using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingEE : EditorEntity
{

    [SerializeField] private Behaviour[] enabledOnlyInPlayModeAlive;
    [SerializeField] private Behaviour[] enabledOnlyInEditMode;
    [Space]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private Enemy walkingEnemy;

    Vector3 ogPosition;
    Quaternion ogRotation;

    Coroutine destroyingCorot;

    private void OnEnable()
    {
        walkingEnemy.OnFinishedNonDestructiveDeath += OnFinishedNotDestructiveDeath;
        entityPlacement.OnDropped += UpdateTransform;
    }
    private void OnDisable()
    {
        walkingEnemy.OnFinishedNonDestructiveDeath -= OnFinishedNotDestructiveDeath;
        entityPlacement.OnDropped -= UpdateTransform;
    }

    public override void OnEnterEditMode()
    {
        if (destroyingCorot != null) StopCoroutine(destroyingCorot);

        foreach (var c in enabledOnlyInPlayModeAlive) c.enabled = false;
        foreach (var c in enabledOnlyInEditMode) c.enabled = true;
        rb.simulated = false;

        walkingEnemy.Resurrect();
    }
    
    public override void OnEnterPlayMode()
    {
        if (destroyingCorot != null) StopCoroutine(destroyingCorot);

        foreach (var c in enabledOnlyInPlayModeAlive) c.enabled = true;
        foreach (var c in enabledOnlyInEditMode) c.enabled = false;
        rb.simulated = true;

        walkingEnemy.Resurrect();
        bodyRenderer.color = new Color(1f, 1f, 1f, 1f);
    }

    public override void UpdateTransform()
    {
        ogPosition = transform.position;
        ogRotation = transform.rotation;
    }

    private void OnFinishedNotDestructiveDeath()
    {
        if (destroyingCorot != null) StopCoroutine(destroyingCorot);

        destroyingCorot =  StartCoroutine(DestroyingBodyCorot());
    }

    private IEnumerator DestroyingBodyCorot()
    {
        yield return new WaitForSeconds(walkingEnemy.dyingDuration);

        foreach (var c in enabledOnlyInPlayModeAlive) c.enabled = false;
        rb.simulated = false;
        bodyRenderer.color = new Color(1f, 1f, 1f, 0f);
    }

}
