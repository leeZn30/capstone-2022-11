using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class InteractiveObject : MonoBehaviour
{
    //기존 parent을 지정하는 것으로 바꿈/
    public GameObject componentObj;
    /**
     * 상호작용 종류
     * 0) 버스킹
     * 1) 순간이동기
     * */
    [SerializeField] protected int InteractiveType;

    private BuskingSpot buskingSpot;
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (componentObj == null) return;

        GameObject player = GameManager.instance.myPlayer;
        if (player.GetComponent<PhotonView>().IsMine && collision.gameObject == player)
        {
            switch (InteractiveType)
            {
                case 0:
                    if (componentObj.TryGetComponent<BuskingSpot>(out buskingSpot))
                    {

                        if (!buskingSpot.isUsed)
                        {
                            player.GetComponent<PlayerControl>().OnInteractiveButton(InteractiveType);
                            player.GetComponent<PlayerControl>().InteractiveButton.GetComponent<Button>().onClick.AddListener(
                                delegate { player.GetComponent<PlayerControl>().OnVideoPanel(1); });
                        }                      
                    }
                    break;

                default:
                    if (collision.tag == "Character")
                    {
                        collision.transform.GetComponent<PlayerControl>().OnInteractiveButton(InteractiveType);
                    }
                    break;
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject player = GameManager.instance.myPlayer;
        if (collision.gameObject == player && player.GetComponent<PhotonView>().IsMine)
        {
            player.GetComponent<PlayerControl>().OffInteractiveButton();

        }
    }

}
