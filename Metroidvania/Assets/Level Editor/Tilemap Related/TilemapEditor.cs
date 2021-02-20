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
    public CompositeCollider2D currentCollider;

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
    public SpriteRenderer tilePlacementIndicatorSprite;

    public enum Layers { Foreground, Background, Damage }
    public Layers activeLayer = Layers.Foreground;
    private Dictionary<Layers, string> layerToEnumStr = new Dictionary<Layers, string>();

    // for this layer, which layers can be edited (for example foreground - foreground & ladder)
    private Dictionary<Layers, Tilemap[]> associatedInternalLayersDict = new Dictionary<Layers, Tilemap[]>();

    [Space]
    [Header("Regarding the layer buttons")]
    [SerializeField] private Image foregroundSelectedIndicator;
    [SerializeField] private Image backgroundSelectedIndicator;
    [SerializeField] private Image damageSelectedIndicator;

    [Space]
    [SerializeField] private TileBase ladderTile;
    [SerializeField] private TileBase lavaTile;

    public Action<TileBase, Tilemap, Vector3Int> OnPlacedTile;

    // called after updating the indicator pos in Update()
    // Just realized that it's useless, since this update method can still
    // execute after the update method of a tilemaptool. whatever.
    public Action<Vector3Int> OnUpdatedIndicatorPos;
    public Action<Layers> OnSwitchedLayerTo;

    [Space]
    [SerializeField] public Color fadedTilemapColor;
    private List<Tilemap> tilemapList = new List<Tilemap>();

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

                    Vector3Int pos = (Vector3Int)charmap.CharmapToTilemapCoords(new Vector2Int(xxx, currentY));

                    if (chr == TilemapSerialization.airChar)
                    {
                        tilemapEditor.SetTile(pos, null);
                    } else
                    {
                        try
                        {
                            TileBase tile = tilemapSerialization.charToTileDict[chr];
                            tilemapEditor.SetTile(pos, tile);
                        }
                        catch (Exception e)
                        {
                            Debug.Log($"couldnt find char {chr}\n{e.Message}");
                        }
                    }

                } 

               
            }
            currentY++;
        }
    }

    private void Awake()
    {
        tilePlacementIndicatorSprite = tilePlacementIndicator.GetComponent<SpriteRenderer>();

        tilemapList.Add(foregroundTilemap);
        tilemapList.Add(backgroundTilemap);
        tilemapList.Add(damageTilemap);
        tilemapList.Add(lavaTilemap);
        tilemapList.Add(ladderTilemap);

        // implemented only after all the damage (mapping in SetActiveLayer) had been done
        layerToEnumStr.Add(Layers.Background, "Background");
        layerToEnumStr.Add(Layers.Foreground, "Foreground");
        layerToEnumStr.Add(Layers.Damage, "Damage");

        associatedInternalLayersDict.Add(Layers.Foreground, new Tilemap[] { foregroundTilemap, ladderTilemap });
        associatedInternalLayersDict.Add(Layers.Background, new Tilemap[] { backgroundTilemap });
        associatedInternalLayersDict.Add(Layers.Damage, new Tilemap[] { damageTilemap, lavaTilemap });
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

        OnUpdatedIndicatorPos(tilePos);

        tilePlacementIndicator.position += new Vector3(0.5f, 0.5f, 0f);

        EntityPlacement.beingMousedOverAnyThisIteration = false;

        mousePosLastF = mousePos;
    }

    // Caled by the 3 layer select buttons
    public void SetActiveLayer(string str) // duplicate code, but whatever for the moment
    {
        if (str == "Foreground")
        {
            activeLayer = Layers.Foreground;
            currentCollider = foregroundCol;
            currentTilemap = foregroundTilemap;
            DarkenLayersExcept(ladderTilemap, foregroundTilemap);

            foregroundSelectedIndicator.color = new Color(1f, 1f, 1f, 1f);
            backgroundSelectedIndicator.color = new Color(1f, 1f, 1f, 0f);
            damageSelectedIndicator.color = new Color(1f, 1f, 1f, 0f);

        } else if (str == "Background")
        {
            activeLayer = Layers.Background;
            currentCollider = null;
            currentTilemap = backgroundTilemap;
            DarkenLayersExcept(backgroundTilemap);

            foregroundSelectedIndicator.color = new Color(1f, 1f, 1f, 0f);
            backgroundSelectedIndicator.color = new Color(1f, 1f, 1f, 1f);
            damageSelectedIndicator.color = new Color(1f, 1f, 1f, 0f);
        }
        else if (str == "Damage")
        {
            activeLayer = Layers.Damage;
            currentCollider = damageCol;
            currentTilemap = damageTilemap;
            DarkenLayersExcept(damageTilemap, lavaTilemap);

            foregroundSelectedIndicator.color = new Color(1f, 1f, 1f, 0f);
            backgroundSelectedIndicator.color = new Color(1f, 1f, 1f, 0f);
            damageSelectedIndicator.color = new Color(1f, 1f, 1f, 1f);
        } else
        {
            Debug.LogError($"Unknown layer set {str}");
            return;
        }
        OnSwitchedLayerTo?.Invoke(activeLayer);
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
            DarkenLayersExcept(associatedInternalLayersDict[activeLayer]);
        }
        else if (mode == LevelControl.Modes.Play)
        {
            isInEditMode = false;

            foregroundCol.GenerateGeometry();
            ladderCol.GenerateGeometry();
            damageCol.GenerateGeometry();
            lavaCol.GenerateGeometry();
            DarkenLayersExcept(tilemapList.ToArray());
        }   
    }

    private void OnSwitchedEditingAction(CommonEditMode.EditingActions action)
    {
        if (action == CommonEditMode.EditingActions.Entity_Placement)
        {
            isEditingTilemap = false;
            tilePlacementIndicator.position = Vector3.one * 0.5f;
            DarkenLayersExcept(tilemapList.ToArray());

            // dependency
            TilemapTool.DeselectCurrent();

        }
        else if (action == CommonEditMode.EditingActions.Tile_Placement)
        {
            isEditingTilemap = true;
            SetActiveLayer(layerToEnumStr[activeLayer]);

            if (TilemapTool.currentlySelected == null) TilemapTool.SelectLastDeselected();
        }
    }

    public Vector2Int GetTilemapSize()
    {
        return new Vector2Int(upperRight.x - lowerLeft.x + 1, upperRight.y - lowerLeft.y + 1);
    }

    /// <summary>
    /// Handles all logic for placing tiles, such as overrides for lava and ladders.
    /// </summary>
    public void SetTile(Vector3Int tilePos, TileBase tile) 
    {
        tilePos.x = Mathf.Clamp(tilePos.x, lowerLeft.x, upperRight.x);
        tilePos.y = Mathf.Clamp(tilePos.y, lowerLeft.x, upperRight.y);

        if (tile == null) // if deleting
        {
            currentTilemap.SetTile(tilePos, null);
            OnPlacedTile?.Invoke(tile, currentTilemap, tilePos);

            if (activeLayer == Layers.Damage && lavaTilemap.GetTile(tilePos) == lavaTile)
            {
                lavaTilemap.SetTile(tilePos, null);
                lavaCol.GenerateGeometry();
            }
            else if (activeLayer == Layers.Foreground && ladderTilemap.GetTile(tilePos) == ladderTile)
            {
                ladderTilemap.SetTile(tilePos, null);
                ladderCol.GenerateGeometry(); 
            }
        }
        else // if placing
        {
            if (tile == lavaTile && activeLayer == Layers.Damage)
            {
                lavaTilemap.SetTile(tilePos, tile);
                currentTilemap.SetTile(tilePos, null);
                lavaCol.GenerateGeometry();

            }
            else if (tile == ladderTile && activeLayer == Layers.Foreground)
            {
                ladderTilemap.SetTile(tilePos, tile);
                currentTilemap.SetTile(tilePos, null);
                ladderCol.GenerateGeometry();
            }
            else
            {
                currentTilemap.SetTile(tilePos, tile);

                if (activeLayer == Layers.Damage && lavaTilemap.GetTile(tilePos) == lavaTile)
                {
                    lavaTilemap.SetTile(tilePos, null);
                    lavaCol.GenerateGeometry();
                }
                else if (activeLayer == Layers.Foreground && ladderTilemap.GetTile(tilePos) == ladderTile)
                {
                    ladderTilemap.SetTile(tilePos, null);
                    ladderCol.GenerateGeometry();
                }
            }
            OnPlacedTile?.Invoke(tile, currentTilemap, tilePos);
        }
    }

    private void DarkenLayersExcept(params Tilemap[] p)
    {
        foreach (var t in tilemapList)
        {
            foreach (var excepted in p)
            {
                t.color = fadedTilemapColor;
                if (t == excepted)
                {
                    t.color = Color.white;
                    break;
                }
            }
        }
    }

    public void GenerateCurrentColliderGeometry()
    {
        if (currentCollider != null) currentCollider.GenerateGeometry();
    }
}
