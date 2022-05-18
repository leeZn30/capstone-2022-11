using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class BuskingSpot : MonoBehaviourPun
{
    // Room 관련
    public int roomNum;

    // Title 관련
    [SerializeField] private GameObject titleBar;
    public string titleText;
    public string buskerNickname = null;
    public string buskerID = null;

    [SerializeField] private GameObject localuser;

    private void Start()
    {
        titleBar = FindObjectOfType<Canvas>().transform.Find("TitleBar").gameObject;
    }

    public void callSetBuskingZone(string id = null, string name = null, string t = null)
    {
        photonView.RPC("setBuskingZone", RpcTarget.AllBuffered, id, name, t);
    }


    [PunRPC]
    void setBuskingZone(string id = null, string name = null, string t = null)
    {
        buskerID = id;
        buskerNickname = name;
        titleText = t;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {

        GameObject player = GameManager.instance.myPlayer;

        if (collision.gameObject == player && player.GetComponent<PhotonView>().IsMine)
        {
            localuser = player;

            // Agora에 버스킹존 정보 넣기
            AgoraChannelPlayer.Instance.nowBuskingSpot = this;
            AgoraChannelPlayer.Instance.channelName = roomNum.ToString();

            // Agora관련
            AgoraChannelPlayer.Instance.callJoin(1);
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject player = GameManager.instance.myPlayer;
        if (collision.gameObject == player && player.GetComponent<PhotonView>().IsMine)
        {
            localuser = null;

            AgoraChannelPlayer.Instance.leaveChannel();

            // AgoraManager의 버스킹 존 관련 정보 지우기
            AgoraChannelPlayer.Instance.nowBuskingSpot = null;
            AgoraChannelPlayer.Instance.channelName = null;

            collision.transform.GetComponent<PlayerControl>().OffVideoPanel();
        }
    }

    public void callInsideUserJoin(string channelName) // localuser은 동기화X / Join은 동기화O > local만 찾아서 가능
    {
        if (channelName == AgoraChannelPlayer.Instance.channelName)
            photonView.RPC("insideUserJoin", RpcTarget.OthersBuffered, null);
    }

    [PunRPC]
    public void insideUserJoin()
    {
        if (AgoraChannelPlayer.Instance.role != "publisher" && localuser != null)
        {
            AgoraChannelPlayer.Instance.callJoin(1);
        }
    }

    public void onTitleBar()
    {
        titleBar.GetComponentInChildren<TextMeshProUGUI>().text = buskerNickname + ": " + titleText;
        titleBar.SetActive(true);
    }

    public void offTitleBar()
    {
        titleBar.GetComponentInChildren<TextMeshProUGUI>().text = null;
        titleBar.SetActive(false);
    }

}
