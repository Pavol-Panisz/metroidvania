using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;

public class SavingSystem : MonoBehaviour
{
    [SerializeField] private Entities entities;
    [SerializeField] private TilemapSerialization charmaps;

    string content = null;

    private string saveFileName = "save.metlvl";
    public static string saveFileCommentStr = "#";

    void Start()
    {
        
    }

    public void Export()
    {
        content = null; // you start completely anew

        content += $"{entities.player.saveSystemId}\n{Vec3ToStr(entities.player.transform.position)}\n\n";
        content += $"{entities.levelExit.saveSystemId}\n{Vec3ToStr(entities.levelExit.transform.position)}\n\n";

        void SaveList(List<EditorEntity> l)
        {
            if (l.Count == 0) return; // if no instances of this entity exist, skip it

            content += $"{l[0].saveSystemId}\n"; // "shooting_enemy"
            foreach (var ee in l) // each line below is a position vector of 1 instance
            {
                content += $"{Vec3ToStr(ee.transform.position)}\n";
            }
            content += "\n";
        }

        SaveList(entities.checkpoints);
        SaveList(entities.walkingEnemies);
        SaveList(entities.shootingEnemies);

        content += "\n";

        foreach (var charmap in charmaps.tilemapRepresentations)
        {
            content += charmap.saveSystemLayerId + "\n";
            content += charmap.GetCharmapString();
            content += "\n";
        }

        WebGLFileSaver.SaveFile(content, saveFileName);
    }


    private string Vec3ToStr(Vector3 v) // cause Vector3.ToString() is too unprecise
    {
        // "14.547 12.44770 0.001"
        return $"{v.x} {v.y} {v.z}";
    }
}
