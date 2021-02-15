using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EntityPlacement : MonoBehaviour
{
    private Vector3 mousePosLastF;

    public static EntityPlacement beingHeld = null;
    private static EntityPlacement beingMousedOver = null;
    private bool wasMousedOverLastF = false;

    public Action OnDropped;

    // the x and y size of the sprite in world space, not accounting for 
    // scaling of the transform, which holds the sprite renderer
    Vector2 unscaledSpriteWorldSize;

    public EditorEntity entity;

    [Tooltip("The spriteRenderer, over whose sprite a mouse-over will be registered. It should be " 
        + " on the same gameobject as this script.")]
    [SerializeField] private SpriteRenderer mouseOverSpriteRenderer;

    [Space]
    [Header("Mouse-over tint related")]
    [Tooltip("All renderers which are supposed to change their color when moused over their mouseOverSpriteRenderer")]
    [SerializeField] private List<SpriteRenderer> changeColorRenderers;
    [SerializeField] private Color mouseOverColor;
    [SerializeField] private Color beingDraggedColor;

    private void Awake()
    {
        Sprite spr = mouseOverSpriteRenderer.sprite;
        unscaledSpriteWorldSize = spr.rect.size * (1 / spr.pixelsPerUnit);

    }

    private void OnEnable()
    {
        beingHeld = null;
        beingMousedOver = null;
        wasMousedOverLastF = false;
    }

    private void Update()
    {
        Rect r = CalculateSpriteWorldRect();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool isMousedOver = false;

        if (r.Contains(mousePos, true))
        {
            isMousedOver = true;
            if (Input.GetKeyDown(KeyCode.Mouse0) && beingHeld == null) StartBeingHeld();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0)) StopBeingHeld();

        if (beingHeld == this)
        {
            Vector3 deltaPos = mousePos - mousePosLastF;
            transform.position += new Vector3(deltaPos.x, deltaPos.y, 0f);
        }

        // color change
        if (!wasMousedOverLastF && isMousedOver) OnMouseEnter();
        else if (wasMousedOverLastF && !isMousedOver) OnMouseExit();

        mousePosLastF = mousePos;
        wasMousedOverLastF = isMousedOver;
    }

    public void StartBeingHeld() // called when instantiating an entity from the ui
    {
        if (beingHeld != null) { beingHeld.Drop(); }
        beingHeld = this;
    }

    private void StopBeingHeld()
    {
        if (beingHeld != null) { beingHeld.Drop(); }
        beingHeld = null;
    }

    private void Drop()
    {
        OnDropped?.Invoke();
    }

    private void OnMouseEnter()
    {

    }

    private void OnMouseExit()
    {

    }

    private Rect CalculateSpriteWorldRect()
    {
        // Create the rect that represents the sprite in world space.
        // Don't forget that rects measure their position not in the middle, but the lower left corner
        Vector2 scaledSpriteWorldSize = Vector2.Scale(unscaledSpriteWorldSize, transform.lossyScale);
        Vector2 lowerLeftSpriteWorldCorner = (Vector2)transform.position - (scaledSpriteWorldSize * 0.5f);
        return new Rect(lowerLeftSpriteWorldCorner, scaledSpriteWorldSize);
    }
}
