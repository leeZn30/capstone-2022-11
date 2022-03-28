using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class EmojiButton : MonoBehaviourPun
{
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
        if (player.GetComponent<PhotonView>().IsMine)
        {
            player.GetComponent<PlayerControl>().rpctest(emojiNum);
        }
    }


}
