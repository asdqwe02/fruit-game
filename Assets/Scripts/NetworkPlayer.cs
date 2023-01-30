using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{
    public int ListenPort = 11000;
    private IPEndPoint listenEndPoint;
    public string IPAddress = "192.168.80.222";
    private UdpClient _udpClient;
    private int size = 1024;
    private byte[] recieveBuffer;
    private bool recieveData;

    void Start()
    {
        // recieveBuffer = new byte[size];
        // _udpClient = new UdpClient(ListenPort);
        // IPEndPoint ep = new IPEndPoint(System.Net.IPAddress.Parse(IPAddress), ListenPort);
        // _udpClient.Connect(ep);
        // _udpClient.Client.Blocking = false;
        _udpClient = new UdpClient(ListenPort);
        listenEndPoint = new IPEndPoint(System.Net.IPAddress.Parse(IPAddress), 0); // endpoint where server is listening
        _udpClient.Connect(listenEndPoint);
        _udpClient.BeginReceive(new AsyncCallback(OnReceived), _udpClient);

        // IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
    }

    private void OnReceived(IAsyncResult ar)
    {
        UdpClient socket = ar.AsyncState as UdpClient;
        IPEndPoint source = new IPEndPoint(0, 0);
        byte[] message = socket.EndReceive(ar, ref source);
        string returnData = Encoding.ASCII.GetString(message);
        // UnityMainThreadDispatcher.Instance().Enqueue(() => Debug.Log("recieved data"));
        UnityMainThreadDispatcher.Instance().Enqueue(() => Debug.Log(returnData));
        // Debug.Log("recieved data");
        // recieveData = true;
        socket.BeginReceive(new AsyncCallback(OnReceived), socket);
        // Debug.Log(ar.ToString());
    }

    void Update()
    {

        // if (recieveData)
        // {
        //     Debug.Log("recieved data");
        // }
        // byte[] data = Encoding.ASCII.GetBytes("request data from server");
        // _udpClient.Send(data, data.Length);
    }
}
