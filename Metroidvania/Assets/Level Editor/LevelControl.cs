using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using UnityEngine.UI;

public class LevelControl : MonoBehaviour
{
    [SerializeField] private Entities entities;

    [Tooltip("The hearhs display. In edit mode, not visible")]
    [SerializeField] private GameObject hearthDisplay;

    [SerializeField] private TilemapEditor tilemapEditor;

    [Tooltip("The text that changes from 'Edit' to 'Play' on the button ")]
    [SerializeField] private TextMeshProUGUI toggleModeBtnText;

    [SerializeField] CinemachineBrain cinemachineBrain;

    [SerializeField] Button switchModesButton;

    public PlayerController playerController;
    public PlayerManager playerManager;
    private bool isDead = false;

    private float ogCamSize;

    public enum Modes { Play, Edit}
    [SerializeField] private Modes mode;

    private void Awake()
    {
        ogCamSize = Camera.main.orthographicSize;        
    }


    void Start()
    {
        SetMode(Modes.Edit);
    }



    private void OnEnable()
    {
        playerManager.OnDeath += SetDead;
        playerController.OnRegainedControl += SetNotDead;
    }
    private void OnDisable()
    {
        playerManager.OnDeath -= SetDead;
        playerController.OnRegainedControl -= SetNotDead;
    }
    private void SetDead() { switchModesButton.interactable = false; }
    private void SetNotDead() { switchModesButton.interactable = true; }



    public void SetMode(Modes mode)
    {
        this.mode = mode;
        UpdateEntities();
        tilemapEditor.SetMode(mode);

        if (this.mode == Modes.Play)
        {
            hearthDisplay.SetActive(true);
            toggleModeBtnText.text = "Edit";

            /*Camera.main.transform.position = new Vector3(
                    entities.player.rb.transform.position.x, 
                    entities.player.rb.transform.position.y, 
                    -10f);*/
            cinemachineBrain.enabled = true;
        } 
        else if (this.mode == Modes.Edit)
        {
            hearthDisplay.SetActive(false);
            toggleModeBtnText.text = "Play";

            cinemachineBrain.enabled = false;
            Camera.main.orthographicSize = ogCamSize;
            Camera.main.transform.position = entities.player.rb.transform.position;
            Camera.main.transform.position = new Vector3(
                    Camera.main.transform.position.x, 
                    Camera.main.transform.position.y, 
                    -10f);

        } else
        {
            Debug.LogError("Undefined mode");
        }
    }

    private void UpdateEntities()
    {
        if (mode == Modes.Edit)
        {
            entities.player.OnEnterEditMode();
        } else
        {
            entities.player.OnEnterPlayMode();
        }
    }


    // called by the toggle mode button
    public void ToggleMode()
    {

        if (mode == Modes.Edit) SetMode(Modes.Play);
        else SetMode(Modes.Edit);

        Debug.Log("toggled mode");
    }
}
