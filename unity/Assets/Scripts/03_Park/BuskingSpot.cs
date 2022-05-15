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
    public bool isUsed = false;

    // Title 관련
    [SerializeField] private GameObject titleBar;
    public string titleText;
    public string buskerNickname;

    [SerializeField] private GameObject localuser;

    private void Start()
    {
        titleBar = FindObjectOfType<Canvas>().transform.Find("TitleBar").gameObject;
    }

    public void callChangeUsed(string name = null, string t = null)
    {
        photonView.RPC("changeUsed", RpcTarget.AllBuffered, name, t);
    }

    [PunRPC]
    void changeUsed(string name = null, string t = null)
    {
        if (!isUsed)
        {
            isUsed = true;
            buskerNickname = name;
            titleText = t;
        }
        else
        {
            isUsed = false;
            buskerNickname = null;
            titleText = null;
        }
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

            if (isUsed && !player.GetComponent<PlayerControl>().isVideoPanelShown)
            {
                collision.transform.GetComponent<PlayerControl>().OnVideoPanel(0);

                // Agora관련
                AgoraChannelPlayer.Instance.callJoin(1);

                player.GetComponent<PlayerControl>().OnInteractiveButton(2);

            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        
        GameObject player = GameManager.instance.myPlayer;
        if (collision.gameObject == player && player.GetComponent<PhotonView>().IsMine)
        {
            localuser = null;

            AgoraChannelPlayer.Instance.leaveChannel();
            GameManager.instance.myPlayer.GetComponent<PlayerControl>().OffInteractiveButton(2); // 팔로우 버튼 삭제 우연히 겹칠때를 대비해서 하나 더

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
            // 만약 방송 준비중이었다면 지워줌
            localuser.GetComponent<PlayerControl>().OffVideoPanel();
            localuser.GetComponent<PlayerControl>().isMoveAble = true;
            localuser.GetComponent<PlayerControl>().isUIActable = true;

            localuser.GetComponent<PlayerControl>().OnVideoPanel(0);
            AgoraChannelPlayer.Instance.callJoin(1);
            localuser.GetComponent<PlayerControl>().OnInteractiveButton(2); // 버튼 활성화 아니었던 사람들
            localuser.GetComponent<PlayerControl>().changeInteractiveButton(2); // 버튼 활성화 되어있던 사람들
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
