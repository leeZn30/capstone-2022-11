using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.Networking;

public class AddPageInSongPage : Page
{
    public Button imageUploadBtn;
    public Button musicUploadBtn;
    public Button okayBtn;
    public FileOpenDialog fileOpenDialog;
    
    public Image songImage;
    public TextMeshProUGUI localFileName;

    public MusicControllerMini musicControllerMini;

    string filePath;//뮤직경ㅇ로
    byte[] musicBytes;
    byte[] imageBytes;
    string[] infos;//뮤직인포 배열 (순서는 enum과 같음(하드코딩))
    private TMP_InputField[] infoInputs;

    enum MusicInfo
    {
        Title,Artist,Content
    }
    void Start()
    {
        Init();
    }
    public override void Init()
    {
        if (isAlreadyInit == false)
        {
            infoInputs = GetComponentsInChildren<TMP_InputField>();
            imageUploadBtn.onClick.AddListener(delegate
            {
                string filePath2 = fileOpenDialog.FileOpen(FileOpenDialog.Type.Image);
                if (!string.IsNullOrEmpty(filePath2))
                {
                    imageBytes = File.ReadAllBytes(filePath2);
                    LoadImage(imageBytes);
                    Debug.Log(filePath2);
                }
            });
            musicUploadBtn.onClick.AddListener(delegate
            {
                filePath = fileOpenDialog.FileOpen(FileOpenDialog.Type.Music);
                if (!string.IsNullOrEmpty(filePath))
                {
                    musicBytes = File.ReadAllBytes(filePath);
                    localFileName.text = filePath;
                    StartCoroutine("SetAudioCilpUsingWebRequest");
                }
            });
            okayBtn.onClick.AddListener(UploadAndFinish);
            isAlreadyInit = true;
        }
    }
    void UploadAndFinish()
    {
        MusicUpload.Instance.FileUpload(musicBytes, filePath);
        Close();
    }
    override public void Reset()
    {
        //초기화
        filePath = "";
        musicBytes = new byte[0];
        imageBytes = new byte[0];
        infos = new string[0];
        localFileName.text = "파일 명";
    }
    private void LoadImage(byte[] byteTexture)
    {

        Texture2D texture = new Texture2D(0, 0);

        texture.LoadImage(byteTexture);
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        songImage.sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
        songImage.color = new Color(255, 255, 255, 1);

    }
    IEnumerator SetAudioCilpUsingWebRequest()
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {

                musicControllerMini.SetAudioClip(DownloadHandlerAudioClip.GetContent(www));
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
