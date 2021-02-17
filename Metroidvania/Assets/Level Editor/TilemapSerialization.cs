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

    public static char airChar = '.';

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
    public class TileMapRepresentation
    {
        public Tilemap associatedTilemap;
        [Tooltip("The mapping from the layer string to TilemapEditor.Layer, as seen in TilemapEditor.SetActiveLayer(string)")]
        public string tilemapLayerStr;
        public string saveSystemLayerId;
        private char[][] arr;

        private Vector2Int size;

        Dictionary<TileBase, char> tileToChar;

        public void Initialize(TilemapEditor tilemapEditor, Dictionary<TileBase, char> tileToCharDict)
        {
            size = tilemapEditor.GetTilemapSize();
            arr = new char[size.x][];

            this.tileToChar = tileToCharDict;

            for (int xxx = 0; xxx < size.x; xxx++)
            {
                arr[xxx] = new char[size.y];

                for (int yyy = 0; yyy < size.y; yyy++)
                {
                    arr[xxx][yyy] = airChar; // populate it all with air for now
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
            arrPos = TilemapToCharmapCoords(pos);

            //Debug.Log($"placing at {arrPos.ToString()}");
            arr[arrPos.x][arrPos.y] = tileToChar[tile];
        }

        public void SetAir(Vector3Int pos)
        {
            Vector2Int arrPos = new Vector2Int(0, 0);
            arrPos = TilemapToCharmapCoords(pos);
            arr[arrPos.x][arrPos.y] = airChar;
        }

        public Vector2Int TilemapToCharmapCoords(Vector3Int pos)
        {
            Vector2Int arrPos = new Vector2Int(0, 0);
            arrPos.x = pos.x - 1;
            arrPos.y = size.y - pos.y;

            return arrPos;
        }

        public Vector2Int CharmapToTilemapCoords(Vector2Int pos)
        {
            Vector2Int chrArrPos = new Vector2Int(0, 0);
            chrArrPos.x = pos.x + 1;
            chrArrPos.y = -pos.y + size.y;

            return chrArrPos;
        }

        public string GetCharmapString()
        {
            string content = "";

            for (int yyy=0; yyy < size.y; yyy++)
            {
                char[] line = new char[size.x];
                for (int xxx = 0; xxx < size.x; xxx++)
                {
                    line[xxx] = arr[xxx][yyy];
                }
                content += new string(line) + "\n";
            }

            Debug.Log(content);
            return content;
        }

    }

    // public, cause SavingSystem will go through each charmap to get its string into the save file
    public List<TileMapRepresentation> tilemapRepresentations =
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

        // create the individual char maps and then the tilemap dict
        foreach (var chrmp in tilemapRepresentations)
        {
            chrmp.Initialize(tilemapEditor, tileCharsDict);
            tilemapToCharMapDict.Add(chrmp.associatedTilemap, chrmp);
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
        if (tile == null) { tilemapToCharMapDict[tilemap].SetAir(pos); }

        else { tilemapToCharMapDict[tilemap].SetTileChar(tile, pos); }
    }

    /// <summary>
    /// Gets the corresponding layer-enum-string (the mapping in TilemapEditor.SetActiveLayer()) from the save system id.
    /// Is an O(n) operation.
    /// </summary>
    public string GetLayerEnumStrFromSaveSysId(string ssid)
    {
        foreach (var charmap in tilemapRepresentations)
        {
            if (charmap.saveSystemLayerId == ssid)
            {
                return charmap.tilemapLayerStr;
            }
        }
        return null;
    }
}
