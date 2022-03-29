using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class InteractiveObject : MonoBehaviour
{
    /**
     * 상호작용 종류
     * 0) 버스킹
     * 1) 금주의 베스트 송
     * 2) 금주의 베스트 아티스트
     * 3) 순간이동기
     * */
    [SerializeField] protected int InteractiveType;

    // 인터렉티브 버튼
    //[SerializeField] GameObject interactiveButton;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        //PhotonView player = collision.transform.gameObject.GetPhotonView();
        GameObject player = GameManager.instance.myPlayer;
        if (player.GetComponent<PhotonView>().IsMine && collision.gameObject == player)
        {
            switch (InteractiveType)
            {
                case 0:
                    if (!this.GetComponent<BuskingSpot>().isUsed)
                    {
                        //collision.transform.GetComponent<PlayerControl>().OnInteractiveButton(InteractiveType);
                        player.GetComponent<PlayerControl>().OnInteractiveButton(InteractiveType);
                        player.GetComponent<PlayerControl>().InteractiveButton.GetComponent<Button>().onClick.AddListener(
                            delegate {this.GetComponent<BuskingSpot>().StartBusking();});
                    }
                    else
                    {
                        collision.transform.GetComponent<PlayerControl>().OnVideoPanel(0);
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
            collision.transform.GetComponent<PlayerControl>().OffInteractiveButton();
        }
    }

}
