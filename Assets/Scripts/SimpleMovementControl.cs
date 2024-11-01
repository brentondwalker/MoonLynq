using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovementControl : MonoBehaviour
{
    public float moveSpeed = 5f;  
    public float rotationSpeed = 45f;  

    void Update()
    {

        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            move += Vector3.forward;  
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            move += Vector3.back; 
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            move += Vector3.left; 
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            move += Vector3.right; 
        }

        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
