using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Keeps track of all entities (player, flag, level exit, shooting & walking enemy).
 * Determines what to do with them when entering edit / play mode.
 * Works in conjunction with the saving system
 */
public class Entities : MonoBehaviour
{
    [SerializeField] private LevelControl levelControl;
    [SerializeField] private List<EditorEntity> shootingEnemies;

    [System.Serializable]
    public struct EntityInstance { 
    
        [Tooltip("Shooting_Enemy, Walking_Enemy, Checkpoint")]
        public string type;
        public EditorEntity prefab;

        [Tooltip("The gameobject, to which all instances of this prefab will be parented")]
        public Transform entityCollectionParent;
    }
    [Tooltip("A dictionary holding all instantiable entites")]
    [SerializeField] private List<EntityInstance> instantiablesDict;

    private void OnEnable()
    {
        levelControl.OnSwitchedModeTo += OnSwitchedMode;
    }
    private void OnDisable()
    {
        levelControl.OnSwitchedModeTo -= OnSwitchedMode;
    }

    // DEBUG - this should be called in when drag n dropping an entity
    private void Start()
    {
        foreach (var e in shootingEnemies) e.UpdateTransform();
    }

    private void OnSwitchedMode(LevelControl.Modes m)
    {
        EntityPlacement.beingHeld = null;
        if (m == LevelControl.Modes.Edit)
        {
            foreach (var e in shootingEnemies) e.OnEnterEditMode();
        }
        else if (m == LevelControl.Modes.Play)
        {
            foreach (var e in shootingEnemies) e.OnEnterPlayMode();
        }
    }

    public void CreateNewInstance(string strType)
    {
        EditorEntity instance = null;

        foreach (EntityInstance instantiable in instantiablesDict) {
            if (instantiable.type == strType)
            {
                instance = Instantiate(instantiable.prefab) as EditorEntity;
                instance.transform.SetParent(instantiable.entityCollectionParent);
            }
        }
        if (instance == null) {
            Debug.LogError($"Didn't find type '{strType}' in the instantiablesDict");
            return;
        }

        switch (strType)
        {
            case "Shooting_Enemy":
                shootingEnemies.Add(instance);
                break;
            default:
                Debug.LogError($"Unhandled strType {strType}");
                break;
        }

        instance.entityPlacement.StartBeingHeld();
        instance.OnEnterEditMode(); // turn it's brain off immediately
    }
}
