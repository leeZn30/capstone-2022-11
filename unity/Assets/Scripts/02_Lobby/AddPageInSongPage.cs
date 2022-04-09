using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.Networking;
using System;

public class AddPageInSongPage : Page
{
    public Button imageUploadBtn;
    public Button musicUploadBtn;
    public Button okayBtn;
    public FileOpenDialog fileOpenDialog;
    
    public Image songImage;
    public TextMeshProUGUI localFileName;

    public MusicControllerMini musicControllerMini;
    public TextMeshProUGUI errorText;

    byte[] musicBytes;
    byte[] imageBytes;
    string[] infos;
    private TMP_InputField[] infoInputs;


    public ToggleGroup toggleGroup;

    void Start()
    {
        Init();
    }
    public override void Init()
    {
        if (isAlreadyInit == false)
        {
            isAlreadyInit = true;
            musicControllerMini.Init();
            infoInputs = GetComponentsInChildren<TMP_InputField>();
            TextMeshProUGUI[] tmps = toggleGroup.GetComponentsInChildren<TextMeshProUGUI>();
            for (int i=0; i<tmps.Length; i++)
            {
                tmps[i].text = GlobalData.Genre[i];

            }
            

            imageUploadBtn.onClick.AddListener(delegate
            {
                string filePath = fileOpenDialog.FileOpen(FileOpenDialog.Type.Image);
                if (!string.IsNullOrEmpty(filePath))
                {
                    
                    LoadImage(filePath);
                    Debug.Log(filePath);
                }
            });
            musicUploadBtn.onClick.AddListener(delegate
            {
                string filePath = fileOpenDialog.FileOpen(FileOpenDialog.Type.Music);
                if (!string.IsNullOrEmpty(filePath))
                {
                    StartCoroutine(SetAudioCilpUsingWebRequest(filePath));
                }
            });
            okayBtn.onClick.AddListener(UploadAndFinish);

            OnUploaded += AfterUpload;

        }
    }
    void AfterUpload(bool success)
    {
        if (success)
        {
            Close();
        }
        else
        {

        }
    }
    void UploadAndFinish()
    {
        if (musicControllerMini.audioClip != null && infoInputs[0].text.Length>0)
        {
            Music music = new Music();
            music.title = infoInputs[0].text;
            music.userID = UserData.Instance.id;
            music.userNickname = UserData.Instance.user.nickname;
            music.lyrics= infoInputs[1].text;
            music.info = infoInputs[2].text;

            foreach(Toggle tg in toggleGroup.ActiveToggles())
            {
                music.category = tg.GetComponentInChildren<TextMeshProUGUI>().text;
            }

            Debug.Log(music.ToString());
            StartCoroutine(Upload(musicBytes, imageBytes, music, localFileName.text));
            //Close();
        }
        else
        {
            Debug.Log("입력한 파일과 내용을 확인하세요");
        }

    }
    override public void Reset()
    {
        Init();

        errorText.text = "";
        musicBytes = null;
        imageBytes = null;
        infos = new string[0];
        for (int i = 0; i < infoInputs.Length; i++)
        {
            infoInputs[i].text = "";
        }
        localFileName.text = "파일 없음";

        toggleGroup.transform.GetChild(0).GetComponent<Toggle>().isOn=true;
        songImage.sprite = null;
        songImage.color = new Color(255, 255, 255, 0);

        musicControllerMini.Reset();
    }
    private void LoadImage(string filePath)
    {
        imageBytes = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(0, 0);

        texture.LoadImage(imageBytes);
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        songImage.sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
        songImage.color = new Color(255, 255, 255, 1);

    }
    IEnumerator SetAudioCilpUsingWebRequest(string _filePath)
    {
        AudioType aType = AudioType.UNKNOWN;

        string type = _filePath.Substring(_filePath.Length - 3);
        if (type == "wav")
        {
            aType = AudioType.WAV;
        }
        else if (type == "mp3")
        {
            aType = AudioType.MPEG;
        }
        else if (type == "ogg")
        {
            aType = AudioType.OGGVORBIS;
        }
        Debug.Log(aType);
        
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(_filePath, aType))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                yield break;
            }


            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            if (clip.length>10)
            {
                errorText.text = "";
                musicBytes = File.ReadAllBytes(_filePath);
                localFileName.text = _filePath;
                musicControllerMini.SetAudioClip(clip);
            }
            else
            {
                errorText.text = "파일이 업로드되지 않았습니다. (권장 파일 : wav, ogg)";
                musicBytes = new byte[0];
                localFileName.text = "파일 없음";
                musicControllerMini.SetAudioClip(null);
            }
            




        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
