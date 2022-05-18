using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MusicSpot : MonoBehaviourPun
{ 
    public int spotNum;
    public List<string> enterUserIds;
    public State state;

    public MusicZoneUI musicZoneUI;
    public enum State
    {
        None,Listening
    }
    // Start is called before the first frame update
    void Start()
    {
        state = State.None;
        enterUserIds = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void RPCJoin()
    {
        photonView.RPC("JoinEnterUsers", RpcTarget.All);
    }

    [PunRPC]
    public void JoinEnterUsers()
    {
        if(enterUserIds.Contains(UserData.Instance.id))
        {//내가 영역안에 존재하면
            if (musicZoneUI == null)
                musicZoneUI = GameObject.FindObjectOfType<MusicZoneUI>();
            musicZoneUI.CallJoinZone(this);
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerManager pm;
        if(collision.gameObject.TryGetComponent<PlayerManager>(out pm))
        {
            enterUserIds.Add(pm.GetId());
        }
        GameObject player = GameManager.instance.myPlayer;

        if (collision.gameObject == player && player.GetComponent<PhotonView>().IsMine)
        {
            if (musicZoneUI == null)
                musicZoneUI = GameObject.FindObjectOfType<MusicZoneUI>();
            musicZoneUI.CallJoinZone(this);

        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        PlayerManager pm;
        if (collision.gameObject.TryGetComponent<PlayerManager>(out pm))
        {
            enterUserIds.Remove(pm.GetId());
        }
        GameObject player = GameManager.instance.myPlayer;

        if (collision.gameObject == player && player.GetComponent<PhotonView>().IsMine)
        {
            state = State.None;

            if (musicZoneUI == null)
                musicZoneUI = GameObject.FindObjectOfType<MusicZoneUI>();

            musicZoneUI.CloseListeningUI();
            musicZoneUI.CleatSpotData();
        }
    }
}