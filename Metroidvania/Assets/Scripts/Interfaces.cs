using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void interact();
}

public interface IDamageDealer 
{
    float GetDamage();
}

public interface ILaunchesAway {}