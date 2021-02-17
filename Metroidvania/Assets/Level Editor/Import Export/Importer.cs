using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Importer : MonoBehaviour
{
    [Tooltip("In case the parsing of a vector 3 goes wrong, default to this value")]
    [SerializeField] private Vector3 errVec = Vector3.one;

    [SerializeField] private TilemapSerialization charmaps;
    [SerializeField] private TilemapEditor tilemapEditor;
    [SerializeField] private Entities entities;

    private string currentMassPlacedEntity = null;

    public void Import(string content)
    {
        // destroy all entites except player and level_exit
        foreach (var e in entities.shootingEnemies) entities.Destroy(e);
        foreach (var e in entities.walkingEnemies) entities.Destroy(e);
        foreach (var e in entities.checkpoints) entities.Destroy(e);

        string[] lines = content.Split('\n');
        for (int iii=0; iii < lines.Length; iii++)
        {
            string line = lines[iii].Trim('\n');

            if (line == entities.player.saveSystemId) {
                entities.player.transform.position = ParseVec3(lines[iii + 1], errVec); // next line's the coords
                iii++; // skip next line
                entities.player.UpdateTransform();
            }
            else if (line == entities.levelExit.saveSystemId)
            {
                entities.levelExit.transform.position = ParseVec3(lines[iii + 1], errVec); // next line's the coords
                iii++; // skip next line
                entities.levelExit.UpdateTransform();
            }
            else if (line == entities.strToEEDict["Shooting_Enemy"].saveSystemId) {
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
            else if (ParseVec3(line, errVec) != errVec && currentMassPlacedEntity != null) { 
                var entity = entities.CreateAndAdd(currentMassPlacedEntity); 
                if (entity != null) {
                    entity.transform.position = ParseVec3(line, errVec);
                    entity.UpdateTransform();
                }

                // LEFT OFF
            }

        }
    }

    public Vector3 ParseVec3(string str, Vector3 errorCase)
    {
        str.Trim('\n');
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


}
