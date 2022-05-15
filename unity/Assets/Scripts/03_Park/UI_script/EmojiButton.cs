using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class EmojiButton : MonoBehaviourPun
{
    // 이모티콘 종류
    [SerializeField] private int emojiNum;

    // bigVideoPanel 관련
    [SerializeField] private GameObject bigVideoPanel;
    [SerializeField] private Chat chatPanel;

    // Start is called before the first frame update
    void Start()
    {
        bigVideoPanel = FindObjectOfType<Canvas>().transform.Find("bigVideoPanel").gameObject;
        chatPanel = FindObjectOfType<Canvas>().transform.Find("bigVideoPanel").Find("ChatView").GetComponent<Chat>();

        this.GetComponent<Button>().onClick.AddListener(sendBubble);
    }

    [PunRPC]
    public void sendBubble()
    {
        GameObject player = GameManager.instance.myPlayer;
        player.GetComponent<PlayerControl>().rpcEmoji(emojiNum);

        if (AgoraChannelPlayer.Instance.nowBuskingSpot != null && AgoraChannelPlayer.Instance.nowBuskingSpot.isUsed)
        {
            chatPanel.emojimsg = "<sprite=" + emojiNum + ">";
            chatPanel.OnSendEmoji();
        }
    }


}
