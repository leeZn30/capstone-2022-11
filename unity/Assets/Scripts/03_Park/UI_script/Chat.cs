using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class Chat : MonoBehaviourPunCallbacks
{
    public GameObject msgList;
    public TMP_InputField ifSendMsg;

    public string emojimsg;

    [SerializeField] private GameObject chatBox;

    public string channelName;

    public void OnEnable()
    {
        channelName = AgoraChannelPlayer.Instance.channelName;

        ifSendMsg.onSubmit.AddListener(delegate { OnSendChatMsg(); });
    }

    public void OnSendEmoji()
    {
        string msg = string.Format("[{0}]  {1}"
                                   , UserData.Instance.user.nickname // PhotonNetwork.LocalPlayer.NickName
                                   , emojimsg);


        photonView.RPC("ReceiveMsg", RpcTarget.AllBuffered, msg, channelName);
    }

    public void OnSendChatMsg()
    {
        if (ifSendMsg.text != null)
        {
            string tmp = ifSendMsg.text.Replace(" ", "");

            if (tmp != "")
            {
                string msg = string.Format("[{0}]  {1}"
                                           , UserData.Instance.user.nickname // PhotonNetwork.LocalPlayer.NickName
                                           , ifSendMsg.text);

                photonView.RPC("ReceiveMsg", RpcTarget.AllBuffered, msg, channelName);

                ifSendMsg.text = "";
            }
        }
    }

    [PunRPC]
    void ReceiveMsg(string msg, string _channel)
    {
        if (_channel == channelName)
        {

            GameObject go = Instantiate<GameObject>(chatBox);
            go.GetComponentInChildren<TextMeshProUGUI>().text = msg;


            go.transform.parent = msgList.transform;
            go.transform.localScale = new Vector3(1, 1, 0);
        }
    }
}
