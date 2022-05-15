using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LobbyButton : MonoBehaviour
{
    public void goToLobby()
    {
        AgoraChannelPlayer.Instance.leaveChannel();

        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(1);
    }

}
