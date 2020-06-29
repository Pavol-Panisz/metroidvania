using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    private float health=0;
    private bool isInvincible=false;
    private bool isDead=false;

    public delegate void eventDelegate();
    public event eventDelegate OnDeath;
    public delegate void floatArgDelegate(float f);
    public event floatArgDelegate OnHealed;
    public event floatArgDelegate OnTakenDamage;
    public event floatArgDelegate OnSetupHealth;

    private void Start() {
        health = maxHealth;
        OnSetupHealth?.Invoke(health);
    }

    public void Heal(float f) {

        OnHealed?.Invoke(f);
        health += f;
        if (health > maxHealth) {
            health = f;
        }
        if (health > 0) {
            isDead = false;
        }
    }

    public void Damage(float f) {
        OnTakenDamage?.Invoke(f);
        health -= f;

        if (health <= 0f) {
            health = 0f; //minimum health
            if (!isDead) { //call OnDeath when the player dies
                OnDeath?.Invoke();
                isDead = true;
                }
        }
    }

    public float GetHealth() {
        return health;
    }

    public void SetInvincible(bool b) {
        isInvincible = b;
    }

    public void FullHealth() {
        Heal(maxHealth);
    }

}
