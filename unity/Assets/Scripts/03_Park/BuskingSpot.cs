using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class BuskingSpot : MonoBehaviourPun, IPunObservable
{
    public int roomNum;

    // 버스킹 장소가 사용되고 있는지
    public bool isUsed = false;

    // 버스킹 장소가 사용되고 있다면, 해당 디바이스 id
    public int deviceID;

    //WebRTC
    public GameObject webRTC;
    public GameObject remoteView;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isUsed);
            stream.SendNext(deviceID);
        }
        else
        {
            this.isUsed = (bool)stream.ReceiveNext();
            this.deviceID = (int)stream.ReceiveNext();
        }
    }

    private void Update()
    {
        /**
        if (isUsed)
        {
            this.GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().color = new Color32(202, 162, 48, 250);
        }
        **/
    }

    [PunRPC]
    void changeColor()
    {
        if (isUsed)
        {
            this.GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().color = new Color32(202, 162, 48, 250);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject player = GameManager.instance.myPlayer;
        if (collision.gameObject == player && player.GetComponent<PhotonView>().IsMine)
        {
            if (isUsed && !player.GetComponent<PlayerControl>().isVideoPanelShown)
            {
                collision.transform.GetComponent<PlayerControl>().OnVideoPanel(0);

                webRTC.SetActive(true);
                remoteView.SetActive(true);
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject player = GameManager.instance.myPlayer;
        if (collision.gameObject == player && player.GetComponent<PhotonView>().IsMine)
        {
            if (player.GetComponent<PlayerControl>().isVideoPanelShown)
                collision.transform.GetComponent<PlayerControl>().OffVideoPanel();
        }
    }


}
