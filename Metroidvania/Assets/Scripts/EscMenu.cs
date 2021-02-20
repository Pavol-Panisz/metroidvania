using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscMenu : MonoBehaviour
{
    public bool isOn = false;
    public GameObject toggleable;

    private void Start()
    {
        toggleable.SetActive(isOn);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        isOn = !isOn;
        toggleable.SetActive(isOn);
    }

}
