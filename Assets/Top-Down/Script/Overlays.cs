using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overlays : MonoBehaviour
{
    private Canvas door;
    private Canvas bed;

    
    public void OpenDoor()
    {
        door.enabled = true;
    }
    public void CloseDoor()
    {
        door.enabled = false;
    }

    public void OpenBed()
    {
        door.enabled = true;
    }
    public void CloseBed()
    {
        bed.enabled = false;
    } 
}
