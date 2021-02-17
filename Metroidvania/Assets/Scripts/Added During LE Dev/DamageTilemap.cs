using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTilemap : MonoBehaviour, IDamageDealer, ILaunchesAway
{
    [SerializeField] private float damage = 1f;

    Rigidbody2D rb;

    public float GetDamage() { return damage; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {

    }
    private void OnTriggerEnter2D(Collider2D coll)
    {

    }


}
