using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class EmojiButton : MonoBehaviour
{
    [SerializeField] private int emojiNum;

    [SerializeField] private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(sendBubble);
    }

    public void sendBubble()
    {
        //player = GameObject.Find("Player");
        player.GetComponent<PlayerControl>().sendEmoji(emojiNum);
        //player.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshPro>().text = "<sprite=" + emojiNum + ">";
        //player.transform.GetChild(0).gameObject.SetActive(true);
    }

}
