using System;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


/*
 * Handles all stuff that's supposed to be active only during edit mode,
 * such as panning + zooming, the editor UI and more. Does not handle
 * the actual tile placement, entity placement or level saving system.
 */
public class CommonEditMode : MonoBehaviour
{
    [SerializeField] private LevelControl levelControl;
    [Tooltip("The gameobjects which parents all the UI visible only in edit mode")]
    [SerializeField] private GameObject editModeUIParent;
    [SerializeField] CinemachineBrain cinemachineBrain;
    [SerializeField] Transform playerTransform;

    private Vector3 mousePosLastF;
    private Vector3 playerPosEditMode; // the camera goes to this pos whenever we re-enter edit mode

    public enum EditingActions { Tile_Placement, Entity_Placement, None}
    public EditingActions CurrentAction { get; private set; }

    public Action<EditingActions> OnSwitchedEditingActionTo;

    private void OnEnable()
    {
        levelControl.OnSwitchedModeTo += OnSwitchedMode;
    }
    private void OnDisable()
    {
        levelControl.OnSwitchedModeTo -= OnSwitchedMode;
    }

    private void Start()
    {
        playerPosEditMode = new Vector3(
                playerTransform.position.x, 
                playerTransform.position.y, 
                Camera.main.transform.position.z);
    }

    private void Update()
    {
        if (levelControl.CurrentMode == LevelControl.Modes.Play) return;

        CameraMovement();
    }

    private void OnSwitchedMode(LevelControl.Modes m)
    {
        if (m == LevelControl.Modes.Play) 
        {
            editModeUIParent.SetActive(false);
            playerPosEditMode = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y,
                Camera.main.transform.position.z);

            // CommonPlayMode handles the camera in this case
        }
        else if (m == LevelControl.Modes.Edit) // not the most performant, but will do for now
        {
            editModeUIParent.SetActive(true); // not the most performant, but will do for now
            CurrentAction = EditingActions.None;
            OnSwitchedEditingActionTo?.Invoke(EditingActions.None);

            cinemachineBrain.enabled = false;
            Camera.main.transform.position = playerPosEditMode;
        }
    }

    private void CameraMovement()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKey(KeyCode.Mouse2)) // if holding mouse wheel, pan
        {
            Camera.main.transform.position += mousePosLastF - mousePos;
        }
        // zoom
        Camera.main.orthographicSize -= Input.mouseScrollDelta.y * 0.3f;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 2f, 25f);

        mousePosLastF = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void SwitchToAction(EditingActions a) // Called by some edit mode ui buttons
    {
        if (a != CurrentAction) // ony do stuff if we didn't change to the same action
        {
            CurrentAction = a;
            OnSwitchedEditingActionTo?.Invoke(CurrentAction);
            Debug.Log($"switched to action {a.ToString()}");
        }
    }
}
