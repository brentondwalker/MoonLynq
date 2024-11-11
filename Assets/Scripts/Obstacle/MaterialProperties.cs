using UnityEngine;
using System;

public class MaterialProperties : MonoBehaviour
{
    public enum MaterialType
    {
        Vacuum,
        Air,
        Copper,
        Aluminium,
        Wood,
        Forest,
        Brick,
        Concrete,
        Glass
    }


    public MaterialType materialType; 

    private MaterialProperties materialProperties;
    public float resistivity;
    public float relativePermittivity;
    public float relativePermeability;

    private const float e0 = 8.854187817e-12f; // F/m

    private void Start()
    {
        InitializeMaterialProperties();
    }

    private void InitializeMaterialProperties()
    {
        switch (materialType)
        {
            case MaterialType.Vacuum:
                resistivity = float.NaN;
                relativePermittivity = 1.0f;
                relativePermeability = 1.0f;
                break;
            case MaterialType.Air:
                resistivity = float.NaN;
                relativePermittivity = 1.00058986f;
                relativePermeability = 1.00000037f;
                break;
            case MaterialType.Copper:
                resistivity = 1.68f;
                relativePermittivity = float.NaN;
                relativePermeability = float.NaN;
                break;
            case MaterialType.Aluminium:
                resistivity = 2.65f;
                relativePermittivity = float.NaN;
                relativePermeability = float.NaN;
                break;
            case MaterialType.Wood:
                resistivity = 1e15f;
                relativePermittivity = 5.0f;
                relativePermeability = 1.00000043f;
                break;
            case MaterialType.Forest:
                resistivity = 37e3f;
                relativePermittivity = 1.6f;
                relativePermeability = 1.0f;
                break;
            case MaterialType.Brick:
                resistivity = 3e3f;
                relativePermittivity = 4.5f;
                relativePermeability = 1.0f;
                break;
            case MaterialType.Concrete:
                resistivity = 1e2f;
                relativePermittivity = 4.5f;
                relativePermeability = 1.0f;
                break;
            case MaterialType.Glass:
                resistivity = 1e12f;
                relativePermittivity = 7.0f;
                relativePermeability = 1.0f;
                break;
            default:
                Debug.LogError("Invalid material type selected.");
                break;
        }
    }

    public float GetDielectricLossTangent(float frequency)
    {
        return 1.0f / (2 * Mathf.PI * frequency * resistivity * relativePermittivity * e0);
    }

    public float GetRefractiveIndex()
    {
        return Mathf.Sqrt(relativePermittivity * relativePermeability);
    }

    public float GetPropagationSpeed()
    {
        const float SPEED_OF_LIGHT = 299792458; // m/s
        return SPEED_OF_LIGHT / GetRefractiveIndex();
    }
}