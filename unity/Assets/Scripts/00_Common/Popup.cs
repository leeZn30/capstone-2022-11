using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class Popup : Singleton<Popup>
{
    enum type
    {
        Token, Server
    }
    public TextMeshProUGUI text;
    public GameObject Panel;
    public Button button;
    // Start is called before the first frame update
    void Start()
    {

        button.onClick.AddListener(delegate {
            UserData.Instance.Clear();
            Panel.SetActive(false);
            SceneManager.LoadScene("01_Main");
        });


    }
    public void Open(int i)
    {
        string str = "";
        if (type.Token==(type)i)
        {
            str = "로그인 시간이 만료되어\n자동 로그아웃 됩니다.";
        }
        else
        {
            str = "서버와의 연결이 끊어졌습니다.";
        }
        text.text = str;
        Panel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
