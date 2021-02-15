using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour, IPointerEnterHandler, 
        IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent OnClick;
    public Sprite selectedSprite;
    public Sprite hoveredSprite;
    public Sprite notHoveredSprite;
    public Image selectionImage;

    [Space]
    [Tooltip("If false, even if you just start pressing the LMB, it'll count as a click." 
            + " If true, a click only registers if you start and also stop pressing the LMB inside.")]
    [SerializeField] private bool clickMustFinishInside = true;

    static CustomButton currentlySelected = null;

    public bool isSelected = false;

    private bool pressedDownHere = false;
    private bool isMouseInside = false;

    public void OnPointerEnter(PointerEventData data)
    {
        isMouseInside = true;
        if (!isSelected)
        {
            selectionImage.sprite = hoveredSprite;
        }
    }

    public void OnPointerExit(PointerEventData data)
    {
        isMouseInside = false;
        if (!isSelected)
        {
            selectionImage.sprite = notHoveredSprite;
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        pressedDownHere = true;
        if (!clickMustFinishInside)
        {
            Click();
            pressedDownHere = false;
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        if (clickMustFinishInside && pressedDownHere && isMouseInside)
        {
            Click();
        }
        pressedDownHere = false;
    }

    private void Click()
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
