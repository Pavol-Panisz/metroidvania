using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Importer : MonoBehaviour
{
    [Tooltip("In case the parsing of a vector 3 goes wrong, default to this value")]
    [SerializeField] private Vector3 errVec = Vector3.one;

    [SerializeField] private TilemapSerialization tilemapSerialization;
    [SerializeField] private TilemapEditor tilemapEditor;
    [SerializeField] private Entities entities;

    [SerializeField] private Transform checkpointTextParent;

    [SerializeField] private TextAsset sampleLevel;

    public ScreenFader screenFader;

    private string currentMassPlacedEntity = null;

    TilemapEditor.FromInstructionsBuilder tilemapBuilder = null;
    private bool startedEditingTilemap = false; // no need to save this. the tilemapeditor takes care of the rest ;)


    private void Start()
    {
        //Import(sampleLevel.text);

        Import(sampleLevel.text);
    }

    public void Import(string content)
    {
        tilemapBuilder = null;
        startedEditingTilemap = false;

        // destroy all entites except player and level_exit
        for (int iii = 0; iii < entities.shootingEnemies.Count; iii++) { entities.Destroy(entities.shootingEnemies[iii]); }
        entities.shootingEnemies.Clear();

        for (int iii = 0; iii < entities.walkingEnemies.Count; iii++) { entities.Destroy(entities.walkingEnemies[iii]); }
        entities.walkingEnemies.Clear();

        for (int iii=0; iii < entities.checkpoints.Count; iii++) { entities.Destroy(entities.checkpoints[iii]); }
        entities.checkpoints.Clear();

        for (int iii=0; iii < checkpointTextParent.childCount; iii++) 
                { Destroy(checkpointTextParent.GetChild(iii).gameObject); } 

        string[] lines = content.Split('\n');
        for (int iii=0; iii < lines.Length; iii++)
        {
            /*try
            {*/
                string line = lines[iii].Trim('\n', '\r');

                if (line == "" || line.StartsWith(SavingSystem.saveFileCommentStr)) continue;

                // need to check this before trying to build the row! Otherwise it'll think this is also an instruction
                else if (IsATilemapLayerId(line) != null)
                {
                    string layerEnumStr = tilemapSerialization.GetLayerEnumStrFromSaveSysId(line);

                    // from now on, all instructions on tile placement are for this layer
                    tilemapBuilder = new TilemapEditor.FromInstructionsBuilder(layerEnumStr, tilemapEditor, tilemapSerialization);
                    startedEditingTilemap = true;
                }

                else if (startedEditingTilemap)
                {
                    tilemapBuilder.BuildRowFromLine(line);
                    Debug.Log("set line");
                }

                else if (line == entities.player.saveSystemId)
                {
                    entities.player.transform.position = ParseVec3(lines[iii + 1], errVec);
                    iii++; // skip next line
                    entities.player.UpdateTransform();
                }
                else if (line == entities.levelExit.saveSystemId)
                {
                    entities.levelExit.transform.position = ParseVec3(lines[iii + 1], errVec); // next line's the coords
                    iii++; // skip next line
                    entities.levelExit.UpdateTransform();
                }
                else if (line == entities.strToEEDict["Shooting_Enemy"].saveSystemId)
                {
                    currentMassPlacedEntity = "Shooting_Enemy"; // map directly to the switch statement in Entites
                }
                else if (line == entities.strToEEDict["Walking_Enemy"].saveSystemId)
                {
                    currentMassPlacedEntity = "Walking_Enemy"; // map directly to the switch statement in Entites
                }
                else if (line == entities.strToEEDict["Checkpoint"].saveSystemId)
                {
                    currentMassPlacedEntity = "Checkpoint"; // map directly to the switch statement in Entites
                }
                // if this line is a valid vector & currently mass-placing something
                else if (ParseVec3(line, errVec) != errVec && currentMassPlacedEntity != null)
                {
                    var entity = entities.CreateAndAdd(currentMassPlacedEntity);
                    if (entity != null)
                    {
                        entity.transform.position = ParseVec3(line, errVec);
                        entity.UpdateTransform();
                    }
                }
            //}
            /*catch (Exception e)
            {
                Debug.Log($"exception :( {e.Message}");

            }*/

        }
        tilemapEditor.SetActiveLayer("Foreground"); // don't leave it at the damage layer, that's not user-friendly
    }

    public Vector3 ParseVec3(string str, Vector3 errorCase)
    {
        str.Trim('\n', '\r');
        string[] strFloats = str.Split(' ');
        Vector3 v = errorCase;

        try
        {
            float x = float.Parse(strFloats[0]);
            float y = float.Parse(strFloats[1]);
            float z = float.Parse(strFloats[2]);

            v = new Vector3(x, y, z);
        } catch { }

        return v;
    }

    public string IsATilemapLayerId(string line)
    {
        for (int jjj = 0; jjj < tilemapSerialization.tilemapRepresentations.Count; jjj++)
        {
            if (tilemapSerialization.tilemapRepresentations[jjj].saveSystemLayerId == line)
            {
                return line;
            }
        }
        return null;
    }
}
