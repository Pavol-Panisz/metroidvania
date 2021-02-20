using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTool : TilemapTool
{

    private void Awake()
    {
        lastDeselected = this;
    }

    private void Update()
    {
        if (currentlySelected != this) return;

        // placement
        if (Input.GetKey(KeyCode.Mouse0) && !CommonEditMode.isBeingInhibited && !EntityPlacement.beingMousedOverAnyThisIteration)
        {
            if (tilemapEditor.activeTile != null)
            {
                tilemapEditor.SetTile(indicatorPos, tilemapEditor.activeTile);
            }

            tilemapEditor.GenerateCurrentColliderGeometry();
        }
        // destruction
        if (Input.GetKey(KeyCode.Mouse1) && !CommonEditMode.isBeingInhibited)
        {
            tilemapEditor.SetTile(indicatorPos, null);
        }
    }

    protected override void OnMouseUp()
    {

    }
    protected override void OnMouseDown()
    {
        Debug.Log("clicked boxfill");
    }
    protected override void OnMouseEnter()
    {
        Debug.Log("works boxfill");
    }
    protected override void OnMouseExit()
    {

    }

    protected override void OnSelected()
    {
        
    }

    protected override void OnDeselected()
    {

    }
}
