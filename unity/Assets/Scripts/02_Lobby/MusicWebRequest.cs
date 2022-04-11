using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using LitJson;
using NAudio;
using NAudio.Wave;

public class MusicIDList
{
    public List<string> musicList;
}
public class ModifiedChar
{
    public string id;
    public int value;
}
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
    public string imageLocate;
    public string title;
    public string id;
    public string userID;
    public string userNickname;
    public string category;
    public string lyrics;
    public string info;
    public string GetArtistName()
    {
        return userNickname + "(" + userID + ")";
    }
    override public string ToString()
    {
        return locate + " " + imageLocate + " " + title + " " + id + " " + userID + " " + userNickname + " " + category + " " + lyrics + " " + info;
    }
}
public class MusicWebRequest : MonoBehaviour
{
    protected string url = GlobalData.url;


    protected delegate void SongListHandler(List<Music> musics, bool play=false);
    protected event SongListHandler OnGetSongList;


    protected delegate void MusicHandler(AudioClip audioClip, bool play);//play- 바로 재생할것인지
    protected event MusicHandler OnGetClip;


    protected delegate void CharacterHandler(int character);//play- 바로 재생할것인지
    protected event CharacterHandler ModifyCharacter;

    protected delegate void UploadHandler(bool success);
    protected event UploadHandler OnUploaded;
    protected IEnumerator POST_AddMyList(MusicIDList idList, string listName="myList")
    {

        string json = JsonUtility.ToJson(idList);
        using (UnityWebRequest request = UnityWebRequest.Post(url + "/user/addMyList", json))
        {// 보낼 주소와 데이터 입력

            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("token", UserData.Instance.Token);
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();//결과 응답이 올 때까지 기다리기


            if (request.error == null)
            { 


            }
            else
            {
                Debug.Log(request.error);
            }
        }
    }
    private IEnumerator POST_MusicDB(Music _music)
    {

        string json = JsonUtility.ToJson(_music);
        using (UnityWebRequest request = UnityWebRequest.Post(url + "/music", json))
        {// 보낼 주소와 데이터 입력

            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("token", UserData.Instance.Token);
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();//결과 응답이 올 때까지 기다리기


            if (request.error == null)
            {
                OnUploaded(true);
                Debug.Log("업로드 !" + _music.title+_music.locate+_music.imageLocate) ;


            }
            else
            {
                OnUploaded(false);

            }
        }
    }
    protected IEnumerator POST_ModifiedChar(string _id, int _character)
    {
        ModifiedChar mo = new ModifiedChar//현재 inputfield에 작성된 값 클래스로 변환
        {
            id = _id,
            value = _character
        };
     


        string json = JsonUtility.ToJson(mo);
        using (UnityWebRequest request = UnityWebRequest.Post(url + "/auth/modifiedChar", json))
        {// 보낼 주소와 데이터 입력

            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("token", UserData.Instance.Token);
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();//결과 응답이 올 때까지 기다리기

            
            if (request.error == null)
            {

                Debug.Log(request.downloadHandler.text);

                ModifyCharacter(_character);

            }
            else
            {
                Debug.Log(request.error.ToString());

            }
        }
    }

    protected IEnumerator Upload(byte[] musicBytes, byte[] imageBytes, Music music, string fileName)
    {

           List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

           formData.Add(new MultipartFormFileSection(fileName, musicBytes));
           if (imageBytes != null)
               formData.Add(new MultipartFormFileSection(music.title + ".png", imageBytes));


           using (UnityWebRequest request = UnityWebRequest.Post(url + "/media", formData))
           {// 보낼 주소와 데이터 입력

                request.SetRequestHeader("token", UserData.Instance.Token);
                
            
             

            yield return request.SendWebRequest();//결과 응답이 올 때까지 기다리기
         
            if (request.error != null)
            {
                Debug.Log(request.error);
                OnUploaded(false);
            }
            else
            {
                
                string jsonResult = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
                JsonData jsonData = JsonToObject(jsonResult);
                Debug.Log(request.downloadHandler.text);
                //로딩애니메이션 종료
                music.locate = (string)jsonData["locate"];
                music.imageLocate= (string)jsonData["imageLocate"];

                StartCoroutine(POST_MusicDB(music));
            }
            
        }
    }
    protected IEnumerator GET_MusicList(string listName, string _userid, bool play=false)
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
                        music.imageLocate = (string)jsonData[i]["imagelocate"];
                        music.userNickname = (string)jsonData[i]["nickname"];

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
    protected IEnumerator GetAudioCilpUsingWebRequest(string _filePath, bool play)
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
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("https://" + _filePath, audioType))
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
    protected IEnumerator GET_SearchMusicTitle(string _title)
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
                        music.imageLocate = (string)jsonData[i]["imageLocate"];
                        music.userID = (string)jsonData[i]["userID"];
                        music.category = (string)jsonData[i]["category"];
                        music.lyrics = (string)jsonData[i]["lyrics"];
                        music.info = (string)jsonData[i]["info"];

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



