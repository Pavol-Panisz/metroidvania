using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

// set to execute after tilemapeditor, so that in Update(), the indicatorPos doesn't
// lag behind 1 frame

public abstract class TilemapTool : MonoBehaviour, IPointerEnterHandler,
        IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] protected TilemapEditor tilemapEditor;

    [Tooltip("The sprite renderer whose image's alhpa will be manipulated to achieve a highlight effect")]
    [SerializeField] private Image selectionImage;
    [SerializeField] private Color hoveredColor = Color.white;
    [SerializeField] private Color selectedColor = Color.green;

    private static TilemapTool _currentlySelected = null;
    public static TilemapTool currentlySelected 
    { get { return _currentlySelected; }
        set {
            if (value == null)
            {
                _currentlySelected.OnDeselected();
                _currentlySelected = null;
            }
            else
            {
                if (_currentlySelected != null) _currentlySelected.OnDeselected();
                _currentlySelected = value;
                _currentlySelected.OnSelected();
            }
        }
    } 
    protected static TilemapTool lastDeselected = null;

    protected Vector3Int indicatorPos;

    [SerializeField] private UnityEvent OnClick;

    public void OnEnable()
    {
        tilemapEditor.OnUpdatedIndicatorPos += UpdateIndicatorPos;
        tilemapEditor.OnSwitchedLayerTo += OnSwitchedLayerTo;
    }
    public void OnDisable()
    {
        tilemapEditor.OnUpdatedIndicatorPos -= UpdateIndicatorPos;
        tilemapEditor.OnSwitchedLayerTo -= OnSwitchedLayerTo;
    }
    private void UpdateIndicatorPos(Vector3Int p)
    {
        indicatorPos = p;
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (currentlySelected != this) selectionImage.color = Color.clear;
        OnMouseExit();
    }
    protected abstract void OnMouseExit();

    public void OnPointerEnter(PointerEventData data)
    {
        if (currentlySelected != this) selectionImage.color = hoveredColor;
        OnClick?.Invoke();
        OnMouseEnter();
    }
    protected abstract void OnMouseEnter();

    public void OnPointerDown(PointerEventData data)
    {
        if (currentlySelected != null && currentlySelected != null) 
                currentlySelected.selectionImage.color = Color.clear;
        
        currentlySelected = this;
        selectionImage.color = selectedColor;
        OnMouseDown();
    }
    protected abstract void OnMouseDown();

    public void OnPointerUp(PointerEventData data)
    {
        OnMouseUp();
    }
    protected abstract void OnMouseUp();

    public static void DeselectCurrent() // called when clicking on an entity
    {
        if (currentlySelected)
        {
            currentlySelected.selectionImage.color = Color.clear;
            lastDeselected = currentlySelected;
            currentlySelected = null;
        }
    }

    public static void SelectLastDeselected() // called when clicking on a tilemap-editing related thing
    {
        if (lastDeselected != null)
        {
            currentlySelected = lastDeselected;
            currentlySelected.selectionImage.color = currentlySelected.selectedColor;
            lastDeselected = null;
        }
    }

    protected virtual void OnSelected() { } // cause they can get selected/deselected not only via clicking

    protected virtual void OnDeselected() { } // cause they can get selected/deselected not only via clicking

    protected virtual void OnSwitchedLayerTo(TilemapEditor.Layers layer) { }
}
