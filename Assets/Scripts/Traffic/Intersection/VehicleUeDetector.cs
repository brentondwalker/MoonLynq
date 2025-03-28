using System.Linq;
using UnityEngine;

public class VehicleUeDetector : MonoBehaviour
{
    public UeBase[] ueBases;
    public float maxDetectDistance = 100;
    public int ueCount = 0;

    void Start()
    {
    }

    void Update()
    {
        FindUeBases();
    }

    void FindUeBases()
    {
        UeBase[] allUeBases = FindObjectsByType<UeBase>(FindObjectsSortMode.None);
        ueBases = allUeBases
            .Where(ue => Vector3.Distance(ue.ueObject.transform.position, transform.position) < maxDetectDistance)
            .ToArray();
        ueCount = ueBases.Length;
    }
}