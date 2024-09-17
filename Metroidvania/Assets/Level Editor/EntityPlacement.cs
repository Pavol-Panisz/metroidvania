using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EntityPlacement : MonoBehaviour
{
    private Vector3 mousePosLastF;

    public static EntityPlacement beingHeld = null;
    public static bool beingMousedOverAnyThisIteration = false; 

    private bool wasMousedOverLastF = false;

    private static int mousedOverCount = 0;

    // constraints, between which this entity's coords will get clamped
    private Vector2 lowerLeft;
    private Vector2 upperRight;

    public Action OnStartedBeingHeld;
    public Action OnDropped;

    // the x and y size of the sprite in world space, not accounting for 
    // scaling of the transform, which holds the sprite renderer
    Vector2 unscaledSpriteWorldSize;

    private Vector2 normalizedPivot;

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

        normalizedPivot = spr.pivot / spr.rect.size;
        //Debug.Log(transform.name + "" + normalizedPivot);
        Debug.Log(transform.name + " " + normalizedPivot);
    }

    private void OnEnable()
    {
        beingHeld = null;
        wasMousedOverLastF = false;
    }

    private void Update()
    {
        Rect r = CalculateSpriteWorldRect();

        Debug.DrawLine(r.position, r.position + Vector2.right * r.width, Color.red);
        Debug.DrawLine(r.position, r.position - Vector2.down* r.height, Color.red);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool isMousedOver = false;

        if (r.Contains(mousePos, true))
        {
            isMousedOver = true;
            if (Input.GetKeyDown(KeyCode.Mouse0) && beingHeld == null) StartBeingHeld();

            // EXTREMELY HACKY:
            // gets reset in tilemap editor, but only once all entity placement updates have finished.
            // the way this works is that all EntityPlacement.Updates execute before TilemapEditor.Update,
            // cause I set it so in the script execution order.
            // TilemapEditor, once that it's finished with beingMousedOverAnyThisIteration, sets it to false.
            beingMousedOverAnyThisIteration = true; 
        }

        if (Input.GetKeyUp(KeyCode.Mouse0)) StopBeingHeld();

        if (beingHeld == this)
        {
            Vector3 deltaPos = mousePos - mousePosLastF;
            transform.position += new Vector3(deltaPos.x, deltaPos.y, 0f);

            var clamped = new Vector3(Mathf.Clamp(transform.position.x, lowerLeft.x, upperRight.x),
                                      Mathf.Clamp(transform.position.y, lowerLeft.y, upperRight.y),
                                      0f);
            transform.position = clamped;

            beingMousedOverAnyThisIteration = true;
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

        OnStartedBeingHeld?.Invoke();
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
        mousedOverCount++;
        foreach (var renderer in changeColorRenderers) renderer.color = mouseOverColor;
    }

    private void OnMouseExit()
    {
        mousedOverCount--;
        foreach (var renderer in changeColorRenderers) renderer.color = Color.white;
    }

    private Rect CalculateSpriteWorldRect()

    {
        // Create the rect that represents the sprite in world space.
        // Don't forget that rects measure their position not in the middle, but the lower left corner
        Vector2 scaledSpriteWorldSize = Vector2.Scale(unscaledSpriteWorldSize, transform.lossyScale);
        Vector2 lowerLeftSpriteWorldCorner = (Vector2)transform.position - (scaledSpriteWorldSize * 0.5f);
        var rect = new Rect(lowerLeftSpriteWorldCorner, scaledSpriteWorldSize);

        // the rect will be centered at the pivot point, but we want it centered at the center of the sprite
        rect.position -= (normalizedPivot - Vector2.one * 0.5f )  * scaledSpriteWorldSize;

        return rect;
    }

    public void SetConstraints(Vector2Int ll, Vector2Int ur)
    {
        lowerLeft = (Vector2)ll;
        upperRight = (Vector2)ur;
    }

    public void OnDestroy()
    {
        
    }
}
