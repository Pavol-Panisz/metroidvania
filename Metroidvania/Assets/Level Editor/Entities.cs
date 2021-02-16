using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
 * Keeps track of all entities (player, flag, level exit, shooting & walking enemy).
 * Determines what to do with them when entering edit / play mode.
 * Works in conjunction with the saving system
 */
public class Entities : MonoBehaviour
{
    [SerializeField] private EntityTrashCan entityTrashCan;
    [Space]
    [SerializeField] private LevelControl levelControl;
    public EditorEntity player;
    public EditorEntity levelExit;
    [Space]
    public List<EditorEntity> shootingEnemies;
    public List<EditorEntity> walkingEnemies;
    public List<EditorEntity> checkpoints;

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
        //foreach (var e in shootingEnemies) e.UpdateTransform();
    }

    private void OnSwitchedMode(LevelControl.Modes m)
    {
        EntityPlacement.beingHeld = null;
        if (m == LevelControl.Modes.Edit)
        {
            player.OnEnterEditMode();
            levelExit.OnEnterEditMode();
            foreach (var e in shootingEnemies) e.OnEnterEditMode();
            foreach (var e in walkingEnemies) e.OnEnterEditMode();
            foreach (var e in checkpoints) e.OnEnterEditMode();
        }
        else if (m == LevelControl.Modes.Play)
        {
            player.OnEnterPlayMode();
            levelExit.OnEnterPlayMode();
            foreach (var e in shootingEnemies) e.OnEnterPlayMode();
            foreach (var e in walkingEnemies) e.OnEnterPlayMode();
            foreach (var e in checkpoints) e.OnEnterPlayMode();
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
            case "Walking_Enemy":
                walkingEnemies.Add(instance);
                break;
            case "Checkpoint":
                checkpoints.Add(instance);
                break;
            default:
                Debug.LogError($"Unhandled strType {strType}");
                break;
        }

        instance.entityPlacement.StartBeingHeld();
        instance.entityPlacement.OnDropped += entityTrashCan.CheckDestroy;
        instance.OnEnterEditMode(); // turn it's brain off immediately
    }

    public void Destroy(EditorEntity ee)
    {
        Type type = ee.GetType();

        List<EditorEntity> targetList = null;

        if (type == typeof(ShootingEnemyEE))
        {
            targetList = shootingEnemies;
        } 
        else if (type == typeof(WalkingEE))
        {
            targetList = walkingEnemies;
        }
        else if (type == typeof(CheckpointEE))
        {
            targetList = checkpoints;
        }
        else if (type == typeof(LevelExitEE))
        {

        }
        else if (type == typeof(PlayerEE))
        {

        }

        // once done, search it in the target list and destroy it
        if (targetList != null)
        {
            for (int iii = 0; iii < targetList.Count; iii++)
            {
                if (targetList[iii].Equals(ee))
                {
                    targetList.RemoveAt(iii);
                    Destroy(ee.gameObject);
                }
            }
        }
    }
}
