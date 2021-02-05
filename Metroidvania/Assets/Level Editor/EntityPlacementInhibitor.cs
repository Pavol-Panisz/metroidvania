using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EntityPlacementInhibitor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerExit(PointerEventData data)
    {
        EntityPlacement.isInhibited = false;
    }

    public void OnPointerEnter(PointerEventData data)
    {
        EntityPlacement.isInhibited = true;
    }
}
