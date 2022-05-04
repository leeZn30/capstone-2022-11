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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject player = GameManager.instance.myPlayer;
        if (player.GetComponent<PhotonView>().IsMine && collision.gameObject == player)
        {
            switch (InteractiveType)
            {
                case 0:
                    if (!GetComponentInParent<BuskingSpot>().isUsed)
                    {
                        player.GetComponent<PlayerControl>().OnInteractiveButton(InteractiveType);
                        // 일단 부딪치면 roomNum, nowBuskingSpot 설정
                        /**
                        webRTCOperate.Instance.roomNum = GetComponentInParent<BuskingSpot>().roomNum;
                        webRTCOperate.Instance.nowBuskingSpot = GetComponentInParent<BuskingSpot>();
                        **/
                        AgoraManager.Instance.nowBuskingSpot = GetComponentInParent<BuskingSpot>();

                        player.GetComponent<PlayerControl>().InteractiveButton.GetComponent<Button>().onClick.AddListener(
                            delegate { player.GetComponent<PlayerControl>().OnVideoPanel(1);});
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
