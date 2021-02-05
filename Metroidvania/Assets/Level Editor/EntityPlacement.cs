using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EntityPlacement : MonoBehaviour
{
    private Vector3 mousePosLastF;

    public static EntityPlacement beingHeld = null;

    public Action OnDropped;

    public static bool isInhibited = false;

    // the x and y size of the sprite in world space, not accounting for 
    // scaling of the transform, which holds the sprite renderer
    Vector2 unscaledSpriteWorldSize;

    [Tooltip("The spriteRenderer, over whose sprite a mouse-over will be registered. It should be " 
        + " on the same gameobject as this script.")]
    [SerializeField] private SpriteRenderer mouseOverSpriteRenderer;

    private void Start()
    {
        Sprite spr = mouseOverSpriteRenderer.sprite;
        unscaledSpriteWorldSize = spr.rect.max * (1 / spr.pixelsPerUnit);

    }

    private void Update()
    {
        // Create the rect that represents the sprite in world space.
        // Don't forget that rects measure their position not in the middle, but the lower left corner
        Vector2 scaledSpriteWorldSize = Vector2.Scale(unscaledSpriteWorldSize, transform.lossyScale);
        Vector2 lowerLeftSpriteWorldCorner = (Vector2)transform.position - (scaledSpriteWorldSize * 0.5f);
        Rect spriteWorldRect = new Rect(lowerLeftSpriteWorldCorner, scaledSpriteWorldSize);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (spriteWorldRect.Contains(mousePos, true)) Debug.Log("is in");

        // we only react to clicks
        if (!isInhibited && Input.GetKeyDown(KeyCode.Mouse0)) {
            
            // if nothing has been held up until now & we just clicked on this
            if (beingHeld == null && spriteWorldRect.Contains(mousePos, true))
            {
                beingHeld = this;
            }
            // if something was being held & it's this
            else if (beingHeld == this)
            {
                beingHeld = null;
                OnDropped?.Invoke();
            }
        }

        if (beingHeld == this) // if is being held, always go to the mouse position
        {
            transform.position = new Vector3(mousePos.x, mousePos.y, 0f);
        }

        mousePosLastF = mousePos;
    }

    public void StartBeingHeld() // called when instantiating an entity from the ui
    {
        if (beingHeld != null) { beingHeld.OnDropped?.Invoke(); }
        beingHeld = this;
    }
}
