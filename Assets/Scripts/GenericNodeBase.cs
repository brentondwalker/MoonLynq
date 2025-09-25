using UnityEngine;
using System;
using static TransmissionParameterManager;
using UnityEngine.UIElements;
using System.Linq;



public class GenericNodeBase : MonoBehaviour
{


    public LOS_Ray LOS_Ray;
    public double txPowerUl = 0.0f;
    public double txPowerDl = 0.0f;
    public int nodeId = 0;

    public UeParameters radioParameters;
    private bool losCollision = false;
    public UeStatusDisplay statusDisplay;
    public bool isSelect = false;
    public GenericNodeBase TargetNode;
    public UeMovementControl prefabMovement;

    public Vector3 prefabPosition;

    public string mobilityInfo;

    public string ulInfo = "";
    public string dlInfo = "";

    public GameObject nodeObject;
    public TransmissionParameter[] transmissionParameters;

    public ChannelModelManager channelModelManager;
}
