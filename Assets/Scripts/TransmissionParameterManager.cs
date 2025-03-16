using UnityEngine;

public class TransmissionParameterManager : MonoBehaviour
{
    public class TransmissionParameter
    {
        public int nodeA_Id;
        public Vector3 positionA;
        public double txPowerA;
        public double antennaGainA;

        public int nodeB_Id;
        public Vector3 positionB;
        public double txPowerB;
        public double antennaGainB;

        public float frequency;
        public int numBands;
        public int numLayers;
        public double cableLoss;
        public double noiseFigure;
        public double thermalNoise;

        public UeBase targetUeBase;
        public float lastUpdateTime;
    }
}
