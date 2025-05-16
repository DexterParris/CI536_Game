using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorScript : MonoBehaviour
{
    private Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }



    public void OpenDoor()
    {
        anim.SetBool("IsOpen", true);
    }
    public void CloseDoor()
    {
        anim.SetBool("IsOpen", false);
    }
}
