using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviourPunCallbacks
{

    // Player 객체 받아오기
    //[SerializeField] private GameObject player;

    // 닉네임
    [SerializeField] private string nickName;

    // character 외형
    [SerializeField] private int appearance;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            setPlayer(PhotonNetwork.LocalPlayer);
            photonView.RPC("setPlayer", RpcTarget.Others, PhotonNetwork.LocalPlayer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    void setPlayer(Player player)
    {
        Hashtable playerData = player.CustomProperties;

        // 정보 저장
        appearance = (int)player.CustomProperties["character"];
        nickName = player.NickName;

        transform.GetChild(0).gameObject.GetComponent<Character>().ChangeSprite((int)playerData["character"]);
        transform.GetChild(2).gameObject.GetComponent<TextMeshPro>().text = player.NickName;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (photonView.IsMine)
            photonView.RPC("setPlayer", RpcTarget.OthersBuffered, PhotonNetwork.LocalPlayer);
    }



}
