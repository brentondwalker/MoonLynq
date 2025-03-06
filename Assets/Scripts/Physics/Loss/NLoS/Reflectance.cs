using System;
using System.Collections.Generic;
using UnityEngine;

public class Reflectance : MonoBehaviour
{

    public double ComputeReflectance(MaterialProperties incidentMaterial, MaterialProperties refractiveMaterial, float angle)
    {



        float n1 = GetRefractiveIndex(incidentMaterial);
        float n2 = GetRefractiveIndex(refractiveMaterial);
        float sinAngle = Mathf.Sin(angle);
        float cosAngle = Mathf.Cos(angle);

        float k = Mathf.Sqrt(1 - Mathf.Pow(n1 / n2 * sinAngle, 2));
        double rs = Math.Pow((n1 * cosAngle - n2 * k) / (n1 * cosAngle + n2 * k), 2);
        double rp = Math.Pow((n1 * k - n2 * cosAngle) / (n1 * k + n2 * cosAngle), 2);
        double reflectance = (rs + rp) / 2;

        //Debug.Log("Angle: " + angle);
        //Debug.Log("Transmittance: " + (1 - reflectance));

        return reflectance;
    }

    float GetRefractiveIndex(MaterialProperties material)
    {
        return material.GetRefractiveIndex();
    }
}
