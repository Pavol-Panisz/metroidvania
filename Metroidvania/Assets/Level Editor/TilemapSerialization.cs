using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

/*
 * Handles the saving and loading of the tilemaps.
 */
public class TilemapSerialization : MonoBehaviour
{
    [SerializeField] private TilemapEditor tilemapEditor;

    [System.Serializable]
    private class TileRepresentation
    {
        public TileBase tile;
        public char chr;
    }
    [Tooltip("Used for creating a dictionary for looking up what char corresponds to a given tile")]
    [SerializeField] private List<TileRepresentation> tileRepresentations = new List<TileRepresentation>();
    private Dictionary<TileBase, char> tileCharsDict = new Dictionary<TileBase, char>();


    [System.Serializable]
    private class TileMapRepresentation
    {
        [SerializeField] private Tilemap associatedTilemap;
        private char[][] arr;

        private Vector2Int size;

        public void Initialize(TilemapEditor tilemapEditor, Dictionary<TileBase, char> tileToCharDict)
        {
            size = tilemapEditor.GetTilemapSize();
            arr = new char[size.x][];

            // It's <= / >= because if the two corners are the same, it means the tilemap's 1x1 tiles large
            for (int xxx = 0; xxx <= size.x; xxx++) 
            {
                for (int yyy = 0; yyy <= size.y; yyy++)
                {
                    arr[xxx][yyy] = tileToCharDict[null]; // populate it all with air for now
                }
            }

            Debug.Log("init");
        }
    
        /// <summary>
        /// Sets the tilemap's char representaton tile. DOES NOT place the tile on the actual
        /// tilemap!
        /// </summary>
        public void SetTileChar(TileBase tile, Vector3Int pos)
        {
            Vector2Int arrPos = new Vector2Int(0, 0);
            //arrPos.x = 
        }

        
    }

    [SerializeField] private List<TileMapRepresentation> tilemapRepresentations = 
                new List<TileMapRepresentation>();
    private Dictionary<Tilemap, TileMapRepresentation> tilemapToCharMapDict = 
                new Dictionary<Tilemap, TileMapRepresentation>();

    private void Awake()
    {
        // create the tile-to-char dictionary
        List<char> used = new List<char>();
        foreach (TileRepresentation tr in tileRepresentations)
        {
            if (used.Contains(tr.chr))
            {
                Debug.LogError($"More than 1 tile has the character '{tr.chr}' assigned! In: {tr.tile.name}");
            }
            tileCharsDict.Add(tr.tile, tr.chr);
            used.Add(tr.chr);
        }
    
        // create the individual char maps
        foreach (var chrmp in tilemapRepresentations)
        {
            chrmp.Initialize(tilemapEditor, tileCharsDict);
        }
    }

    private void OnEnable()
    {
        tilemapEditor.OnPlacedTile += OnPlacedTile;
    }

    private void OnDisable()
    {
        tilemapEditor.OnPlacedTile -= OnPlacedTile;
    }

    // called when placing a tile in the editor with the mouse
    private void OnPlacedTile(TileBase tile, Tilemap tilemap, Vector3Int pos)
    {
        // So, the tile's been placed onto the tilemap. But we need
        // to record this change in the charmap, which is exactly what
        // this method does.
        tilemapToCharMapDict[tilemap].SetTileChar(tile, pos); 
    }
}
