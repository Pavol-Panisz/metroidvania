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

    [Space]
    [Header("Cam movement when mouse close to screen bounds")]
    [SerializeField] [Range(0f, 0.3f)] private float startMoveZone = 0.1f;
    [SerializeField] [Range(0f, 0.3f)] private float stopMoveZoneOutsideScreen = 0.1f;
    [Tooltip("Cam move speed in units per second")]
    [SerializeField] private float camMoveSpeed = 1f;

    // Set by EditorActionInihibtor.
    // Editor scripts can do what they want with this, such as not placing a tile while being inhibited
    public static int inhibitingCount = 0; // How many inhibitors are currently inhibiting?
    public static bool isBeingInhibited 
    { 
        get
        {
            return (inhibitingCount > 0);
        }
        private set
        {
            isBeingInhibited = value;
        }
    }

    private Vector3 mousePosLastF;
    private Vector3 playerPosEditMode; // the camera goes to this pos whenever we re-enter edit mode

    public enum EditingActions { Tile_Placement, Entity_Placement, None}

    private Dictionary<string, EditingActions> StrToAction = new Dictionary<string, EditingActions>();
    public EditingActions CurrentAction { get; private set; }

    public Action<EditingActions> OnSwitchedEditingActionTo;

    private void Awake()
    {
        StrToAction.Add("Tile_Placement", EditingActions.Tile_Placement);
        StrToAction.Add("Entity_Placement", EditingActions.Entity_Placement);
        StrToAction.Add("None", EditingActions.None);
    }

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

        if (EntityPlacement.beingHeld != null) TryMoveCameraByMouse();
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
            // reset this, just in case. Debug, basically
            inhibitingCount = 0;

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

        if (Input.GetKey(KeyCode.Mouse2)) // if holding mouse wheel, camera moves
        {
            Camera.main.transform.position += mousePosLastF - mousePos;
        }
        // zoom
        Camera.main.orthographicSize -= Input.mouseScrollDelta.y * 0.3f;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 2f, 25f);

        mousePosLastF = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void SwitchToActionStr(string str) // called by UI buttons, since UnityEvents don't support enums
    {
        if (!StrToAction.ContainsKey(str))
        {
            Debug.LogError($"The key {str} is not present in the StrToAction dictionary!");
            return;
        }

        SwitchToAction(StrToAction[str]);
    }

    public void SwitchToAction(EditingActions a)
    {
        if (a != CurrentAction) // ony do stuff if we didn't change to the same action
        {
            CurrentAction = a;
            OnSwitchedEditingActionTo?.Invoke(CurrentAction);
            Debug.Log($"switched to action {a.ToString()}");
        }
    }

    private void TryMoveCameraByMouse()
    {
        Vector3 p = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        Vector3 dir = Vector3.zero;

        if (p.x < startMoveZone && p.x > -stopMoveZoneOutsideScreen) 
            dir = Vector3.left;
        else if (p.x > (1f - startMoveZone) && p.x < (1f + stopMoveZoneOutsideScreen)) 
            dir = Vector3.right;
        
        if (p.y < startMoveZone && p.y > -stopMoveZoneOutsideScreen) 
            dir += Vector3.down;
        else if (p.y > (1 - startMoveZone) && p.y < (1f + stopMoveZoneOutsideScreen)) 
            dir += Vector3.up;

        Camera.main.transform.position += dir * (camMoveSpeed * Time.deltaTime);
    }

    public void CenterOnPlayer()
    {
        Camera.main.transform.position = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y,
                Camera.main.transform.position.z
        );
    }
}
