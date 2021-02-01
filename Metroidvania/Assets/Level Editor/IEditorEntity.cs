using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEditorEntity
{
    void OnEnterEditMode();
    void OnEnterPlayMode();

    void UpdateTransform(); // called when setting a new position for this entity
}
