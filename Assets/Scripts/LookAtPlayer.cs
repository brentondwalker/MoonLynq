using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Transform playerTransform; 


    private void Update()
    {
        if (playerTransform != null)
        {

            transform.LookAt(playerTransform.position);
        }
    }
}