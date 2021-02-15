using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CheckpointEE : EditorEntity
{
    private static List<CheckpointEE> checkpoints = new List<CheckpointEE>();

    [SerializeField] private CheckpointLE checkpointLE;
    public TextMeshProUGUI priorityDisplayText;
    public RectTransform priorityDisplay;

    [SerializeField] private Transform priorityDisplayWorldTransform;
    [SerializeField] private int priorityTextSize = 40;

    [SerializeField] private Behaviour[] enabledOnlyInPlayModeAlive;
    [SerializeField] private Behaviour[] enabledOnlyInEditMode;

    private void Start() // called only once, when a new instance gets created
    {

        checkpointLE.priority = checkpoints.Count; // 0th el - 0, 1st el - 1 ..
        priorityDisplayText.text = checkpoints.Count.ToString();
        checkpoints.Add(this);

        priorityDisplay.SetParent(GameObject.FindGameObjectWithTag("Non_UI_Bound").transform);
    }

    private void OnEnable()
    {
        entityPlacement.OnDropped += UpdateTransform;
    }
    private void OnDisable()
    {
        entityPlacement.OnDropped -= UpdateTransform;
    }

    public override void OnEnterEditMode()
    {
        foreach (var c in enabledOnlyInPlayModeAlive) c.enabled = false;
        foreach (var c in enabledOnlyInEditMode) c.enabled = true;

        checkpointLE.SetHasBeenTouched(false);
    }

    public override void OnEnterPlayMode()
    {
        CheckpointLE.stCurrentPriority = -1;

        foreach (var c in enabledOnlyInPlayModeAlive) c.enabled = true;
        foreach (var c in enabledOnlyInEditMode) c.enabled = false;

        checkpointLE.SetHasBeenTouched(false);

    }

    public void LateUpdate()
    {
        // this transform's world coords get transformed into screen space coords
        priorityDisplay.position = Camera.main.WorldToScreenPoint(priorityDisplayWorldTransform.position);


        // 1 / 4 refers to the size being true for cam size == 4
        priorityDisplayText.fontSize = priorityTextSize * (4f / Camera.main.orthographicSize); 
    }

    public override void UpdateTransform()
    {
        checkpointLE.respawnPoint = (Vector2)transform.position + Vector2.up;
    }

    // Called in edit mode when you place an instance of this in the trash
    public override void OnDestroyThis()
    {
        bool found = false;

        // Since the priorityDisplay is parented to Non UI Bound Parent,
        // we must explicitly destroy it.
        Destroy(priorityDisplay.gameObject);

        // find this instance in the static list
        for (int iii=0; iii < checkpoints.Count; iii++)
        {
            if (checkpoints[iii].Equals(this))
            {
                checkpoints.RemoveAt(iii);
                found = true;
            }
            if (found && checkpoints.Count != 0) // true for all checkpointLEs after the one which got removed
            {
                // just update its priority
                checkpoints[iii].checkpointLE.priority -= 1;
                checkpoints[iii].priorityDisplayText.text = checkpoints[iii].checkpointLE.priority.ToString();
            }
        }
    }

}
