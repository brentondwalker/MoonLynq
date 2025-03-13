using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class JakesFading : MonoBehaviour
{
    private const float SPEED_OF_LIGHT = 3.0e8f; 

    private Dictionary<int, Dictionary<int, JakesFadingData>> jakesFadingMap = new Dictionary<int, Dictionary<int, JakesFadingData>>();
    private Dictionary<int, Dictionary<int, JakesFadingData>> jakesFadingMapUe = new Dictionary<int, Dictionary<int, JakesFadingData>>();

    private int fadingPaths = 6; 
    private float delayRMS = 363e-9f;

    public float dopplerSpeed = 0;

    public double JakesFadingComputation(int numBands, int band, float speed, bool isUpload, UeBase ueBase)
    {
        Dictionary<int, Dictionary<int, JakesFadingData>> actualJakesMap = isUpload
            ? jakesFadingMap : jakesFadingMapUe ;

        int nodeId = ueBase.ueId;
        float carrierFrequency = ueBase.ueParameters.frequency;

        dopplerSpeed = speed;

        if (!actualJakesMap.ContainsKey(nodeId) || speed != 0)
        {
            actualJakesMap[nodeId] = new Dictionary<int, JakesFadingData>();  

            for (int j = 0; j < numBands; j++)
            {
                JakesFadingData temp = new JakesFadingData();

                for (int i = 0; i < fadingPaths; i++)
                {
                    temp.angleOfArrival.Add(Mathf.Cos(Random.Range(0, Mathf.PI)));
                    temp.delaySpread.Add(Exponential(delayRMS));
                }

                actualJakesMap[nodeId][j] = temp;
            }
        }

        float f = carrierFrequency; 
        float t = Time.time - 0.001f; 

        float re_h = 0f;
        float im_h = 0f;

        JakesFadingData actualJakesData = actualJakesMap[nodeId][band];

        float dopplerShift = (speed * f) / SPEED_OF_LIGHT;

        for (int i = 0; i < fadingPaths; i++)
        {
            float phiD = actualJakesData.angleOfArrival[i] * dopplerShift;
            float phiI = actualJakesData.delaySpread[i] * f;
            float phi = 2.0f * Mathf.PI * (phiD * t - phiI);

            float attenuation = 1.0f / Mathf.Sqrt(fadingPaths);

            re_h += attenuation * Mathf.Cos(phi);
            im_h -= attenuation * Mathf.Sin(phi);
        }

        return LinearToDb(re_h * re_h + im_h * im_h);
    }


    private float Exponential(float lambda)
    {
        return -lambda * Mathf.Log(UnityEngine.Random.value);
    }

    private double LinearToDb(float linear)
    {
        return 10f * Mathf.Log10(linear);
    }

}

public class JakesFadingData
{
    public List<float> angleOfArrival = new List<float>();
    public List<float> delaySpread = new List<float>();
}

