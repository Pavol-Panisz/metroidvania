using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class BoxFillTool : TilemapTool
{
    [SerializeField] private Transform sliderToggleable;
    [SerializeField] private Slider slider;
    [SerializeField] private SpriteRenderer radiusIndicator;


    private int radius = 1;

    private void Start()
    {
        //currentlySelected = this;
        //OnChangedRadius();
    }

    void Update()
    {

        if (currentlySelected != this) return;

        Debug.Log("b: " + currentlySelected.ToString());
        radiusIndicator.transform.position = indicatorPos + Vector3.one * 0.5f;

        // placement
        if (Input.GetKey(KeyCode.Mouse0) && !CommonEditMode.isBeingInhibited && !EntityPlacement.beingMousedOverAnyThisIteration)
        {
            if (tilemapEditor.activeTile != null)
            {
                BoxFill(tilemapEditor.activeTile);
            }

            tilemapEditor.GenerateCurrentColliderGeometry();
        }
        // destruction
        if (Input.GetKey(KeyCode.Mouse1) && !CommonEditMode.isBeingInhibited)
        {
            BoxFill(null);
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
        sliderToggleable.gameObject.SetActive(true);
        radiusIndicator.gameObject.SetActive(true);
    }

    protected override void OnDeselected()
    {
        sliderToggleable.gameObject.SetActive(false);
        radiusIndicator.gameObject.SetActive(false);
    }

    public void OnChangedRadius()
    { 
        radius = (int)slider.value;
        radiusIndicator.transform.localScale = Vector3.one * radiusIndicator.sprite.pixelsPerUnit * (2*radius+1);
    }

    private void BoxFill(TileBase tile)
    {
        for (int xxx = indicatorPos.x - radius; xxx <= indicatorPos.x + radius; xxx++)
        {
            for (int yyy = indicatorPos.y - radius; yyy <= indicatorPos.y + radius; yyy++)
            {
                tilemapEditor.SetTile(new Vector3Int(xxx, yyy, 0), tile);
            }
        }
    }

    // lazyyyyyy
    protected override void OnSwitchedLayerTo(TilemapEditor.Layers layer)
    {
        if (layer == TilemapEditor.Layers.Foreground)
        {
            radiusIndicator.sortingLayerName = "Foreground";
        }
        else if (layer == TilemapEditor.Layers.Background)
        {
            radiusIndicator.sortingLayerName = "Background";
        }
        else if (layer == TilemapEditor.Layers.Damage)
        {
            radiusIndicator.sortingLayerName = "Water";
        }
    }
}
