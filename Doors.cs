using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{

    Animator anim;
    bool doorOpen;

    void Start()
    {
        doorOpen = false;
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player" || col.gameObject.tag == "Monster")
        {
            doorOpen = true;
            DoorControl("OpenDoor");
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (doorOpen)
        {
            doorOpen = false;
            DoorControl("CloseDoor");
        }
    }

    void DoorControl(string direction)
    {
        anim.SetTrigger(direction);
    }
}