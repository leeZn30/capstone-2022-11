using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LobbyButton : MonoBehaviour
{
    public void goToLobby()
    {
        if (AgoraChannelPlayer.Instance.role != "publisher")
        {
            AgoraChannelPlayer.Instance.leaveChannel();

            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(1);
        }

    }

}
