using UnityEngine;

public class GlobalStatistikDangerousTurn : MonoBehaviour
{
    private static int dangerousTurnCount = 0;
    private static int rightTurnCount = 0;
    private static int intersectionPassCount = 0;

    public int dangerousTurnCountDisplay = 0;
    public int rightTurnCountDisplay = 0;
    public int intersectionPassCountDisplay = 0;

    public static float elapsedTime = 0.0f;
    public float elapsedTimeDisplay = 0.0f;

    public static void DangerousTurnCount() {  dangerousTurnCount++;}

    public static void RightTurnCount() { rightTurnCount++; }

    public static void IntersectionPassCount() {  intersectionPassCount++; }

    public static int GetDangerousTurnCount() {  return dangerousTurnCount; }
    public static int GetRightTurnCount() { return rightTurnCount; }
    public static int GetIntersectionPassCount() { return intersectionPassCount; }
    public static float GetElapsedTime() { return elapsedTime; }    

    public static void clearCount()
    {
        intersectionPassCount = 0;
        rightTurnCount = 0;
        dangerousTurnCount = 0;
        elapsedTime = 0.0f;
    }


    public void Update()
    {
        elapsedTime += Time.deltaTime;
        elapsedTimeDisplay = elapsedTime;
        dangerousTurnCountDisplay = dangerousTurnCount;
        rightTurnCountDisplay = rightTurnCount;
        intersectionPassCountDisplay = intersectionPassCount;
    }
}
