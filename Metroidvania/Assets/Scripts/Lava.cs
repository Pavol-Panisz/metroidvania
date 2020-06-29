using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour, IDamageDealer
{
    [SerializeField] private float damage;

    public float GetDamage() {
        return damage;
    }


}
