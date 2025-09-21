using UnityEngine;
using static TransmissionParameterManager;
public class TransmissionManager : MonoBehaviour
{
    public CQI_Base cqiBase;
    public int cqi;
    public double snr;
    public int targetId;
    public int GetCqi(TransmissionParameter parameter)
    {
        cqi = cqiBase.GetCqi(parameter);
        snr = cqiBase.meanSNR;
        return cqi;
    }

    public int GetTargetId(TransmissionParameter parameter)
    {
        targetId = parameter.nodeB_Id;
        return targetId;
    }

    public GenericNodeBase GetTargetNode(TransmissionParameter parameter) 
    {
        return parameter.targetNode;
    }
}
