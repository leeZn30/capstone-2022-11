using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
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
        Debug.Log("Network object Start!");

        //���ӿ� �ʿ��� ���� (���� ����) ����
        PhotonNetwork.GameVersion = this.gameVersion;

        //설정한 정보로 마스터 서버 접속 시도
        PhotonNetwork.ConnectUsingSettings();

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            this.joinButton.interactable = false;
            this.connectionInfoText.text = "마스터 서버에 접속하는 중...";
        }
        else
        {
            this.joinButton.interactable = true;
            this.connectionInfoText.text = "온라인 : 마스터 서버와 연결 됨";
        }
    }


    // ������ ���� ���� ������ �ڵ� ����
    public override void OnConnectedToMaster()
    {
        this.joinButton.interactable = true;
        this.connectionInfoText.text = "온라인 : 마스터 서버와 연결 됨";

    }

    // ������ ���� ���� ���н� �ڵ� ����
    public override void OnDisconnected(DisconnectCause cause)
    {
        this.joinButton.interactable = false;
        this.connectionInfoText.text = "�������� : ������ ������ ������� ����\n ���� ��õ���... ";
        //설정한 정보로 마스터 서버 접속 시도
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
            this.connectionInfoText.text = "룸에 접속 중..";
            //PhotonNetwork.JoinRandomRoom();

            // �� �����
            PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 100 });

        }
        else
        {
            this.connectionInfoText.text = "방 참가 완료! 광장 이동 중..";
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
        //PhotonNetwork.LoadLevel("Park test scene");

        PhotonNetwork.LocalPlayer.NickName = UserData.Instance.user.nickname;
        Hashtable playerData = new Hashtable();

        playerData.Add("character", UserData.Instance.user.character);
        playerData.Add("id", UserData.Instance.user.id);

        // ���� ���޷� �ؾ���, setcustomproperties�ϸ� ���� ���� ����
        PhotonNetwork.LocalPlayer.CustomProperties = playerData;

        // �굵 ����� ���� ����ȭ��
        playerData["id"] = UserData.Instance.user.id;
        playerData["character"] = UserData.Instance.user.character;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerData);

    }
}
