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
    [SerializeField] private GameObject player;

    // 외형
    [SerializeField] private int head;
    [SerializeField] private int body;

    // 닉네임
    [SerializeField] private TextMeshPro nickName;

    // character 객체
    [SerializeField] private Character character;

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
    void setPlayer()
    {
        transform.GetChild(0).GetComponent<Character>().ChangeSprite(UserData.Instance.user.character);
        transform.GetChild(2).GetComponent<TextMeshPro>().text = UserData.Instance.user.nickname;
    }

    [PunRPC]
    void setPlayer(Player player)
    {
        Hashtable playerData = player.CustomProperties;
        transform.GetChild(0).gameObject.GetComponent<Character>().ChangeSprite((int)playerData["character"]);
        transform.GetChild(2).gameObject.GetComponent<TextMeshPro>().text = player.NickName;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (photonView.IsMine)
            photonView.RPC("setPlayer", RpcTarget.Others, PhotonNetwork.LocalPlayer);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        info.Sender.TagObject = this.gameObject;
    }


}
