using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SavingSystem : MonoBehaviour
{
    [SerializeField] private Entities entities;

    // debug
    public TextMeshProUGUI fileLoadResultText;

    string content = null;

    private string saveFileName = "save.metlev";

    private void Start()
    {
        
    }

    // debug - called when the file dialogue's file gets read
    public void DisplayFileContent(string str)
    {
        fileLoadResultText.text = str;
    }

    public void Export()
    {
        content = null; // you start completely anew

        content += $"{entities.player.saveSystemId} {Vec3ToStr(entities.player.transform.position)}\n";
        content += $"{entities.levelExit.saveSystemId} {Vec3ToStr(entities.levelExit.transform.position)}\n";

        void SaveList(List<EditorEntity> l)
        {
            if (l.Count == 0) return; // if no instances of this entity exist, skip it

            content += $"{l[0].saveSystemId}\n";
            foreach (var ee in l)
            {
                content += $"{Vec3ToStr(ee.transform.position)}\n";
            }
        }

        SaveList(entities.checkpoints);
        SaveList(entities.walkingEnemies);
        SaveList(entities.shootingEnemies);

        WebGLFileSaver.SaveFile(content, saveFileName);
    }

    private string Vec3ToStr(Vector3 v) // cause Vector3.ToString() is too unprecise
    {
        // "14.547 12.44770 0.001"
        return $"{v.x} {v.y} {v.z}";
    }
}
