using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBuff : MonoBehaviour
{
    public static Dictionary<int, SpeedBuff> spawns = new Dictionary<int, SpeedBuff>();
    private static int nextSpawnerId = 1;
    public int spawnerId;
    public bool hasItem = false;
    private void Start()
    {
        hasItem = false;
        spawnerId = nextSpawnerId;
        nextSpawnerId++;
        spawns.Add(spawnerId, this);
        StartCoroutine(SpawnBuff());
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player.AttemptPickup())
            {
               BuffPickedUp(player.id);              
            }
        }
    }
    private IEnumerator SpawnBuff()
    {
        yield return new WaitForSeconds(4f);
        hasItem = true;
        ServerSend.BuffSpawned(spawnerId);
    }
    private void BuffPickedUp(int player)
    {
        hasItem = false;
        ServerSend.BuffPickedUp(spawnerId, player);
        StartCoroutine(SpawnBuff());
    }
}
