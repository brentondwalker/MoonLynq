using UnityEngine;

public class VehicleMobility : MonoBehaviour
{

    public float moveSpeed = 10f;  
    public float turnSpeed = 120f;
    public float turnAngleThreshold = 1f;
    public float rotationSpeed = 10f;

    private float stopTimer = 0f;


    public void MoveToPoint(Vector3 moveTarget)
    {
        Vector3 direction = moveTarget - transform.position;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        transform.position = Vector3.MoveTowards(transform.position, moveTarget, moveSpeed * Time.deltaTime);
        
    }

    public void StopMovement()
    {
    }

}