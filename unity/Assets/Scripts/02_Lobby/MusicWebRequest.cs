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
public class UserID
{
    public string id;
}
[System.Serializable]
public class Music
{
    public string locate;
    public string imagelocate;
    public string title;
    public string id;
    public string userID;
    public string nickname;
    public string category;
}
public class MusicWebRequest : MonoBehaviour
{
    protected string url = "http://localhost:8080/api";


    protected delegate void SongListHandler(List<Music> musics, bool play=false);
    protected event SongListHandler OnGetSongList;


    protected delegate void MusicHandler(AudioClip audioClip, bool play);//play- 바로 재생할것인지
    protected event MusicHandler OnGetClip;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void GetMusicList(string listName, string _userid, bool play=false)
    {
        StartCoroutine(GET_MusicList(listName, _userid, play));
    }
    public void SearchTitle(string _title)
    {
        StartCoroutine(GET_SearchMusicTitle(_title));
    }
    public void GetAudioClip(string filePath, bool play)
    {
        StartCoroutine(GetAudioCilpUsingWebRequest(filePath,play));
    }
    IEnumerator GET_MusicList(string listName, string _userid, bool play=false)
    {
        UserID userID= new UserID();
        userID.id = _userid;

        string json = JsonUtility.ToJson(userID);
        Debug.Log(listName+" 리스트: " + json);

        using (UnityWebRequest www = UnityWebRequest.Get(url + "/auth/"+listName))
        {
            www.SetRequestHeader("token",  UserData.Instance.Token);
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
                    Debug.Log("결과 " + jsonResult);
                    
                    JsonData jsonData2 = JsonToObject(jsonResult);
                    JsonData jsonData = jsonData2[listName];

                    for (int i = 0; i < jsonData.Count; i++)
                    {
                        Music music = new Music();

                        music.title = (string)jsonData[i]["title"];
                        music.id = (string)jsonData[i]["id"];
                        music.locate = (string)jsonData[i]["locate"];
                        music.userID = (string)jsonData[i]["userID"];
                        music.category = (string)jsonData[i]["category"];
                        music.imagelocate = (string)jsonData[i]["imagelocate"];
                        music.nickname = (string)jsonData[i]["nickname"];

                        musics.Add(music);

                    }
                }
                OnGetSongList(musics, play);
                Debug.Log("done");

            }
            else
            {
                Debug.Log(www.error.ToString());
            }
        }



    }
    IEnumerator GetAudioCilpUsingWebRequest(string _filePath, bool play)
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
        Debug.Log("get audio " +_filePath+audioType.ToString());
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url + _filePath, audioType))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                OnGetClip(DownloadHandlerAudioClip.GetContent(www),play);
            }
        }
    }
    IEnumerator GET_SearchMusicTitle(string _title)
    {
        MusicTitle musicTitle = new MusicTitle();
        musicTitle.title = _title;

        string json = JsonUtility.ToJson(musicTitle);
        Debug.Log("곡 검색 json: " + json);

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
                    Debug.Log("결과 " + jsonResult);
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
                OnGetSongList(musics);
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



