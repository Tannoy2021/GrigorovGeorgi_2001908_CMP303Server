using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
        public static void UserConnected(int _fromClient, Packet _packet)
        {
        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
        Server.clients[_fromClient].SendIntoGame();
        }
        public static void PlayerMovement(int _fromClient, Packet _packet)
        {
            bool[] _clientInputs = new bool[_packet.ReadInt()];
            for (int i = 0; i < _clientInputs.Length; i++)
            {
                _clientInputs[i] = _packet.ReadBool();
            }           
            Quaternion rotation = _packet.ReadQuaternion();
            Server.clients[_fromClient].player.SetInput(_clientInputs, rotation);
        }

}

