using UnityEngine;
using System;
using static TransmissionParameterManager;
using UnityEngine.UIElements;
using System.Linq;



public class Ieee802154Base : GenericNodeBase
{



    public enum NodeType
    {
        FFD
    }
    public NodeType nodeType = NodeType.FFD;

    // there is nothing too UE-specific here anyway
    // just the numbands numlayers stuff
    //public UeParameters radioParameters;

    //private bool losCollision = false;

    //public UeStatusDisplay statusDisplay;
    //public bool isSelect = false;

    //public Ieee802154Base TargetNode;

    //public string ulInfo = "";
    //public string dlInfo = "";

    //public GameObject ieee802154Object;

    //public TransmissionParameter[] transmissionParameters;
    public Ieee802154Base[] ieee802154Bases;

    public UeExtraModuleManager extraModule;

    public float maxD2D_ConnectionDistance = 300;
    public int maxD2D_ConnectionNum = 4;

    void Start()
    {
        //txPowerBaseUl = UnityEngine.Random.Range( 25.0f, 27.0f );
        FindOther802154Bases();
    }


    void Update()
    {
        switch (nodeType)
        {
            case NodeType.FFD:
                FindOther802154Bases();
                transmissionParameters = new TransmissionParameter[ieee802154Bases.Length];
                for (int i = 0; i < transmissionParameters.Length; i++)
                {
                    transmissionParameters[i] = new TransmissionParameter();
                }
                updateTransmissionParameterD2D(ieee802154Bases);
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

        
        //ulInfo = "txPower" + ueId.ToString() + "->" + targetEnbId.ToString()  + ": " + txPowerUl.ToString();
        //dlInfo = "txPower" + targetEnbId.ToString() + "->" + ueId.ToString() + ": " + txPowerDl.ToString();

        if (statusDisplay != null) isSelect = statusDisplay.status;


    }

    void updateTransmissionParameterD2D(Ieee802154Base[] ieee802154Bases)
    {
        for (int i = 0; i < transmissionParameters.Length; i++)
        {
            transmissionParameters[i].nodeA_Id = nodeId;
            transmissionParameters[i].positionA = nodeObject.transform.position;
            transmissionParameters[i].txPowerA = radioParameters.txPower;
            transmissionParameters[i].antennaGainA = radioParameters.antennaGain;

            transmissionParameters[i].nodeB_Id = ieee802154Bases[i].nodeId;
            transmissionParameters[i].positionB = ieee802154Bases[i].nodeObject.transform.position;
            transmissionParameters[i].txPowerB = ieee802154Bases[i].radioParameters.txPower;
            transmissionParameters[i].antennaGainB = ieee802154Bases[i].radioParameters.antennaGain;

            transmissionParameters[i].frequency = radioParameters.frequency;
            transmissionParameters[i].numBands = radioParameters.numBands;
            transmissionParameters[i].numLayers = radioParameters.numLayers;
            transmissionParameters[i].cableLoss = radioParameters.cableLoss;
            transmissionParameters[i].noiseFigure = radioParameters.noiseFigure;
            transmissionParameters[i].thermalNoise = radioParameters.thermalNoise;

            transmissionParameters[i].lastUpdateTime = Time.time;
            transmissionParameters[i].targetNode = ieee802154Bases[i];
        }
    }

    void FindOther802154Bases()
    {
        Ieee802154Base[] allIeee802154Bases = FindObjectsByType<Ieee802154Base>(FindObjectsSortMode.None);
        ieee802154Bases = allIeee802154Bases
            .Where(node => node != this && Vector3.Distance(node.nodeObject.transform.position, this.nodeObject.transform.position) < maxD2D_ConnectionDistance)
            .Take(maxD2D_ConnectionNum)
            .ToArray();
    }

}
