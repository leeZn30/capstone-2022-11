using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class Chat : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI msgList;
    public TMP_InputField ifSendMsg;

    public void OnSendChatMsg()
    {
        string msg = string.Format("[{0}]  {1}"
                                   ,UserData.Instance.user.nickname // PhotonNetwork.LocalPlayer.NickName
                                   , ifSendMsg.text);
        photonView.RPC("ReceiveMsg", RpcTarget.Others, msg);
        ReceiveMsg(msg);
    }

    [PunRPC]
    void ReceiveMsg(string msg)
    {
        msgList.text += "\n" + msg;
    }
}
