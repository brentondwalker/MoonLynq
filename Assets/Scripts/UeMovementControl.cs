using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UeMovementControl : MonoBehaviour
{
    public float moveSpeed = 5f;
    public UeStatusDisplay statusDisplay;
    public Transform cameraTransform;
    public Vector3 positionLocal;

    void Update()
    {
        positionLocal = transform.localPosition;
        Vector3 move = Vector3.zero;

        Vector3 cameraForward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        Vector3 cameraRight = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            move += cameraTransform.forward;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            move += -cameraTransform.forward;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            move += -cameraTransform.right;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            move += cameraTransform.right;
        }
        if (Input.GetKey(KeyCode.I))
        {
            move.z += moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.K))
        {
            move.z -= moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.J))
        {
            move.x -= moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.L))
        {
            move.x += moveSpeed * Time.deltaTime;
        }

        move.y = 0;

        if (statusDisplay.status)
        {
            transform.Translate(move.normalized * moveSpeed * Time.deltaTime, Space.World);
        }
    }
}