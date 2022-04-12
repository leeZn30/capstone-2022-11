using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
//using System.Windows.Forms;
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
    public TextMeshProUGUI userNickname;

    void Start()
    {
        
        //버튼 이벤트 등록
        settingBtn.onClick.AddListener(delegate { lobbySetting.Open(); });
        openSongPageBtn.onClick.AddListener(delegate { songPage.Open(); });
        characterSetBtn.onClick.AddListener(delegate { characterSetPage.Open(); });
        
        characterSetPage.Close();
        songPage.Close();

        characterSetPage.OnChangeCharacter += ChangeCharacter;

        userNickname.text = UserData.Instance.user.GetName();
        ChangeCharacter();
    }

    void ChangeCharacter()
    {
        Debug.Log(UserData.Instance.user.character);
        character.ChangeSprite(UserData.Instance.user.character);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
