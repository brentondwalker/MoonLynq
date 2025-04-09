using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UeMovementControl : MonoBehaviour
{
    public float moveSpeed = 5f;
    public UeStatusDisplay statusDisplay;
    public Transform cameraTransform;
    public Vector3 positionLocal;

    void FixedUpdate()
    {
        positionLocal = transform.localPosition;
        Vector3 move = Vector3.zero;

        Vector3 cameraForward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        Vector3 cameraRight = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            move += cameraForward;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            move -= cameraForward;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            move -= cameraRight;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            move += cameraRight;
        }
        if (Input.GetKey(KeyCode.I))
        {
            move.z += 1;
        }
        if (Input.GetKey(KeyCode.K))
        {
            move.z -= 1;
        }
        if (Input.GetKey(KeyCode.J))
        {
            move.x -= 1;
        }
        if (Input.GetKey(KeyCode.L))
        {
            move.x += 1;
        }

        move.y = 0;

        if (move != Vector3.zero)
        {
            move = move.normalized; 
        }

        if (statusDisplay.status)
        {
            transform.Translate(move * moveSpeed * Time.fixedDeltaTime, Space.World);
        }
    }
}