using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
public class Popup : Singleton<Popup>
{
    enum type
    {
        Token, Server, Duplicate
    }
    public TextMeshProUGUI text;
    public GameObject Panel;
    public Button button;
    private type _type;
    // Start is called before the first frame update
    void Start()
    {

        button.onClick.AddListener(delegate {

            if (_type == type.Duplicate)
            {              
                FindObjectOfType<LobbyButton>().goToLobby();
                Panel.SetActive(false);
            }
            else
            {

                Panel.SetActive(false);
                FindObjectOfType<LobbySetting>().Logout();            
            }
        });


    }
    public void Open(int i)
    {

        _type = (type)i;

        string str = "";
        if (type.Token== _type)
        {
            str = "로그인 시간이 만료되어\n자동 로그아웃 됩니다.";
        }
        else if(type.Token == _type)
        {
            str = "서버와의 연결이 끊어졌습니다.";
        }
        else
        {
            str = "중복 접속한 아이디가 있습니다.";
        }
        text.text = str;
        Panel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
