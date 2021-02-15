using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EntityTrashCan : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Entities entities;

    private bool isMousedOver = false;

    public void OnPointerEnter(PointerEventData data)
    {
        isMousedOver = true;
    }

    public void OnPointerExit(PointerEventData data)
    {
        isMousedOver = false;
    }

    public void CheckDestroy() // Assigned to every EntityPlacement's OnDropped delegate
    {


        if (isMousedOver && EntityPlacement.beingHeld != null)
        {
            EntityPlacement.beingHeld.entity.OnDestroyThis();
            entities.Destroy(EntityPlacement.beingHeld.entity);
        }
    }
}
