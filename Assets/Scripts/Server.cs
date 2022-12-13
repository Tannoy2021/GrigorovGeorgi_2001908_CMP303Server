using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
public class Server
{
    public static int MaxPlayers { get; private set; }
    public static int ServerPort { get; private set; }
    public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
    public delegate void Packets(int _fromClient, Packet _packet);
    public static Dictionary<int, Packets> packets;
    private static TcpListener tcpListener;
    private static UdpClient udpListener;
    public static void Start(int _maxPlayers, int _port)
    {
        MaxPlayers = _maxPlayers;
        ServerPort = _port;
        InitializeServerData();
        tcpListener = new TcpListener(IPAddress.Any, ServerPort);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
        udpListener = new UdpClient(ServerPort);
        udpListener.BeginReceive(UDPReceiveCallback, null);
        Debug.Log($"Server started on port {ServerPort}.");
    }
    private static void TCPConnectCallback(IAsyncResult _result)
    {
        TcpClient _client = tcpListener.EndAcceptTcpClient(_result);
        tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
        Debug.Log($"Incoming connection from {_client.Client.RemoteEndPoint}...");
        for (int i = 1; i <= MaxPlayers; i++)
        {
            if (clients[i].tcp.socket == null)
            {
                clients[i].tcp.Connect(_client);
                return;
            }
        }
        Debug.Log($"{_client.Client.RemoteEndPoint} failed to connect: Server full!");
    }
    private static void UDPReceiveCallback(IAsyncResult _result)
    {
            IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
            udpListener.BeginReceive(UDPReceiveCallback, null);
            if (_data.Length < 4)
            {
                return;
            }
            using (Packet _packet = new Packet(_data))
            {
                int _clientId = _packet.ReadInt();

                if (_clientId == 0)
                {
                    return;
                }
                if (clients[_clientId].udp.endPoint == null)
                {
                    clients[_clientId].udp.Connect(_clientEndPoint);
                    return;
                }
                if (clients[_clientId].udp.endPoint.ToString() == _clientEndPoint.ToString())
                {
                    clients[_clientId].udp.HandleUdpData(_packet);
                }
            }
    }
    private static void InitializeServerData()
    {
        for (int i = 1; i <= MaxPlayers; i++)
        {
            clients.Add(i, new Client(i));
        }
        packets = new Dictionary<int, Packets>()
            {
                { (int)ClientPackets.userConnected, ServerHandle.UserConnected },
                { (int)ClientPackets.playerMovement, ServerHandle.PlayerMovement },
            };
        Debug.Log("Initialized packets.");
    }
    public static void Stop()
    {
        tcpListener.Stop();
        udpListener.Close();
    }
}
