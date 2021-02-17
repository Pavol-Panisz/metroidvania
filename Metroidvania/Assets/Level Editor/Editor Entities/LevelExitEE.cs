using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExitEE : EditorEntity
{
    [SerializeField] private SpriteRenderer editorSpriteRenderer;

    private void Awake()
    {
        saveSystemId = "level_exit";
    }
    private void OnEnable()
    {
        entityPlacement.OnDropped += UpdateTransform;
    }
    private void OnDisable()
    {
        entityPlacement.OnDropped -= UpdateTransform;
    }

    public override void OnEnterEditMode()
    {
        editorSpriteRenderer.enabled = true;
    }

    public override void OnEnterPlayMode()
    {
        editorSpriteRenderer.enabled = false;
    }

    public override void UpdateTransform()
    {

    }

}
