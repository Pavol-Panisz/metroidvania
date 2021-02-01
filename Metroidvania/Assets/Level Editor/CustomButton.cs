using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent OnClick;
    public Sprite selectedSprite;
    public Sprite hoveredSprite;
    public Sprite notHoveredSprite;
    public Image selectionImage;

    static CustomButton currentlySelected = null;


    public bool isSelected = false;

    public void OnPointerEnter(PointerEventData data)
    {
        if (!isSelected)
        {
            selectionImage.sprite = hoveredSprite;
        }
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (!isSelected)
        {
            selectionImage.sprite = notHoveredSprite;
        }
    }

    public void OnPointerClick(PointerEventData data)
    {
        DeselectCurrent();

        isSelected = true;
        currentlySelected = this;
        selectionImage.sprite = selectedSprite;

        OnClick?.Invoke();
    }

    static void DeselectCurrent()
    {
        if (currentlySelected != null)
        {
            currentlySelected.isSelected = false;
            currentlySelected.selectionImage.sprite = currentlySelected.notHoveredSprite;
            currentlySelected = null;
        }
    }
}
