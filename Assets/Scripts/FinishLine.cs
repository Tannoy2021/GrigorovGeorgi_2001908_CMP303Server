using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public static Dictionary<int, FinishLine> FinishLines = new Dictionary<int, FinishLine>();
    public int Id = 1;
    public bool first = true;
    public bool second = false;
    public bool third = false;
    public bool last = false;
    private void Start()
    {
        FinishLines.Add(Id, this);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
                if (first == true)
                {
                player.PlayerFinish();
                    FinishCollected(player.id);                   
                    StartCoroutine(FinishTest());
                    Debug.Log($"First Place");
                }
            else if (second == true)
            {
                player.PlayerFinish();
                FinishCollected(player.id);
                StartCoroutine(SecondFinish());
                Debug.Log($"Second Place");
            }
            else if (third == true)
            {
                player.PlayerFinish();
                FinishCollected(player.id);
                StartCoroutine(ThirdFinish());
                Debug.Log($"Third Place");
            }


            else if (last == true)
                {
                player.PlayerFinish();
                    FinishCollected(player.id);                
                    Debug.Log($"Last Place");
                }
        }
    }
    private void FinishCollected(int player)
    {
        ServerSend.FinishCollected(player);
    }

    private IEnumerator FinishTest()
    {
        first = false;
        yield return new WaitForSeconds(0.5f);
        second = true;
    }
    private IEnumerator SecondFinish()
    {
        second = false;
        yield return new WaitForSeconds(0.5f);
        third = true;
    }

    private IEnumerator ThirdFinish()
    {
        third = false;
        yield return new WaitForSeconds(0.5f);
        last = true;
    }
}
