using UnityEngine;

public class UeParameters : MonoBehaviour
{
    public double txPower = 26;
    public float frequency = 2000000000;

    public double noiseFigure = 5.0;
    public double thermalNoise = -104;
    public double antennaGain = 0.0;
    public double cableLoss = 2.0;

    public int numBands = 1;
    public int numPRBs = 1;
    public int numLayers = 1;
}
