using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class TilemapPlacementInhibitor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TilemapEditor tilemapEditor;

    public void OnPointerEnter(PointerEventData data)
    {
        tilemapEditor.isBeingInhibited = true;
    }

    public void OnPointerExit(PointerEventData data)
    {
        tilemapEditor.isBeingInhibited = false;
    }
}
