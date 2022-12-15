using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    void Update ()
    {
        // does the player exist?
        if(PlayerMovement.me != null)
        {
            Vector3 targetPos = PlayerMovement.me.transform.position;
            targetPos.z = -6;

            transform.position = targetPos;
        }

    }


    // cubethon script
    /* public Transform player;
     public Vector3 offset;

     // Update is called once per frame
     void Update()
     {
         transform.position = player.position + offset;
     } */
}

