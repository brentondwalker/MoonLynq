using Unity.VisualScripting;
using UnityEngine;
using static TransmissionParameterManager;

public class DopplerSpeed : MonoBehaviour
{
    private Vector3 lastPositionUe;
    private Vector3 lastPositionEnodeB;
    private float lastTime;

    public float computeDopplerSpeed(TransmissionParameter transmissionParameter)
    {
        float currentTime = Time.time;

        if (lastTime == 0)
        {
            lastPositionUe = transmissionParameter.positionA;
            lastPositionEnodeB = transmissionParameter.positionB;
            lastTime = currentTime;
            return 0f;
        }

        Vector3 deltaPositionUe = transmissionParameter.positionA - lastPositionUe;
        Vector3 deltaPositionEnodeB = transmissionParameter.positionB - lastPositionEnodeB;

        Vector3 directionToEnodeB = (transmissionParameter.positionB - transmissionParameter.positionA).normalized;

        float ueSpeedAlongDirection = Vector3.Dot(deltaPositionUe, directionToEnodeB) / (currentTime - lastTime);
        float enodeBSpeedAlongDirection = Vector3.Dot(deltaPositionEnodeB, directionToEnodeB) / (currentTime - lastTime);

        float dopplerSpeed = ueSpeedAlongDirection - enodeBSpeedAlongDirection;

        dopplerSpeed = dopplerSpeed / ScenarioScale.staticScale;

        lastPositionUe = transmissionParameter.positionA;
        lastPositionEnodeB = transmissionParameter.positionB;
        lastTime = currentTime;

        return dopplerSpeed;
    }
}
