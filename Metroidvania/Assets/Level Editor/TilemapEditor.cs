using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System;

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
    public Vector2Int lowerLeft;
    public Vector2Int upperRight;
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
    public TileBase activeTile;
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

    public Action<TileBase, Tilemap, Vector3Int> OnPlacedTile;

    public class FromInstructionsBuilder
    {
        private int currentY = 0;
        private TilemapSerialization tilemapSerialization;
        private TilemapEditor tilemapEditor;
        
        private TilemapSerialization.TileMapRepresentation charmap;
        private Tilemap tilemap;

        public FromInstructionsBuilder(string layerEnumStr, TilemapEditor tE, TilemapSerialization tS)
        {

            tE.SetActiveLayer(layerEnumStr);
            tilemapSerialization = tS;
            tilemapEditor = tE;


            tilemap = tE.currentTilemap;
            charmap = tS.tilemapToCharMapDict[tilemap]; 
        }

        public void BuildRowFromLine(string line)
        {
            line.Trim(); // remove any whitespace
            for (int xxx=0; xxx < line.Length; xxx++)
            {
                char chr = line[xxx];
                // if not placing the same tile that's already there or some error tile
                char inCharmap = charmap.GetCharAt(xxx, currentY, TilemapSerialization.errorChar);
                if ((inCharmap != chr) && (inCharmap != TilemapSerialization.errorChar)) {

                    TileBase tile = tilemapSerialization.charToTileDict[chr];

                    tilemapEditor.SetTile((Vector3Int)charmap.TilemapToCharmapCoords(new Vector3Int(xxx, currentY, 0)), tile);
                } 

               
            }
            currentY++;
        }
    }

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
            if (activeTile != null)
            {
                SetTile(tilePos, activeTile);

                // I realized at this point that all tiles inherit from TileBase. That's why
                // up until now, I've been "differentiating" between ruleTiles & normal tiles :p
                OnPlacedTile?.Invoke(activeTile, currentTilemap, tilePos);
            }
            else if (activeTile != null)
            {
                SetTile(tilePos, activeTile);
                OnPlacedTile?.Invoke(activeTile, currentTilemap, tilePos);
            }


            if (currentCollider != null) currentCollider.GenerateGeometry();
        }
        // destruction
        if (Input.GetKey(KeyCode.Mouse1) && !CommonEditMode.isBeingInhibited)
        {
            

            SetTile(tilePos, null);
            OnPlacedTile?.Invoke(null, currentTilemap, tilePos);
            // generated when entering play mode
            //if (currentCollider != null) currentCollider.GenerateGeometry();
        }

        // TODO tile placement & special rules when placing lava or ladders
        

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
            currentCollider = damageCol;
            currentTilemap = damageTilemap;

            foregroundSelectedIndicator.color = new Color(1f, 1f, 1f, 0f);
            backgroundSelectedIndicator.color = new Color(1f, 1f, 1f, 0f);
            damageSelectedIndicator.color = new Color(1f, 1f, 1f, 1f);
        } else
        {
            Debug.LogError($"Unknown layer set {str}");
        }
    }

    // Called by each tile icon
    public void SetActiveTile(Tile tile) // Two separate functions because previously, I differentiated between tiles. Not anymore.
    {
        this.activeTile = tile;
        tilePlacementIndicatorSprite.sprite = tile.sprite;
    }

    public void SetActiveRuleTile(RuleTile ruleTile) // Two separate functions because previously, I differentiated between tiles. Not anymore.
    {
        this.activeTile = ruleTile;
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

        foregroundCol.GenerateGeometry();
        ladderCol.GenerateGeometry();
        damageCol.GenerateGeometry();
        lavaCol.GenerateGeometry();
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

    public Vector2Int GetTilemapSize()
    {
        return new Vector2Int(upperRight.x - lowerLeft.x + 1, upperRight.y - lowerLeft.y + 1);
    }

    /// <summary>
    /// Handles all logic for placing tiles, such as overrides for lava and ladders.
    /// </summary>
    private void SetTile(Vector3Int tilePos, TileBase tile) 
    {
        currentTilemap.SetTile(tilePos, tile);
    }
}
