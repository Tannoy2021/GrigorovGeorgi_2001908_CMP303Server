using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend
{
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendTcpData(_packet);
    }

    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendTcpData(_packet);
        }
    }
    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].tcp.SendTcpData(_packet);
            }
        }
    }

    #region Packets
    public static void Welcome(int _toClient)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_toClient);
            SendTCPData(_toClient, _packet);
        }
    }

    public static void SpawnPlayer(int _toClient, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);
            SendTCPData(_toClient, _packet);
        }
    }

    public static void PlayerPosition(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);
            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerRotation(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.rotation);
            SendTCPDataToAll(_player.id, _packet);
        }
    }

    public static void PlayerDisconnected(int _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            _packet.Write(_player);
            SendTCPDataToAll(_packet);
        }
    }

    public static void CreateBuffSpawner(int toClient, int spawnerId, Vector3 pos, bool hasItem)
    {
        using (Packet _packet = new Packet((int)ServerPackets.createBuffSpawner))
        {
            _packet.Write(spawnerId);
            _packet.Write(pos);
            _packet.Write(hasItem);
            SendTCPData(toClient, _packet);
        }
    }
    public static void BuffSpawned(int _spawnerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.buffSpawned))
        {
            _packet.Write(_spawnerId);
            SendTCPDataToAll(_packet);
        }
    }
    public static void BuffPickedUp(int _spawnerId, int _byPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.buffPickedUp))
        {
            _packet.Write(_spawnerId);
            _packet.Write(_byPlayer);
            SendTCPDataToAll(_packet);
        }
    }
    public static void CreateFinishLine(int toClient,Vector3 pos)
    {
        using (Packet _packet = new Packet((int)ServerPackets.createFinishLine))
        {
            _packet.Write(pos);
            SendTCPData(toClient, _packet);
        }
    }
    public static void FinishCollected(int _byPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.finishCollected))
        {
            _packet.Write(_byPlayer);          
            SendTCPDataToAll(_packet);           
        }
    }
}
#endregion