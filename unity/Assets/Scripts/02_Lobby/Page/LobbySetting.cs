using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LobbySetting : Page
{
    public Button gotoMainSceneBtn;

    void LogoutAndClose()
    {

        UserData.Instance.Clear();
        SceneManager.LoadScene("01_Main");

    }
    override public void Init()
    {//설정창 초기세팅 and 기존 설정 불러오기
        gotoMainSceneBtn.onClick.AddListener(LogoutAndClose);
    }

}
