using UnityEngine;
using TMPro;

public class IntersectionStatistikDisplay : MonoBehaviour
{
    public TMP_Text showDangerousTurns;
    public TMP_Text showRightTurns;
    public TMP_Text showTotalPassThroughs;
    public TMP_Text showElapsedTime;

    void Update()
    {
        showDangerousTurns.text = GlobalStatistikDangerousTurn.GetDangerousTurnCount().ToString();
        showRightTurns.text = GlobalStatistikDangerousTurn.GetRightTurnCount().ToString();
        showTotalPassThroughs.text = GlobalStatistikDangerousTurn.GetIntersectionPassCount().ToString();
        showElapsedTime.text = GlobalStatistikDangerousTurn.GetElapsedTime().ToString("0.0");
    }

    public void ClearCount()
    {
        GlobalStatistikDangerousTurn.clearCount();
    }
}
