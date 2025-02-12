using Unity.VisualScripting;
using UnityEngine;

public class DopplerSpeed : MonoBehaviour
{
    private Vector3 lastPositionUe;
    private Vector3 lastPositionEnodeB;
    private float lastTime;

    public float computeDopplerSpeed(GameObject ue, GameObject eNB)
    {
        float currentTime = Time.time;

        if (lastTime == 0)
        {
            lastPositionUe = ue.transform.position;
            lastPositionEnodeB = eNB.transform.position;
            lastTime = currentTime;
            return 0f;
        }

        Vector3 deltaPositionUe = ue.transform.position - lastPositionUe;
        Vector3 deltaPositionEnodeB = eNB.transform.position - lastPositionEnodeB;

        Vector3 directionToEnodeB = (eNB.transform.position - ue.transform.position).normalized;

        float ueSpeedAlongDirection = Vector3.Dot(deltaPositionUe, directionToEnodeB) / (currentTime - lastTime);
        float enodeBSpeedAlongDirection = Vector3.Dot(deltaPositionEnodeB, directionToEnodeB) / (currentTime - lastTime);

        float dopplerSpeed = ueSpeedAlongDirection - enodeBSpeedAlongDirection;

        dopplerSpeed = dopplerSpeed / ScenarioScale.staticScale;

        lastPositionUe = ue.transform.position;
        lastPositionEnodeB = eNB.transform.position;
        lastTime = currentTime;

        return dopplerSpeed;
    }
}
