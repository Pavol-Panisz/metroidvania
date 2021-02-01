using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RuntimeTest : MonoBehaviour
{
    public Tilemap tilemap;
    public CompositeCollider2D col;
    public Tile tile;

    public Transform playerTransform;

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {

            var p = playerTransform.position;


            Vector3Int pos = new Vector3Int((int)p.x, (int)p.y + 4, (int)p.z);

            tilemap.SetTile(pos, tile);
            col.GenerateGeometry();

            
        }
    }
}