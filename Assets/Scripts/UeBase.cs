using UnityEngine;
using System;
using static TransmissionParameterManager;
using UnityEngine.UIElements;
using System.Linq;



public class UeBase : GenericNodeBase
{
    public enum UeType
    {
        Standard,
        UeD2D
    }
    public UeType ueType = UeType.Standard;
    //public UeParameters ueParameters;

    //private bool losCollision = false;

    //public UeStatusDisplay statusDisplay;
    //public bool isSelect = false;

    //public EnbInfo TargetEnb;

    //public UeMovementControl prefabMovement;
    //public Vector3 prefabPosition;

    //public string mobilityInfo;

    //public string ulInfo = "";
    //public string dlInfo = "";

    //public GameObject ueObject;

    //public TransmissionParameter[] transmissionParameters;
    public UeBase[] ueBases;

    public UeExtraModuleManager extraModule;

    public float maxD2D_ConnectionDistance = 300;
    public int maxD2D_ConnectionNum = 4;

    void Start()
    {
        //txPowerBaseUl = UnityEngine.Random.Range( 25.0f, 27.0f );
        FindOtherUeBases();
    }


    void Update()
    {
        switch (ueType)
        {
            case UeType.Standard:
                
                transmissionParameters = new TransmissionParameter[1];
                transmissionParameters[0] = new TransmissionParameter();
                updateTransmissionParameterStandard(TargetEnb);
                break;


            case UeType.UeD2D:
                FindOtherUeBases();
                transmissionParameters = new TransmissionParameter[ueBases.Length];
                for (int i = 0; i < transmissionParameters.Length; i++)
                {
                    transmissionParameters[i] = new TransmissionParameter();
                }
                updateTransmissionParameterD2D(ueBases);
                break;
        }

        if (prefabMovement != null)
        {
            prefabPosition = prefabMovement.positionLocal;
            mobilityInfo = nodeId.ToString() + ": " + Math.Round(prefabPosition.x, 1) + " " + -Math.Round(prefabPosition.z, 1) + " " + Math.Round(prefabPosition.y, 1);
        }

        //losCollision = LOS_Ray.collision;
        //targetEnbId = TargetEnb.enbId;


        //txPowerUl = System.Math.Round(ueParameters.txPower+LOS_Ray.totalLossForwardInDB, 1);
        //txPowerDl = System.Math.Round(TargetEnb.txPower+LOS_Ray.totalLossReverseInDB, 1);

        
        //ulInfo = "txPower" + nodeId.ToString() + "->" + targetEnbId.ToString()  + ": " + txPowerUl.ToString();
        //dlInfo = "txPower" + targetEnbId.ToString() + "->" + nodeId.ToString() + ": " + txPowerDl.ToString();

        if (statusDisplay != null) isSelect = statusDisplay.status;


    }

    void updateTransmissionParameterD2D(UeBase[] ueBases)
    {
        for (int i = 0; i < transmissionParameters.Length; i++)
        {
            transmissionParameters[i].nodeA_Id = nodeId;
            transmissionParameters[i].positionA = nodeObject.transform.position;
            transmissionParameters[i].txPowerA = radioParameters.txPower;
            transmissionParameters[i].antennaGainA = radioParameters.antennaGain;

            transmissionParameters[i].nodeB_Id = ueBases[i].nodeId;
            transmissionParameters[i].positionB = ueBases[i].nodeObject.transform.position;
            transmissionParameters[i].txPowerB = ueBases[i].radioParameters.txPower;
            transmissionParameters[i].antennaGainB = ueBases[i].radioParameters.antennaGain;

            transmissionParameters[i].frequency = radioParameters.frequency;
            transmissionParameters[i].numBands = radioParameters.numBands;
            transmissionParameters[i].numLayers = radioParameters.numLayers;
            transmissionParameters[i].cableLoss = radioParameters.cableLoss;
            transmissionParameters[i].noiseFigure = radioParameters.noiseFigure;
            transmissionParameters[i].thermalNoise = radioParameters.thermalNoise;

            transmissionParameters[i].lastUpdateTime = Time.time;
            transmissionParameters[i].targetNode = ueBases[i];
        }
    }

    void updateTransmissionParameterStandard(EnbInfo enb)
    {
            transmissionParameters[0].nodeA_Id = nodeId;
            transmissionParameters[0].positionA = nodeObject.transform.position;
            transmissionParameters[0].txPowerA = radioParameters.txPower;
            transmissionParameters[0].antennaGainA = radioParameters.antennaGain;

            transmissionParameters[0].nodeB_Id = enb.enbId;
            transmissionParameters[0].positionB = enb.enbObject.transform.position;
            transmissionParameters[0].txPowerB = enb.txPower;
            transmissionParameters[0].antennaGainB = enb.antennaGain;

            transmissionParameters[0].frequency = radioParameters.frequency;
            transmissionParameters[0].numBands = radioParameters.numBands;
            transmissionParameters[0].numLayers = radioParameters.numLayers;
            transmissionParameters[0].cableLoss = radioParameters.cableLoss;
            transmissionParameters[0].noiseFigure = radioParameters.noiseFigure;
            transmissionParameters[0].thermalNoise = radioParameters.thermalNoise;

        transmissionParameters[0].lastUpdateTime = Time.time;
    }

    void FindOtherUeBases()
    {
        UeBase[] allUeBases = FindObjectsByType<UeBase>(FindObjectsSortMode.None);
        ueBases = allUeBases
            .Where(ue => ue != this && Vector3.Distance(ue.nodeObject.transform.position, this.nodeObject.transform.position) < maxD2D_ConnectionDistance)
            .Take(maxD2D_ConnectionNum)
            .ToArray();
    }

}
