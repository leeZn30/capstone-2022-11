using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class Chat : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI msgList;
    //public TMP_InputField ifSendMsg;

    public string emojimsg;

    public string channelName;

    public void OnEnable()
    {
        channelName = AgoraChannelPlayer.Instance.channelName;
    }


    public void OnSendChatMsg()
    {
        string msg = string.Format("[{0}]  {1}"
                                   ,UserData.Instance.user.nickname // PhotonNetwork.LocalPlayer.NickName
                                   , emojimsg);

        
        photonView.RPC("ReceiveMsg", RpcTarget.AllBuffered, msg, channelName);
        //ReceiveMsg(msg, channelName);
    }

    [PunRPC]
    void ReceiveMsg(string msg, string _channel)
    {
        if (_channel == channelName)
            msgList.text += "\n" + msg;
    }
}
