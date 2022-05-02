using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class BuskingSpot : MonoBehaviourPun, IPunObservable
{
    public int roomNum;

    // ����ŷ ��Ұ� ���ǰ� �ִ���
    public bool isUsed = false;

    // ����ŷ ��Ұ� ���ǰ� �ִٸ�, �ش� ����̽� id
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

    [PunRPC]
    void setUsed()
    {
        if (!isUsed)
        {
            isUsed = true;
        }
    }

    public void callSetUsed()
    {
        photonView.RPC("setUsed", RpcTarget.All);
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
