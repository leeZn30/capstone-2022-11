using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class EmojiButton : MonoBehaviourPun
{
    // 이모티콘 종류
    [SerializeField] private int emojiNum;


    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(sendBubble);
    }

    [PunRPC]
    public void sendBubble()
    {
        GameObject player = GameManager.instance.myPlayer;
        if (player.GetComponent<PhotonView>().IsMine && this.GetComponentInParent<EmoticonPanel>().mode == 0)
        {
            player.GetComponent<PlayerControl>().rpcEmoji(emojiNum);
        }
        else if (player.GetComponent<PhotonView>().IsMine && this.GetComponentInParent<EmoticonPanel>().mode == 1)
        {
            GameObject ChatPanel = this.GetComponentInParent<EmoticonPanel>().ChatPanel;
            ChatPanel.GetComponent<Chat>().emojimsg = "<sprite=" + emojiNum + ">";
            ChatPanel.GetComponent<Chat>().OnSendChatMsg();
        }
    }


}
