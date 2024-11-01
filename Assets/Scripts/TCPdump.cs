using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCPdump : MonoBehaviour
{
    public InputFieldManager interfaceInput;
    public PlinkSSH plink;
    private void Start()
    {
        
    }
    public void StartTCPDump() 
    {
        plink.StartSSHSession();
        plink.SendCommand("sudo mkdir -p /mnt/Unity/PCAP");
        plink.SendCommand("sudo rm /mnt/Unity/PCAP/"+interfaceInput.inputText+"*");
        plink.SendCommand("sudo tcpdump -i " + interfaceInput.inputText + " -s 60 -G 15 -w /mnt/Unity/PCAP/"+interfaceInput.inputText+"-%m-%d_%H-%M-%S.pcap");
    }
}
