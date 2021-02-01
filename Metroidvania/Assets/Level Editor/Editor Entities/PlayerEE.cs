using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEE : MonoBehaviour, IEditorEntity
{

    public Rigidbody2D rb;
    private Vector3 ogPosition;
    private Vector3 ogLocalScale;
    private Quaternion ogRotation;

    [Tooltip("All components you wish to toggle when the mode switches")]
    [SerializeField] private MonoBehaviour[] toggleableComponents;
    [SerializeField] private HealthSystem healthSystem;

    private void Awake()
    {
        UpdateTransform();
    }

    private void Start()
    {
        
    }

    public void OnEnterEditMode()
    {
        rb.velocity = Vector3.zero;
        rb.simulated = false;
        rb.transform.position = ogPosition;
        rb.transform.rotation = ogRotation;
        rb.transform.localScale = ogLocalScale;

        healthSystem.FullHealth();
        foreach (var tg in toggleableComponents) tg.enabled = false;
    }
    public void OnEnterPlayMode()
    {
        rb.simulated = true;

        foreach (var tg in toggleableComponents) tg.enabled = true;
    }

    public Vector3 getOgPosition()
    {
        return ogPosition;
    }

    public void UpdateTransform()
    {
        ogPosition = rb.transform.position;
        ogLocalScale = rb.transform.localScale;
        ogRotation = rb.transform.rotation;
    }
}
