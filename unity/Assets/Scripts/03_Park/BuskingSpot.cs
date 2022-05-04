using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class BuskingSpot : MonoBehaviourPun
{
    public int roomNum;

    // 버스킹 장소가 사용되고 있는지
    public bool isUsed = false;
    public void callChangeUsed()
    {
        photonView.RPC("changeUsed", RpcTarget.AllBuffered, null);
    }

    [PunRPC]
    void changeUsed()
    {
        if (!isUsed)
            isUsed = true;
        else
            isUsed = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject player = GameManager.instance.myPlayer;
        if (collision.gameObject == player && player.GetComponent<PhotonView>().IsMine)
        {
            if (isUsed && !player.GetComponent<PlayerControl>().isVideoPanelShown)
            {
                collision.transform.GetComponent<PlayerControl>().OnVideoPanel(0);
                //webRTCOperate.Instance.roomNum = roomNum;
                //webRTCOperate.Instance.webRTCConnect();
                AgoraManager.Instance.loadEngine("ed5d27a64ca7451189266ef6703397bf");
                AgoraManager.Instance.join(1);

            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject player = GameManager.instance.myPlayer;
        if (collision.gameObject == player && player.GetComponent<PhotonView>().IsMine)
        {
            if (player.GetComponent<PlayerControl>().isVideoPanelShown)
                collision.transform.GetComponent<PlayerControl>().OffVideoPanel();
        }
    }


}
