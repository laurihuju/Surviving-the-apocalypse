using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour, EntityDeath
{
    private bool dead;

    public void OnDeath()
    {
        if (dead)
            return;
        Menu.MenuSystem.GetInstance().ShowPage(1);
        dead = true;
    }
}
