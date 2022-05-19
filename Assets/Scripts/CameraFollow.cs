using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script for making the camera follow a specific <see cref="GameObject"/>
/// </summary>
/// <remarks>
/// This is used instead of making the camera a child of the player is because
/// the way I flip an <see cref="Actor"/>'s sprite is by scaling the objects's
/// x dimension to -1 or 1, which means that if it's a child of the player then
/// when it turns around then it looks like it flipped the whole world instead
/// </remarks>
public class CameraFollow : MonoBehaviour
{
    private Transform target;

    void Start()
    {
        target = WorldMaster.GetPlayer().transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = target.position + Vector3.back * 10;
    }
}
