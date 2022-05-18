using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class LobbySetting : MonoBehaviour
{
    public GameObject panel;

    public Button settingBtn;//열기 버튼
    public Button exitBtn;
    public Button logoutBtn;//로그아웃 버튼
    public Button quitBtn;//종료 버튼
    public Button applyBtn;

    public Slider volumeSlider;

    public TMP_Dropdown screenModeDropdown;
    public TMP_Dropdown resolutionDropdown;

    public string[] modeOptions;
    public string[] resolutionOptions;

    private void Start()
    {
        Init();

    }
    void Logout()
    {//로그아웃
        
        UserData.Instance.Clear();
        SceneManager.LoadScene("01_Main");

    }
    void Quit()
    {//프로그램 종료 버튼
        if (SceneManager.GetActiveScene().name == "03_Park")
        {
            if (AgoraChannelPlayer.Instance.role != "publisher")
                Application.Quit();
        }
        else
        {
            Application.Quit();
        }

    }
    public void Load()
    {
        volumeSlider.value = AudioListener.volume;

    }
     public void Init()
    {//설정창 초기세팅 and 기존 설정 불러오기
        volumeSlider.value = AudioListener.volume;
        
        settingBtn.onClick.AddListener(delegate { panel.SetActive(true);
            Load();
        });
        exitBtn.onClick.AddListener(delegate { panel.SetActive(false); });
        applyBtn.onClick.AddListener(SetConfig);
        quitBtn.onClick.AddListener(Quit);

        //모든 씬에서 같이 쓸 수 있도록 null체크
        if(logoutBtn!=null)
            logoutBtn.onClick.AddListener(Logout);


        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < modeOptions.Length; ++i)
            options.Add(new TMP_Dropdown.OptionData(modeOptions[i]));
        screenModeDropdown.options = options;
       

        List<TMP_Dropdown.OptionData> options2 = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < resolutionOptions.Length; ++i)
            options2.Add(new TMP_Dropdown.OptionData(resolutionOptions[i]));

        resolutionDropdown.options = options2;

        volumeSlider.value=  UserData.Instance.sets[0];
        screenModeDropdown.value = (int)UserData.Instance.sets[1];
        resolutionDropdown.value = (int)UserData.Instance.sets[2];
        SetConfig();
    }
    void SetConfig()
    {
        AudioListener.volume = volumeSlider.value;

        UserData.Instance.sets[0] = volumeSlider.value ;
        UserData.Instance.sets[1] = screenModeDropdown.value;
        UserData.Instance.sets[2]=  resolutionDropdown.value;

        if (screenModeDropdown.value == 0)
        {
            SetFullScreen();
        }
        else
        {
            switch (resolutionDropdown.value)
            {
                case 0:
                    Screen.SetResolution(1920,1080,false);
                    break;
                case 1:
                    Screen.SetResolution(1280, 720, false);
                    break;
                case 2:
                    Screen.SetResolution(960,540, false);
                    break;

            }
            Rect rect = new Rect(0, 0, 1, 1);
            Camera.main.rect = rect;
        }

    }
    void SetFullScreen()
    {
        Camera camera = Camera.main;
        Rect rect = camera.rect;
        float scaleheight = ((float)Screen.width / Screen.height) / ((float)16 / 9); // (가로 / 세로)
        float scalewidth = 1f / scaleheight;
        if (scaleheight < 1)
        {
            rect.height = scaleheight;
            rect.y = (1f - scaleheight) / 2f;
        }
        else
        {
            rect.width = scalewidth;
            rect.x = (1f - scalewidth) / 2f;
        }
        camera.rect = rect;
        Screen.SetResolution(1920, 1080, true);
    }
}
