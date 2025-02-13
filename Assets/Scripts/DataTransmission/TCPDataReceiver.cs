using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPDataReceiver : MonoBehaviour
{
    private TcpListener server;
    private bool isRunning = false;
    private Thread serverThread;
    public string receivedMessage = "";

    public PlinkSSH plinkInterface;

    void Start()
    {
        serverThread = new Thread(new ThreadStart(StartServer));
        serverThread.IsBackground = true; 
        serverThread.Start();
    }

    void StartServer()
    {
        server = new TcpListener(IPAddress.Parse("127.0.0.1"), 12350);
        server.Start();
        isRunning = true;

        Debug.Log("Server listening on 127.0.0.1:12350");

        try
        {
            while (isRunning)
            {
                if (server.Pending())
                {
                    TcpClient client = server.AcceptTcpClient();
                    HandleClient(client);
                }
                else
                {
                    Thread.Sleep(10); 
                }
            }
        }
        catch (SocketException ex)
        {
            Debug.LogWarning("Server stopped or interrupted: " + ex.Message);
        }
        finally
        {
            server.Stop();
            Debug.Log("Server shut down.");
        }
    }

    private void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];
        int bytesRead;

        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
        {
            string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Debug.Log("Received message: " + message);

            receivedMessage = message;

            string response = "Processed: " + message;
            byte[] responseBytes = Encoding.ASCII.GetBytes(response);
            stream.Write(responseBytes, 0, responseBytes.Length);
        }

        client.Close();
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        if (server != null)
        {
            server.Stop(); 
        }
        if (serverThread != null && serverThread.IsAlive)
        {
            serverThread.Join(); 
        }
    }
}