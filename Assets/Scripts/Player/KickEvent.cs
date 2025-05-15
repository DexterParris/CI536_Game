using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickEvent : MonoBehaviour
{
    public PlayerMovement playerMovement;

    void OnKick()
    {
        playerMovement.KickDamage();
    }
}
