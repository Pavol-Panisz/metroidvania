using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapEditor : MonoBehaviour
{
    private Vector3 mousePosLastF;

    private bool isEditMode = false;
    private bool isHoldingTile = true;

    public Tilemap tilemap;
    public CompositeCollider2D col;

    public RuleTile ruleTile;
    public Tile tile;

    public Transform tilePlacementIndicator;
    private SpriteRenderer tilePlacementIndicatorSprite;

    public GameObject toggleable;
    public Transform playerTransform;

    // set from a TilemapPlacementInhibitor, e.g. a button in the UI
    public bool isBeingInhibited = false;

    private void Awake()
    {
        tilePlacementIndicatorSprite = tilePlacementIndicator.GetComponent<SpriteRenderer>();
    }

    public void SetMode(LevelControl.Modes mode)
    {
        if (mode == LevelControl.Modes.Edit)
        {
            isEditMode = true;
            tilePlacementIndicatorSprite.enabled = true;
            toggleable.SetActive(true);
        }
        else
        {
            isEditMode = false;
            tilePlacementIndicatorSprite.enabled = false;
            toggleable.SetActive(false);
        }
    }

    public void Update()
    {
        if (!isEditMode) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKey(KeyCode.Mouse2))
        {
            Camera.main.transform.position += mousePosLastF - mousePos;
        }
        Camera.main.orthographicSize -= Input.mouseScrollDelta.y * 0.3f;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 2f, 25f);

        if (isHoldingTile)
        {
            var tilePos = new Vector3Int((int)mousePos.x, (int)mousePos.y, 0);
            tilePlacementIndicator.position = tilePos;
            tilePlacementIndicator.position += new Vector3(0.5f, 0.5f, 0f);
            tilePlacementIndicatorSprite.sprite = tile.sprite;
            
            // placement
            if (Input.GetKey(KeyCode.Mouse0) && !isBeingInhibited)
            {
                if (ruleTile != null) tilemap.SetTile(tilePos, ruleTile);
                else if (tile != null) tilemap.SetTile(tilePos, tile);

                if (col != null) col.GenerateGeometry();
            }
            // destruction
            if (Input.GetKey(KeyCode.Mouse1) && !isBeingInhibited)
            {
                tilemap.SetTile(tilePos, null);
            }


        }

        mousePosLastF = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void CenterOnPlayer()
    {
        Camera.main.transform.position = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y,
                Camera.main.transform.position.z
        );

        Camera.main.orthographicSize = 5f;
    }

    // Called when clicking on a tile button
    public void SetActiveTile(Tile tile)
    {
        isHoldingTile = true;
        ruleTile = null;
        this.tile = tile;
    }
    public void SetActiveRuleTile(RuleTile ruleTile)
    {
        isHoldingTile = true;
        this.tile = null;
        this.ruleTile = ruleTile;
    }

    


}
