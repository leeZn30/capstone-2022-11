using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{

    [SerializeField] int cloth;
    [SerializeField] string Name;


    // Start is called before the first frame update
    void Start()
    {
        // if > islocal로 하면 두번씩 실행 > 아마 다른 캐릭터도 islocal은 맞아서 실행하는 느낌...
        /**
        if (photonView.IsMine)
        {
            setPlayer(PhotonNetwork.LocalPlayer);
        }
        **/

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
