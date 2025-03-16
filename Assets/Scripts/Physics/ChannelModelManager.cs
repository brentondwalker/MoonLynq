using UnityEngine;

public class ChannelModelManager : MonoBehaviour
{
    public UeBase ueBase;
    public GameObject transmission;
    private GameObject[] transmissionList;
    private TransmissionManager[] transmissionManagers;

    public int[] cqi;
    public int[] targetId;

    void Start()
    {
        transmissionList = new GameObject[1];
        transmissionList[0] = transmission;
        transmissionManagers = new TransmissionManager[1];
        transmissionManagers[0] = transmission.GetComponent<TransmissionManager>();
        cqi = new int[1];
    }

    public float timeSinceLastUpdate;
    private float lastUpdateTime;
    public float updateInterval = 0.1f;

    void Update()
    {
        timeSinceLastUpdate = Time.time - lastUpdateTime;
        if (timeSinceLastUpdate >= updateInterval)
        {
            CustomUpdate();
            lastUpdateTime = Time.time;
        }
    }

    void CustomUpdate()
    {
        int connectionNum = ueBase.transmissionParameters.Length;

        //Debug.Log("connectionNum: " + connectionNum);

        if (connectionNum > transmissionList.Length)
        {
            System.Array.Resize(ref transmissionList, connectionNum);
            System.Array.Resize(ref transmissionManagers, connectionNum);  
        }

        for (int i = 0; i < connectionNum; i++)
        {
            if (transmissionList[i] == null)
            {
                transmissionList[i] = Instantiate(transmission, transmission.transform.parent);
                transmissionManagers[i] = transmissionList[i].GetComponent<TransmissionManager>();
            }
        }

        cqi = new int[connectionNum];
        targetId = new int[connectionNum];
        for (int i = 0; i < connectionNum; i++)
        {
            cqi[i] = transmissionManagers[i].GetCqi(ueBase.transmissionParameters[i]);
            targetId[i] = transmissionManagers[i].GetTargetId(ueBase.transmissionParameters[i]);
        }
    }
}