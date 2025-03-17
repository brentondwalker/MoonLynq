using UnityEngine;

public class GlobalStatistikDangerousTurn : MonoBehaviour
{
    private static int dangerousTurnCount = 0;
    private static int rightTurnCount = 0;

    public int dangerousTurnCountDisplay = 0;
    public int rightTurnCountDisplay = 0;

    public static void DangerousTurnCount() {  dangerousTurnCount++;}

    public static void RightTurnCount() { rightTurnCount++; }

    public void Update()
    {
        dangerousTurnCountDisplay = dangerousTurnCount;
        rightTurnCountDisplay = rightTurnCount;
    }
}
