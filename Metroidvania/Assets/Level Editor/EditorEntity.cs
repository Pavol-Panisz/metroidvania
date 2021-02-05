using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EditorEntity : MonoBehaviour
{
    public EntityPlacement entityPlacement;

    public abstract void OnEnterEditMode();
    public abstract void OnEnterPlayMode();
    public abstract void UpdateTransform();
}
