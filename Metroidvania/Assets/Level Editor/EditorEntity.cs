using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityPlacement))]
public abstract class EditorEntity : MonoBehaviour
{
    public EntityPlacement entityPlacement;

    public string saveSystemId = "name_not_set"; // in the save file, identification of the entity type

    public abstract void OnEnterEditMode();
    public abstract void OnEnterPlayMode();
    public abstract void UpdateTransform();

    // You don't have to override this, if you wish.
    // ATM, only CheckpointEE uses this to reorganize each checkpoint's priority, after one of 'em
    // gets destroyed. Called by EntityTrashCan
    public virtual void OnDestroyThis()
    {
        
    }
}
