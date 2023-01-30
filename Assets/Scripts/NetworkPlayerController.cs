using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Class;
using UnityEngine;
using NetworkPlayer = Class.NetworkPlayer;

public class NetworkPlayerController : MonoBehaviour
{
    public int ListenPort = 11000;
    private IPEndPoint listenEndPoint;
    public string IPAddress = "192.168.80.222";
    private UdpClient _udpClient;
    private int size = 1024;
    private byte[] recieveBuffer;

    private bool recieveData;

    // [SerializeField] private List<Blade> _blades;
    [SerializeField] private List<NetworkPlayer> _networkPlayers;

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
        string returnData = JsonHelper.FixJson(Encoding.ASCII.GetString(message));
        // UnityMainThreadDispatcher.Instance().Enqueue(() => Debug.Log(returnData));
        NetworkPlayerData[] data = JsonHelper.FromJson<NetworkPlayerData>(returnData);
        UnityMainThreadDispatcher.Instance().Enqueue(() => UpdatePlayerBladePosition(data));
        socket.BeginReceive(new AsyncCallback(OnReceived), socket);
    }

    private void UpdatePlayerBladePosition(NetworkPlayerData[] networkPlayerDatas)
    {
        int index = 0;
        foreach (var data in networkPlayerDatas)
        {
            Vector2 leftHandScreenPos = new Vector2(data.LeftHandNormalPosX, data.LeftHandNormalPosY);
            Vector2 rightHandScreenPos = new Vector2(data.RightHandNormalPosX, data.RightHandNormalPosY);
            // Vector3 playerBodyPos = new Vector3(data.PlayerBodyPositionX, data.PlayerBodyPositionY, data.PlayerBodyPositionZ);
            _networkPlayers[index].UpdateBladePosition(leftHandScreenPos, rightHandScreenPos);
            index++;
        }
    }
}