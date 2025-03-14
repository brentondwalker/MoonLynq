using UnityEngine;

public class VehicleCollisionAvoidance : MonoBehaviour
{
    private bool isCollidingWithVehicle = false;
    private float collisionTimer = 0f;
    private float ignoreCollisionTimer = 0f;
    public float maxCollisionWaitTime = 25f;
    private bool isIgnoringCollision = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("vehicle") && !isIgnoringCollision)
        {
            isCollidingWithVehicle = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("vehicle"))
        {
            isCollidingWithVehicle = false;
            collisionTimer = 0f; // 退出时重置计时器
        }
    }

    void Update()
    {
        if (isIgnoringCollision)
        {
            ignoreCollisionTimer += Time.deltaTime;
            if (ignoreCollisionTimer >= 5f)
            {
                isIgnoringCollision = false;
                ignoreCollisionTimer = 0f;
            }
        }
        else if (isCollidingWithVehicle)
        {
            collisionTimer += Time.deltaTime;
            if (collisionTimer >= maxCollisionWaitTime)
            {
                isIgnoringCollision = true;
                isCollidingWithVehicle = false; 
                collisionTimer = 0f; 
            }
        }
    }

    public bool CheckVehicleCollision()
    {
        return !isIgnoringCollision && isCollidingWithVehicle;
    }
}