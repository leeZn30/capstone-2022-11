using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;
using LitJson;
using System;

using Cysharp.Threading.Tasks;
public class Lobby : MusicWebRequest
{

    public Button openSongPageBtn;
    public Button loadSquareSceneBtn;
    public Button followPageBtn;
    public Button characterSetBtn;

    public CharacterSetPage characterSetPage;
    public SongPage songPage;
    public FollowPage followPage;

    public Character character;
    public TextMeshProUGUI userNickname;
    public TextMeshProUGUI followNum;
    public TextMeshProUGUI followerNum;

    void Start()
    {
        
        //버튼 이벤트 등록
        
        openSongPageBtn.onClick.AddListener(delegate { songPage.Open(); });
        characterSetBtn.onClick.AddListener(delegate { characterSetPage.Open(); });
        followPageBtn.onClick.AddListener(delegate { followPage.Open(); });
        
        characterSetPage.Close();
        songPage.Close();
        followPage.Close();

        characterSetPage.OnChangeCharacter += ChangeCharacter;
        UserData.Instance.OnChangeFollow += UpdateFollow;
        
        GetUserData();

    }
    void UpdateFollow()
    {
        Debug.Log("update follow");
        followNum.text = UserData.Instance.user.followNum+"\n팔로우";
        followerNum.text = UserData.Instance.user.followerNum + "\n팔로워";

    }
    async void GetUserData()
    {
        User us = await GET_UserInfoAsync(UserData.Instance.user.id);
        if (us!=null)
        {
            UserData.Instance.user = us;
            userNickname.text = UserData.Instance.user.GetName();
            UpdateFollow();
            ChangeCharacter();

            StringList listNameList = await GET_ListNameAsync();
            //재생목록리스트 업데이트
            if (listNameList == null) return;

            MusicController.Instance.SetOptions(listNameList.stringList);
        }

    }
    void ChangeCharacter()
    {
        Debug.Log(UserData.Instance.user.character);
        character.ChangeSprite(UserData.Instance.user.character);
    }
    
    JsonData JsonToObject(string json)
    {
        return JsonMapper.ToObject(json);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
