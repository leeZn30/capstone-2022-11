using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

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
        player = GameManager.instance.myPlayer;

        if (player.GetPhotonView().IsMine)
        {
            character = player.transform.GetChild(0).GetComponent<Character>();
            character.ChangeSprite(UserData.Instance.user.character);
            player.transform.GetChild(2).GetComponent<TextMeshPro>().text = UserData.Instance.user.nickname;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
