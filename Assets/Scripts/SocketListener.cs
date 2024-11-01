using System.IO;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class SocketListener : MonoBehaviour
{
    private TcpListener tcpListener;
    private TcpClient client;

    void Start()
    {
        tcpListener = new TcpListener(IPAddress.Any, 8080); 
        tcpListener.Start();
        Debug.Log("Server Started on port 8080");
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), null);
    }

    void OnClientConnect(IAsyncResult ar)
    {
        client = tcpListener.EndAcceptTcpClient(ar);
        Debug.Log("Client Connected!");
        StreamReader reader = new StreamReader(client.GetStream(), Encoding.UTF8);

        string message = reader.ReadLine();
        Debug.Log("Received Message: " + message);
    }
}