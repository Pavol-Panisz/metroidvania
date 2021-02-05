using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/*
 * Handles the very basics of the entire level, such as the mode toggle button
 * and what mode we're currently in
 */
public class LevelControl : MonoBehaviour
{
    public enum Modes { Edit, Play}
    public Modes CurrentMode { get; private set; }
    public Action<Modes> OnSwitchedModeTo;

    [SerializeField] PlayerController playerController;
    [SerializeField] PlayerManager playerManager;
    private bool isDead = false;

    [Header("The button that toggles the mode")]
    [SerializeField] private Button modeToggleButton;
    [SerializeField] private TextMeshProUGUI modeToggleButtonText;

    private void Awake()
    {
        CurrentMode = Modes.Play;       
    }

    public void Start()
    {
        ToggleMode(); // you start in edit mode + subscribers of OnSwitchedModeTo can react
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
    private void SetDead() { modeToggleButton.interactable = false; }
    private void SetNotDead() { modeToggleButton.interactable = true; }

    public void ToggleMode()
    {
        if (CurrentMode == Modes.Edit) {
            CurrentMode = Modes.Play;
            modeToggleButtonText.text = "Edit";
        } else if (CurrentMode == Modes.Play)
        {
            CurrentMode = Modes.Edit;
            modeToggleButtonText.text = "Play";
        }
        OnSwitchedModeTo?.Invoke(CurrentMode);
    }
}
