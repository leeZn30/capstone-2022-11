using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Chat : MonoBehaviourPunCallbacks
{
    public Text msgList;
    public InputField ifSendMsg;

    public void OnSendChatMsg()
    {
        string msg = string.Format("[{0}] {1}"
                                   ,GameManager.instance.myPlayer // PhotonNetwork.LocalPlayer.NickName
                                   , ifSendMsg.text);
        photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, msg); //buffer는 적재되어있던 것을 한 번에 가져옴
        ReceiveMsg(msg);
    }

    [PunRPC]
    void ReceiveMsg(string msg)
    {
        msgList.text += "\n" + msg;
    }
}
