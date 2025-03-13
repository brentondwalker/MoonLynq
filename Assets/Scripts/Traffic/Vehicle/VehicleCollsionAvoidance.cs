using UnityEngine;

public class VehicleCollisionAvoidance : MonoBehaviour
{
    private bool isCollidingWithVehicle = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("vehicle"))
        {
            isCollidingWithVehicle = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("vehicle"))
        {
            isCollidingWithVehicle = false;
        }
    }

    public bool CheckVehicleCollision()
    {
        return isCollidingWithVehicle;
    }
}