using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using LitJson;
using NAudio;
using NAudio.Wave;
public class MusicTitle
{
    public string title;
}
[System.Serializable]
public class Music
{
    public string locate;
    public string title;
    public string id;
    public string userID;
    public string category;
}
public class MusicWebRequest : Singleton<MusicWebRequest>
{
    string url = "http://localhost:8080";


    public delegate void SearchHandler(List<Music> musics);
    public event SearchHandler OnSearched;
    public event SearchHandler OnGetMusicList;

    public delegate void MusicHandler(AudioClip audioClip);
    public event MusicHandler OnGetClip;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void GetMusicList(string _title)
    {
        StartCoroutine(GET_MusicList(_title));
    }
    public void SearchTitle(string _title)
    {
        StartCoroutine(GET_SearchMusicTitle(_title));
    }
    public void GetAudioClip(string filePath)
    {
        StartCoroutine(GetAudioCilpUsingWebRequest(filePath));
    }
    IEnumerator GET_MusicList(string _title)
    {
        MusicTitle musicTitle = new MusicTitle();
        musicTitle.title = _title;

        string json = JsonUtility.ToJson(musicTitle);
        Debug.Log("°î °Ë»ö json: " + json);

        using (UnityWebRequest www = UnityWebRequest.Get(url + "/music/"))
        {

            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("accept", "text/plain");
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));

            yield return www.SendWebRequest();

            List<Music> musics = new List<Music>();
            if (www.error == null)
            {
                if (www.isDone)
                {
                    string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    Debug.Log("°á°ú " + jsonResult);
                    JsonData jsonData = JsonToObject(jsonResult);


                    for (int i = 0; i < jsonData.Count; i++)
                    {
                        Music music = new Music();

                        music.title = (string)jsonData[i]["title"];
                        music.id = (string)jsonData[i]["id"];
                        music.locate = (string)jsonData[i]["locate"];
                        music.userID = (string)jsonData[i]["userID"];
                        music.category = (string)jsonData[i]["category"];

                        musics.Add(music);

                    }
                }
                OnGetMusicList(musics);
                Debug.Log("done");

            }
            else
            {
                Debug.Log(www.error.ToString());
            }
        }



    }
    IEnumerator GetAudioCilpUsingWebRequest(string _filePath)
    {
        AudioType audioType = AudioType.MPEG;

        string type = _filePath.Substring(_filePath.Length - 3);
        if (type == "wav")
        {
            audioType = AudioType.WAV;
        }
        else if (type == "mp3")
        {


            audioType = AudioType.MPEG;
        }
        else if (type == "ogg")
        {
            audioType = AudioType.OGGVORBIS;
        }
        Debug.Log(audioType.ToString());
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url + _filePath, audioType))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                OnGetClip(DownloadHandlerAudioClip.GetContent(www));
            }
        }
    }
    IEnumerator GET_SearchMusicTitle(string _title)
    {
        MusicTitle musicTitle = new MusicTitle();
        musicTitle.title = _title;

        string json = JsonUtility.ToJson(musicTitle);
        Debug.Log("°î °Ë»ö json: " + json);

        using (UnityWebRequest www = UnityWebRequest.Get(url + "/music/"))
        {

            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("accept", "text/plain");
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));

            yield return www.SendWebRequest();

            List<Music> musics = new List<Music>();
            if (www.error == null)
            {
                if (www.isDone)
                {
                    string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    Debug.Log("°á°ú " + jsonResult);
                    JsonData jsonData = JsonToObject(jsonResult);


                    for (int i = 0; i < jsonData.Count; i++)
                    {
                        Music music = new Music();

                        music.title = (string)jsonData[i]["title"];
                        music.id = (string)jsonData[i]["id"];
                        music.locate = (string)jsonData[i]["locate"];
                        music.userID = (string)jsonData[i]["userID"];
                        music.category = (string)jsonData[i]["category"];

                        musics.Add(music);

                    }
                }
                OnSearched(musics);
                Debug.Log("done");

            }
            else
            {
                Debug.Log(www.error.ToString());
            }
        }



    }
    JsonData JsonToObject(string json)
    {
        return JsonMapper.ToObject(json);
    }
}



