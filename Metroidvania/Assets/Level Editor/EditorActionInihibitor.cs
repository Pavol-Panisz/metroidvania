using UnityEngine;
using UnityEngine.EventSystems;

public class EditorActionInihibitor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData data)
    {
        CommonEditMode.inhibitingCount += 1; // another one inhibits
        Debug.Log(CommonEditMode.inhibitingCount);
    }

    public void OnPointerExit(PointerEventData data)
    {
        CommonEditMode.inhibitingCount -= 1;
        Debug.Log(CommonEditMode.inhibitingCount);
    }
}
