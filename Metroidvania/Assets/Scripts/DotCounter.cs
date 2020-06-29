#region README
/* A counter for displaying things such as hearths
 * or breath-bubbles.
 */
#endregion README

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotCounter : MonoBehaviour
{

    private float val;

    protected void SetupCounter(float f) { val = f; UpdateCounter(); }
    
    protected void IncreaseBy(float f) => UpdateVal(f);
    protected void DecreaseBy(float f) => UpdateVal(-f);

    protected void UpdateVal(float f) {
        if (val < 0) {
            val = 0;
        }
        val += f;
        UpdateCounter();
    }

    protected void UpdateCounter() {

        int n = (int)val;

        foreach (Transform child in transform) {
            if (child.GetSiblingIndex() >= n) {
                child.gameObject.SetActive(false);
            }
            else {
                child.gameObject.SetActive(true);
            }
        }
    }

}
