using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BuskingSpot : MonoBehaviour
{
    [SerializeField] private GameObject videoPanel;

    public bool isUsed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Character" && isUsed)
        {
            collision.transform.GetComponent<PlayerControl>().OnVideoPanel();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Character")
        {
            collision.transform.GetComponent<PlayerControl>().OffVideoPanel();
        }
    }


}
