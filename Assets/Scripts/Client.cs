using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static int bufferSize = 4096;
    public int id;
    public Player player;
    public TCP tcp;
   public UDP udp;
    public static Client instance;
    public Client(int _clientId)
    {
        id = _clientId;
        tcp = new TCP(id);
        udp = new UDP(id);
    }
    public class TCP
    {
        public TcpClient socket;
        private readonly int id;
        private NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;
        public TCP(int _id)
        {
            id = _id;
        }
        public void Connect(TcpClient _socket)
        {
            socket = _socket;
            socket.ReceiveBufferSize = bufferSize;
            socket.SendBufferSize = bufferSize;
            stream = socket.GetStream();
            receivedData = new Packet();
            receiveBuffer = new byte[bufferSize];
            stream.BeginRead(receiveBuffer, 0, bufferSize, ReceiveCallback, null);
            ServerSend.Welcome(id);
        }
        public void SendTcpData(Packet _packet)
        {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
        }
        private void ReceiveCallback(IAsyncResult _result)
        {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    Server.clients[id].DisconnectUdpTcp();
                    return;
                }
                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);
                receivedData.Reset(HandleTcpData(_data));
                stream.BeginRead(receiveBuffer, 0, bufferSize, ReceiveCallback, null);
        }
        private bool HandleTcpData(byte[] _data)
        {
            int _packetLength = 0;
            receivedData.SetBytes(_data);
            if (receivedData.UnreadLength() >= 4)
            {
                _packetLength = receivedData.ReadInt();
                if (_packetLength <= 0)
                {
                    return true; 
                }
            }
            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {

                byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        Server.packets[_packetId](id, _packet);
                    }
                });
                _packetLength = 0; 
                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }
            }
            if (_packetLength <= 1)
            {
                return true;
            }
            return false;
        }
        public void DisconnectTcp()
        {
            socket.Close();
            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }
    public class UDP
    {
        public IPEndPoint endPoint;
        private int id;
        public UDP(int _id)
        {
            id = _id;
        }
        public void Connect(IPEndPoint _endPoint)
        {
            endPoint = _endPoint;
        }
        public void HandleUdpData(Packet _packetData)
        {
            int _packetLength = _packetData.ReadInt();
            byte[] _packetBytes = _packetData.ReadBytes(_packetLength);
            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_packetBytes))
                {
                    int _packetId = _packet.ReadInt();
                    Server.packets[_packetId](id, _packet);
                }
            });
        }
        public void DisconnectUdp()
        {
            endPoint = null;
        }
    }
    public void SendIntoGame()
    {
        player = NetworkManager.instance.InstantiatePlayer();
        player.Initialize(id);

        foreach (Client _client in Server.clients.Values)
        {
            if (_client.player != null)
            {
                if (_client.id != id)
                {
                    ServerSend.SpawnPlayer(id, _client.player);
                }
            }
        }
        foreach (Client _client in Server.clients.Values)
        {
            if (_client.player != null)
            {
                ServerSend.SpawnPlayer(_client.id, player);
            }
        }
        foreach (SpeedBuff speedBuff in SpeedBuff.spawns.Values)
        {
            ServerSend.CreateBuffSpawner(id, speedBuff.spawnerId, speedBuff.transform.position, speedBuff.hasItem);
        }
        foreach (FinishLine finishLine in FinishLine.FinishLines.Values)
        {
            ServerSend.CreateFinishLine(id,finishLine.transform.position);
        }
    }
    private void DisconnectUdpTcp()
    {
        Debug.Log($"{tcp.socket.Client.RemoteEndPoint} has disconnected.");
        ThreadManager.ExecuteOnMainThread(() =>
        {
            UnityEngine.Object.Destroy(player.gameObject);
            player = null;
        });
        tcp.DisconnectTcp();
        udp.DisconnectUdp();
        ServerSend.PlayerDisconnected(id);
    }
}
