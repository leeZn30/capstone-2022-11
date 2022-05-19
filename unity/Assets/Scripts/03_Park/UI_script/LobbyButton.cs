using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LobbyButton : MonoBehaviourPunCallbacks
{
    private bool isClick = false;
    public void goToLobby()
    {
        if (AgoraChannelPlayer.Instance.role != "publisher")
        {
            isClick = true;
            AgoraChannelPlayer.Instance.leaveChannel();

            PhotonNetwork.LeaveRoom();
            //SceneManager.LoadScene(1);
        }

    }

    public override void OnLeftRoom()
    {
        if (isClick)
        {
            SceneManager.LoadScene(1);
            isClick = false;
        }
       
    }


}
