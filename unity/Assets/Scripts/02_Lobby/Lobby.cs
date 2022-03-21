using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviour
{
    public Button settingBtn;
    public Button openSongPageBtn;
    public Button loadSquareSceneBtn;
    public Button characterSetBtn;

    public CharacterSetPage characterSetPage;
    public SongPage songPage;
    public LobbySetting lobbySetting;

    public Character character;
    void Start()
    {
        //버튼 이벤트 등록
        settingBtn.onClick.AddListener(delegate { lobbySetting.Init(); });
        loadSquareSceneBtn.onClick.AddListener(LoadSquareScene);
        openSongPageBtn.onClick.AddListener(delegate { songPage.Open(); });
        characterSetBtn.onClick.AddListener(delegate { characterSetPage.Open(); });

        characterSetPage.Close();
        songPage.Close();

        characterSetPage.OnChangeCharacter += ChangeCharacter;
        //user 데이더 서버에서 받아와서 UserData.Instance에 저장
        //여기 쓰기(함수 호출)
        //


        ChangeCharacter();
    }

    void LoadSquareScene()
    {//광장씬 로드

    }
    void ChangeCharacter()
    {
        character.ChangeSprite(UserData.Instance.user.character);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
