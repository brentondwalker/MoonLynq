using UnityEngine;
using static TransmissionParameterManager;
public class TransmissionManager : MonoBehaviour
{
    public CQI_Base cqiBase;
    public int cqi;
    public int GetCqi(TransmissionParameter parameter)
    {
        cqi = cqiBase.GetCqi(parameter);
        return cqi;
    }
}
