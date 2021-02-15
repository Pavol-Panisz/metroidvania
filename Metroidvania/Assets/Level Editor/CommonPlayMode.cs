using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

/*
 * Handles all stuff that's supposed to be active during play mode only,
 * such as the health display or various entities' scripts.
 */
public class CommonPlayMode : MonoBehaviour
{
    [SerializeField] private LevelControl levelControl;
    [SerializeField] private RectTransform healthDisplayToggleable;

    // kinda bad design, cause we also have this in CommonEditMode. Maybe 
    //make a separate CameraControl.cs, where the camera's handled? 
    //(panning zooming..//
    [SerializeField] CinemachineBrain cinemachineBrain;
    [SerializeField] Transform playerTr;

    private void OnEnable()
    {
        levelControl.OnSwitchedModeTo += OnSwitchedMode;
    }
    private void OnDisable()
    {
        levelControl.OnSwitchedModeTo -= OnSwitchedMode;
    }

    private void OnSwitchedMode(LevelControl.Modes m)
    {
        if (m == LevelControl.Modes.Play)
        {
            healthDisplayToggleable.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(healthDisplayToggleable);

            Camera.main.transform.position = playerTr.position;
            cinemachineBrain.enabled = true;
        }
        else if (m == LevelControl.Modes.Edit)
        {
            healthDisplayToggleable.gameObject.SetActive(false);

            // CommonEditMode handles the camera in this case
        }
    }
}
