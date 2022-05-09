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
            // AgoraManager에 버스킹존 정보 넣기
            AgoraManager.Instance.nowBuskingSpot = this;
            AgoraManager.Instance.channelName = roomNum.ToString();

            if (isUsed && !player.GetComponent<PlayerControl>().isVideoPanelShown)
            {
                collision.transform.GetComponent<PlayerControl>().OnVideoPanel(0);

                // Agora관련
                AgoraManager.Instance.loadEngine();
                AgoraManager.Instance.callJoin(1);

            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject player = GameManager.instance.myPlayer;
        if (collision.gameObject == player && player.GetComponent<PhotonView>().IsMine)
        {
            // AgoraManager의 버스킹 존 관련 정보 지우기
            AgoraManager.Instance.nowBuskingSpot = null;
            AgoraManager.Instance.channelName = null;

            // AgoraEngine unloaded
            AgoraManager.Instance.unloadEngine();

            if (player.GetComponent<PlayerControl>().isVideoPanelShown)
                collision.transform.GetComponent<PlayerControl>().OffVideoPanel();
        }
    }


}
