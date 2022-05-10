using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1"; // ���� ����

    public TextMeshProUGUI connectionInfoText; // ��Ʈ��ũ ������ ǥ���� �ؽ�Ʈ
    public Button joinButton; // �� ���� ��ư

    public string roomName = "MetaBuskingPark";

    // Start is called before the first frame update
    void Start()
    {
        //���ӿ� �ʿ��� ���� (���� ����) ����
        PhotonNetwork.GameVersion = this.gameVersion;
        //������ ������ ������ ���� ���� �õ�
        PhotonNetwork.ConnectUsingSettings();

        this.joinButton.interactable = false;
        this.connectionInfoText.text = "������ ������ ������...";

    }

    // ������ ���� ���� ������ �ڵ� ����
    public override void OnConnectedToMaster()
    {
        this.joinButton.interactable = true;
        this.connectionInfoText.text = "�¶��� : ������ ������ ���� ��";

    }

    // ������ ���� ���� ���н� �ڵ� ����
    public override void OnDisconnected(DisconnectCause cause)
    {
        this.joinButton.interactable = false;
        this.connectionInfoText.text = "�������� : ������ ������ ������� ����\n ���� ��õ���... ";
        //������ ������ ������ ���� ���� �õ�
        PhotonNetwork.ConnectUsingSettings();
    }

    // �� ���� �õ�
    public void Connect()
    {
        // �ߺ� ���� ����
        this.joinButton.interactable = false;

        // ������ ������ ���� ���̶��
        if (PhotonNetwork.IsConnected)
        {
            //�뿡 �����Ѵ�.
            this.connectionInfoText.text = "�뿡 ����....";
            //PhotonNetwork.JoinRandomRoom();

            // �� �����
            PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 100 });

        }
        else
        {
            this.connectionInfoText.text = "�������� : ������ ������ ���� ���� \n �ٽ� ���� �õ��մϴ�.";
            //������ ������ ������ ���� ���� �õ�
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        if (returnCode == 32766)
            PhotonNetwork.JoinRoom(roomName);

        else
            Debug.Log("�� ���� ����");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("�� ���� ����");
    }

    // �뿡 ���� �Ϸ�� ��� �ڵ� ����
    public override void OnJoinedRoom()
    {

        this.connectionInfoText.text = "�� ���� ����!";

        //��� �� �����ڰ� Main ���� �ε��ϰ� ��
        PhotonNetwork.LoadLevel("03_Park");

        PhotonNetwork.LocalPlayer.NickName = UserData.Instance.user.nickname;
        Hashtable playerData = new Hashtable();

        playerData.Add("character", UserData.Instance.user.character);

        // ���� ���޷� �ؾ���, setcustomproperties�ϸ� ���� ���� ����
        PhotonNetwork.LocalPlayer.CustomProperties = playerData;

        // �굵 ����� ���� ����ȭ��
        playerData["character"] = UserData.Instance.user.character;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerData);

    }
}
