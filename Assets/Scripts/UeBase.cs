using UnityEngine;
using System;
using static TransmissionParameterManager;
using UnityEngine.UIElements;
using System.Linq;



public class UeBase : MonoBehaviour
{

    public LOS_Ray LOS_Ray;
    public double txPowerUl = 0.0f;
    public double txPowerDl = 0.0f;
    public int ueId = 0;


    public enum UeType
    {
        Standard,
        UeD2D
    }
    public UeType ueType = UeType.Standard;
    public UeParameters ueParameters;

    private bool losCollision = false;

    public UeStatusDisplay statusDisplay;
    public bool isSelect = false;

    public EnbInfo TargetEnb;

    public UeMovementControl prefabMovement;
    public Vector3 prefabPosition;

    public string mobilityInfo;

    public string ulInfo = "";
    public string dlInfo = "";

    public GameObject ueObject;

    public TransmissionParameter[] transmissionParameters;
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
            mobilityInfo = ueId.ToString() + ": " + Math.Round(prefabPosition.x, 1) + " " + -Math.Round(prefabPosition.z, 1) + " " + Math.Round(prefabPosition.y, 1);
        }

        //losCollision = LOS_Ray.collision;
        //targetEnbId = TargetEnb.enbId;


        //txPowerUl = System.Math.Round(ueParameters.txPower+LOS_Ray.totalLossForwardInDB, 1);
        //txPowerDl = System.Math.Round(TargetEnb.txPower+LOS_Ray.totalLossReverseInDB, 1);

        
        //ulInfo = "txPower" + ueId.ToString() + "->" + targetEnbId.ToString()  + ": " + txPowerUl.ToString();
        //dlInfo = "txPower" + targetEnbId.ToString() + "->" + ueId.ToString() + ": " + txPowerDl.ToString();

        if (statusDisplay != null) isSelect = statusDisplay.status;


    }

    void updateTransmissionParameterD2D(UeBase[] ueBases)
    {
        for (int i = 0; i < transmissionParameters.Length; i++)
        {
            transmissionParameters[i].nodeA_Id = ueId;
            transmissionParameters[i].positionA = ueObject.transform.position;
            transmissionParameters[i].txPowerA = ueParameters.txPower;
            transmissionParameters[i].antennaGainA = ueParameters.antennaGain;

            transmissionParameters[i].nodeB_Id = ueBases[i].ueId;
            transmissionParameters[i].positionB = ueBases[i].ueObject.transform.position;
            transmissionParameters[i].txPowerB = ueBases[i].ueParameters.txPower;
            transmissionParameters[i].antennaGainB = ueBases[i].ueParameters.antennaGain;

            transmissionParameters[i].frequency = ueParameters.frequency;
            transmissionParameters[i].numBands = ueParameters.numBands;
            transmissionParameters[i].numLayers = ueParameters.numLayers;
            transmissionParameters[i].cableLoss = ueParameters.cableLoss;
            transmissionParameters[i].noiseFigure = ueParameters.noiseFigure;
            transmissionParameters[i].thermalNoise = ueParameters.thermalNoise;

            transmissionParameters[i].lastUpdateTime = Time.time;
            transmissionParameters[i].targetUeBase = ueBases[i];
        }
    }

    void updateTransmissionParameterStandard(EnbInfo enb)
    {
            transmissionParameters[0].nodeA_Id = ueId;
            transmissionParameters[0].positionA = ueObject.transform.position;
            transmissionParameters[0].txPowerA = ueParameters.txPower;
            transmissionParameters[0].antennaGainA = ueParameters.antennaGain;

            transmissionParameters[0].nodeB_Id = enb.enbId;
            transmissionParameters[0].positionB = enb.enbObject.transform.position;
            transmissionParameters[0].txPowerB = enb.txPower;
            transmissionParameters[0].antennaGainB = enb.antennaGain;

            transmissionParameters[0].frequency = ueParameters.frequency;
            transmissionParameters[0].numBands = ueParameters.numBands;
            transmissionParameters[0].numLayers = ueParameters.numLayers;
            transmissionParameters[0].cableLoss = ueParameters.cableLoss;
            transmissionParameters[0].noiseFigure = ueParameters.noiseFigure;
            transmissionParameters[0].thermalNoise = ueParameters.thermalNoise;

        transmissionParameters[0].lastUpdateTime = Time.time;
    }

    void FindOtherUeBases()
    {
        UeBase[] allUeBases = FindObjectsByType<UeBase>(FindObjectsSortMode.None);
        ueBases = allUeBases
            .Where(ue => ue != this && Vector3.Distance(ue.ueObject.transform.position, this.ueObject.transform.position) < maxD2D_ConnectionDistance)
            .Take(maxD2D_ConnectionNum)
            .ToArray();
    }

}
