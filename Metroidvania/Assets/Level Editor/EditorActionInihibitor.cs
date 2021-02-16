using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditorActionInihibitor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData data)
    {
        CommonEditMode.inhibitingCount += 1; // another one inhibits
        Debug.Log(transform.name + " after += 1 : " + CommonEditMode.inhibitingCount);
    }

    public void OnPointerExit(PointerEventData data)
    {
        CommonEditMode.inhibitingCount -= 1;
        Debug.Log(transform.name + " after -= 1 : " + CommonEditMode.inhibitingCount);
    }
}
