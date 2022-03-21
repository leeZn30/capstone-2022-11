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
    [SerializeField] GameObject interactiveButton;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        PhotonView player = collision.transform.gameObject.GetPhotonView();
        if (player.IsMine)
        {
            switch (InteractiveType)
            {
                case 0:
                    if (!this.GetComponent<BuskingSpot>().isUsed && collision.tag == "Character")
                    {
                        collision.transform.GetComponent<PlayerControl>().OnInteractiveButton(InteractiveType);
                        interactiveButton.GetComponent<Button>().onClick.AddListener(StartBusking);
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
        if (collision.tag == "Character")
        {
            collision.transform.GetComponent<PlayerControl>().OffInteractiveButton();
        }
    }

    // 버스킹 인터렉티브
    void StartBusking()
    {

    }

}
