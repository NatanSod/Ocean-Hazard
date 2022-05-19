using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public WorldMaster.Doors door;

    // Start is called before the first frame update
    void Start()
    {
        WorldMaster.AddDoorToList(door, gameObject);
    }
}
