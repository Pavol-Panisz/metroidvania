﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TilemapEditor : MonoBehaviour
{
    [SerializeField] CommonEditMode editModeScript;
    private bool isEditingTilemap = false;
    private bool isInEditMode = false;
    private bool isBeingInhibited = false;

    private Vector3 mousePosLastF;

    private Tilemap currentTilemap;
    private CompositeCollider2D currentCollider;

    [SerializeField] private LevelControl levelControl;
    [SerializeField] private CommonEditMode editModeControl;
    [Space]
    [Header("The min and max placeable points")]
    [SerializeField] private Vector2Int lowerLeft;
    [SerializeField] private Vector2Int upperRight;
    [Space]
    [Header("Colliders of different layers")]
    public CompositeCollider2D foregroundCol;
    public CompositeCollider2D ladderCol;
    public CompositeCollider2D damageCol;
    public CompositeCollider2D lavaCol;
    [Space]
    [Header("Tilemaps of different layers")]
    public Tilemap foregroundTilemap;
    public Tilemap ladderTilemap;
    public Tilemap damageTilemap;
    public Tilemap lavaTilemap;
    public Tilemap backgroundTilemap;

    [Space]
    [Header("The currently selected tiles - just for show")]
    public RuleTile ruleTile;
    public Tile tile;
    [Space]
    public Transform tilePlacementIndicator;
    private SpriteRenderer tilePlacementIndicatorSprite;

    public enum Layers { Foreground, Background, Damage }
    public Layers activeLayer = Layers.Foreground;

    [Space]
    [Header("Regarding the layer buttons")]
    [SerializeField] private Image foregroundSelectedIndicator;
    [SerializeField] private Image backgroundSelectedIndicator;
    [SerializeField] private Image damageSelectedIndicator;

    private void Awake()
    {
        tilePlacementIndicatorSprite = tilePlacementIndicator.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        SetActiveLayer("Foreground");
    }

    private void OnEnable()
    {
        levelControl.OnSwitchedModeTo += OnSwitchedMode;
        editModeControl.OnSwitchedEditingActionTo += OnSwitchedEditingAction;
    }
    private void OnDisable()
    {
        levelControl.OnSwitchedModeTo -= OnSwitchedMode;
        editModeControl.OnSwitchedEditingActionTo -= OnSwitchedEditingAction;
    }

    private void Update()
    {
        if (!isInEditMode || !isEditingTilemap) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        var tilePos = new Vector3Int((int)mousePos.x, (int)mousePos.y, 0);
        tilePos.x = Mathf.Clamp(tilePos.x, lowerLeft.x, upperRight.x);
        tilePos.y = Mathf.Clamp(tilePos.y, lowerLeft.x, upperRight.y);
        tilePlacementIndicator.position = tilePos;
        tilePlacementIndicator.position += new Vector3(0.5f, 0.5f, 0f);

        // placement
        if (Input.GetKey(KeyCode.Mouse0) && !CommonEditMode.isBeingInhibited)
        {
            if (ruleTile != null) currentTilemap.SetTile(tilePos, ruleTile);
            else if (tile != null) currentTilemap.SetTile(tilePos, tile);

            if (currentCollider != null) currentCollider.GenerateGeometry();
        }
        // destruction
        if (Input.GetKey(KeyCode.Mouse1) && !CommonEditMode.isBeingInhibited)
        {
            

            currentTilemap.SetTile(tilePos, null);
            if (currentCollider != null) currentCollider.GenerateGeometry();
        }

        // LEFT OFF - tile placement & special rules when placing lava or ladders
        

        //mousePosLastF = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    // Caled by the 3 layer select buttons
    public void SetActiveLayer(string str) // duplicate code, but whatever for the moment
    {
        if (str == "Foreground")
        {
            activeLayer = Layers.Foreground;
            currentCollider = foregroundCol;
            currentTilemap = foregroundTilemap;

            foregroundSelectedIndicator.color = new Color(1f, 1f, 1f, 1f);
            backgroundSelectedIndicator.color = new Color(1f, 1f, 1f, 0f);
            damageSelectedIndicator.color = new Color(1f, 1f, 1f, 0f);

        } else if (str == "Background")
        {
            activeLayer = Layers.Background;
            currentCollider = null;
            currentTilemap = backgroundTilemap;

            foregroundSelectedIndicator.color = new Color(1f, 1f, 1f, 0f);
            backgroundSelectedIndicator.color = new Color(1f, 1f, 1f, 1f);
            damageSelectedIndicator.color = new Color(1f, 1f, 1f, 0f);
        }
        else if (str == "Damage")
        {
            activeLayer = Layers.Damage;
            currentCollider = lavaCol;
            currentTilemap = lavaTilemap;

            foregroundSelectedIndicator.color = new Color(1f, 1f, 1f, 0f);
            backgroundSelectedIndicator.color = new Color(1f, 1f, 1f, 0f);
            damageSelectedIndicator.color = new Color(1f, 1f, 1f, 1f);
        } else
        {
            Debug.LogError($"Unknown layer set {str}");
        }
    }

    // Called by each tile icon
    public void SetActiveTile(Tile tile)
    {
        this.tile = tile;
        this.ruleTile = null;
        tilePlacementIndicatorSprite.sprite = tile.sprite;
    }

    public void SetActiveRuleTile(RuleTile ruleTile)
    {
        this.tile = null;
        this.ruleTile = ruleTile;
        tilePlacementIndicatorSprite.sprite = ruleTile.m_DefaultSprite;
    }

    private void OnSwitchedMode(LevelControl.Modes mode)
    {
        if (mode == LevelControl.Modes.Edit) 
        {
            isInEditMode = true;
        }
        else if (mode == LevelControl.Modes.Play)
        {
            isInEditMode = false;
        }   
    }

    private void OnSwitchedEditingAction(CommonEditMode.EditingActions action)
    {
        if (action == CommonEditMode.EditingActions.Entity_Placement)
        {
            isEditingTilemap = false;
        }
        else if (action == CommonEditMode.EditingActions.Tile_Placement)
        {
            isEditingTilemap = true;
        }
    }
}