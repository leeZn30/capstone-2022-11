using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using LitJson;
using Cysharp.Threading.Tasks;
using System;

public class AudioClipPlay
{
    public AudioClip audioClip;
    public bool play;

    public AudioClipPlay(AudioClip a, bool play)
    {
        this.audioClip = a;
        this.play = play;
    }
}
public class MusicID
{
    public string musicId;
}
public class MusicIDListName
{
    public string musicId;
    public string listName;
}
public class StringList
{
    public List<string> stringList;
}
public class MusicIDListNameList
{
    public string listName;
    public List<string> musicList;
}
public class MusicIDList
{
    public List<string> musicList;
}
public class MusicList
{
    public List<Music> musicList;
    public bool play = false;

    public MusicList(List<Music> musicList, bool play=false)
    {
        this.musicList = musicList;
        this.play = play;
    }
}
public class UserList
{
    public List<User> userList;
    public UserList(List<User> userList)
    {
        this.userList = userList;
    }
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
public class MusicLocate
{
    public string locate;
    public string imageLocate;
}
public class MusicArtist
{
    public string artist;
}
public class MusicCategory
{
    public string category;
}
public class UserID
{
    public string userId;
}
public class UserNickName
{
    public string userNickname;
}
public class UserNameId
{
    public string userId;
    public string userNickname;
}

public class fp
{
    public string filepath;
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
    public float time;
    public string GetArtistName()
    {
        return userNickname + "(" + userID + ")";
    }
    override public string ToString()
    {
        return locate + " " + imageLocate + " " + title + " " + id + " " + userID + " " + userNickname + " " + category + " " + lyrics + " " + info;
    }
}
public class TokenExpirationException : Exception
{
    public TokenExpirationException() 
    {

    }
    public TokenExpirationException(string message) :base(message)
    {

    }
}
public class MusicWebRequest : MonoBehaviour
{
    protected string url = GlobalData.url;


    protected bool getAudioStopFlag=false; 


    protected delegate void MusicHandler(AudioClip audioClip, bool play);//play- 바로 재생할것인지
    protected event MusicHandler OnGetClip;


    protected delegate void CharacterHandler(int character);//play- 바로 재생할것인지
    protected event CharacterHandler ModifyCharacter;

    protected delegate void UploadHandler(bool success);
    protected event UploadHandler OnUploaded;
    /*
    public static AudioClip FromPcmBytes(byte[] bytes, string clipName = "pcm")
    {
        //clipName.ThrowIfNullOrWhitespace(nameof(clipName));
        var pcmData = PcmData.FromBytes(bytes);
        var audioClip = AudioClip.Create(clipName, pcmData.Length, pcmData.Channels, pcmData.SampleRate, false);
        audioClip.SetData(pcmData.Value, 0);
        return audioClip;
    }
    */
    
    protected IEnumerator POST_DeleteMusic(MusicID id, string listName)
    {
       
        MusicIDListName idname = new MusicIDListName();
        idname.musicId = id.musicId;
        idname.listName = listName;
        string json = JsonUtility.ToJson(idname);
        using (UnityWebRequest request = UnityWebRequest.Post(url + "/api/user/deleteSong", json))
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


            }
            else
            {
                Debug.Log(request.error.ToString());

            }
        }
    }
    //버킷에서 음원 삭제
    protected IEnumerator POST_DeleteFromBucket(string fileName,string imgName)
    {

        MusicLocate musicLocate = new MusicLocate();
        musicLocate.locate = fileName;
        musicLocate.imageLocate = imgName;

        string json = JsonUtility.ToJson(musicLocate);
        using (UnityWebRequest request = UnityWebRequest.Post(url + "/media/delete" , json))
        {// 보낼 주소와 데이터 입력

            request.uploadHandler = new UploadHandlerRaw(new System.Text.UTF8Encoding().GetBytes(json));
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();//결과 응답이 올 때까지 기다리기

            if (request.error == null)
            {



            }
            else
            {
                Debug.Log(request.error.ToString());

            }
        }
    }


    //음원 담기
    protected IEnumerator POST_AddMusic(MusicIDList idList, string listName="myList")
    {

        MusicIDListNameList idname = new MusicIDListNameList();
        idname.musicList = idList.musicList;
        idname.listName = listName;

        string json = JsonUtility.ToJson(idname);
        using (UnityWebRequest request = UnityWebRequest.Post(url + "/api/user/addsong", json))
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
        using (UnityWebRequest request = UnityWebRequest.Post(url + "/api/music", json))
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

                //현재 재생목록이 uploadList라면 추가
                List<Music> ms = new List<Music>();
                ms.Add(_music);
                MusicController.Instance.AddNewMusics("uploadList", ms);

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
        using (UnityWebRequest request = UnityWebRequest.Post(url + "/api/user/modifiedChar", json))
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
    protected async UniTask<MusicList> GET_MusicListAsync(string listName, bool play = false, string _userid = null)
    {
        try{ 
            string json = "";
            string resultUrl = url + "/api/user/musicList";
            if (_userid != null)
            {
                UserID userID = new UserID();
                userID.userId = _userid;

                json = JsonUtility.ToJson(userID);
                resultUrl = url + "/api/music/" + listName;
            }
            else
            {
                json="{\"listName\":\""+listName+"\"}";
            }
            Debug.Log(json);
            using (UnityWebRequest www = UnityWebRequest.Get(resultUrl))
            {
                www.SetRequestHeader("token", UserData.Instance.Token);
                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("accept", "text/plain");
                www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));

                await www.SendWebRequest();

                List<Music> musics = new List<Music>();
                if (www.error == null)
                {
                    if (www.isDone)
                    {
                        string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                        Debug.Log(jsonResult);
                        JsonData jsonData = JsonToObject(jsonResult);

                        if (_userid != null)
                        {
                          
                            jsonData = jsonData[listName];
                        }
                        else
                        {
                          
                            jsonData = jsonData["music"];
                        }

                        for (int i = 0; i < jsonData.Count; i++)
                        {
                            Music music = new Music();

                            music.title = (string)jsonData[i]["title"];
                            music.id = (string)jsonData[i]["id"];
                            music.locate = (string)jsonData[i]["locate"];
                            music.imageLocate = (string)jsonData[i]["imageLocate"];
                            music.userID = (string)jsonData[i]["userID"];
                            music.userNickname = (string)jsonData[i]["userNickname"];
                            music.category = (string)jsonData[i]["category"];
                            music.lyrics = (string)jsonData[i]["lyrics"];
                            music.info = (string)jsonData[i]["info"];
                            music.time = (float)(double)jsonData[i]["time"];
                            musics.Add(music);

                        }
                    }

                    return new MusicList(musics, play);
                    //OnGetSongList(musics, play);
                    //Debug.Log("done");

                }
                else
                {
                    Debug.Log(www.error.ToString());
                    return null;
                }
            }
        }
        catch (ArgumentNullException e )
        {
            Debug.Log("search 요청 취소됨");
            return null;
        }
        catch (UnityWebRequestException e)
        {
            if (e.ResponseCode == 400)
            {
                Debug.Log(e.ResponseCode+" [GET_MusicListAsync] 토큰 만료");
                ErrorPopup.Instance.Open(0);
            }
            else if (e.ResponseCode == 0)
            {
                Debug.Log("서버와 연결 끊어짐");
                ErrorPopup.Instance.Open(1);
            }
            else
            {
                Debug.Log(e);
            }
            return null;
        }
        catch (Exception e)
        { 
            Debug.LogError( e);
            return null;
        }



    }
    protected UnityWebRequest getAudioWWW;
    protected async UniTask<AudioClipPlay> GetAudioClipAsync(string _filePath, bool play)
    {
        try
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
           
            using (getAudioWWW = UnityWebRequestMultimedia.GetAudioClip("https://"+_filePath, audioType))
            {
                Debug.Log("get audio " + _filePath + audioType.ToString());

                await getAudioWWW.SendWebRequest();// Unity의 Async Operation 이라 await 가능하다.

                if (getAudioWWW.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(getAudioWWW.error);
                    return null;
                }
                else
                {
                    return new AudioClipPlay(DownloadHandlerAudioClip.GetContent(getAudioWWW),play);
                    //OnGetClip(DownloadHandlerAudioClip.GetContent(www), play);
                }
            }
        }
        catch (ArgumentNullException e)
        {
            Debug.Log("get audio 요청 취소됨");
            return null;
        }
        catch (UnityWebRequestException e)
        {
            if (e.ResponseCode == 400)
            {
                ErrorPopup.Instance.Open(0);
                Debug.Log(e.ResponseCode+"[GetAudioClipAsync] 토큰 만료");
            }
            else if (e.ResponseCode == 0)
            {
                Debug.Log("서버와 연결 끊어짐");
                ErrorPopup.Instance.Open(1);
            }
            else
            {
                Debug.Log(e);
            }
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    protected async UniTask<MusicList> GET_SearchMusicTitleAsync(string type, string value)
    {
        try
        {
            string json = "";
            if (type=="artist")
            {
                MusicArtist musicArtist = new MusicArtist();
                musicArtist.artist = value;

                json = JsonUtility.ToJson(musicArtist);
            }
            else if (type=="category")
            {
                MusicCategory musicCategory = new MusicCategory();
                musicCategory.category = value;

                json = JsonUtility.ToJson(musicCategory);

            }
            else
            {//title
                MusicTitle musicTitle = new MusicTitle();
                musicTitle.title = value;

                json = JsonUtility.ToJson(musicTitle);
            }

            Debug.Log("곡 검색 json: " + json);

            using (UnityWebRequest www = UnityWebRequest.Get(url + "/api/music/" + type))
            {

                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("accept", "text/plain");
                www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));

                await www.SendWebRequest();

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
                            music.userNickname = (string)jsonData[i]["userNickname"];
                            music.category = (string)jsonData[i]["category"];
                            music.lyrics = (string)jsonData[i]["lyrics"];
                            music.info = (string)jsonData[i]["info"];

                            musics.Add(music);

                        }
                    }

                    Debug.Log("done");
                    return new MusicList(musics);

                }
                else
                {
                    Debug.Log(www.error.ToString());
                    return null;
                }
            }
        }
        catch (ArgumentNullException e)
        {
            Debug.Log("search 요청 취소됨");
            return null;
        }
        catch (UnityWebRequestException e)
        {
            if (e.ResponseCode == 400)
            {
                ErrorPopup.Instance.Open(0);
                Debug.Log(e.ResponseCode+" [GET_SearchMusicTitleAsync] 토큰 만료");
            }
            else if (e.ResponseCode == 0)
            {
                Debug.Log("서버와 연결 끊어짐");
                ErrorPopup.Instance.Open(1);
            }
            else
            {
                Debug.Log(e);
            }
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }
    protected async UniTask<MusicList> GET_SpecificMusicListAsync(SpecificMusic type)
    {
        try
        {
            //type은 recent, personalGenre, popular 있음
            using (UnityWebRequest www = UnityWebRequest.Get(url + "/api/music/" + type.ToString()))
            {
                if(type==SpecificMusic.personalGenre)
                    www.SetRequestHeader("token", UserData.Instance.Token);
                //www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));

                await www.SendWebRequest();

                List<Music> musics = new List<Music>();
                if (www.error == null)
                {
                    if (www.isDone)
                    {

                        string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                        

                        JsonData jsonData2 = JsonToObject(jsonResult);
                        JsonData jsonData = jsonData2[type.ToString()];

                        for (int i = 0; i < jsonData.Count; i++)
                        {
                            Music music = new Music();

                            music.title = (string)jsonData[i]["title"];
                            music.id = (string)jsonData[i]["id"];
                            music.locate = (string)jsonData[i]["locate"];
                            music.imageLocate = (string)jsonData[i]["imageLocate"];
                            music.userID = (string)jsonData[i]["userID"];
                            music.userNickname = (string)jsonData[i]["userNickname"];
                            music.category = (string)jsonData[i]["category"];
                            music.lyrics = (string)jsonData[i]["lyrics"];
                            music.info = (string)jsonData[i]["info"];
                            musics.Add(music);

                        }
                    }
                   
                    Debug.Log("done");
                    return new MusicList(musics);
               

                }
                else
                {
                    Debug.Log(www.error.ToString());
                    return null;
                }
            }           
        }
        catch (ArgumentNullException e)
        {
            Debug.Log("get "+type+" List 요청 취소됨");
            return null;
        }
        catch (UnityWebRequestException e)
        {
            if (e.ResponseCode == 400)
            {
                ErrorPopup.Instance.Open(0);
                Debug.Log(e.ResponseCode+"[GET_SpecificMusicListAsync] 토큰 만료");
            }
            else if (e.ResponseCode == 0)
            {
                Debug.Log("서버와 연결 끊어짐");
                ErrorPopup.Instance.Open(1);
            }
            else
            {
                Debug.Log(e);
            }
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }
    protected async UniTask<UserList> GET_FollowSystemUserListAsync(FollowPage.FollowSystemType ft)
    {
        try
        {

            using (UnityWebRequest www = UnityWebRequest.Get(url + "/api/follow" + (ft== FollowPage.FollowSystemType.follower ? "/follower" : ""))) 
            {
                
                www.SetRequestHeader("token", UserData.Instance.Token);

                await www.SendWebRequest();

                List<User> users = new List<User>();
                if (www.error == null)
                {
                    if (www.isDone)
                    {

                        string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);

                        Debug.Log(jsonResult);
                        JsonData jsonData2 = JsonToObject(jsonResult);
                        JsonData jsonData = jsonData2[ft.ToString()];

                        for (int i = 0; i < jsonData.Count; i++)
                        {
                            User user = new User();

                            user.id = (string)jsonData[i][0];
                            user.nickname= (string)jsonData[i][1];

                            users.Add(user);

                        }
                    }


                    return new UserList(users);


                }
                else
                {
                    Debug.Log(www.error.ToString());
                    return null;
                }
            }
        }
        catch (ArgumentNullException e)
        {
            Debug.Log("get "  + " List 요청 취소됨");
            return null;
        }
        catch (UnityWebRequestException e)
        {
            if (e.ResponseCode == 400 || e.ResponseCode == 401)
            { 
                ErrorPopup.Instance.Open(0);
                Debug.Log(e.ResponseCode + "[GET_FollowSystemUserListAsync] 토큰 만료");
            }
            else if (e.ResponseCode == 0)
            {
                Debug.Log("서버와 연결 끊어짐");
                ErrorPopup.Instance.Open(1);
            }
            else
            {
                Debug.Log(e);
            }
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }
    protected async UniTask<UserList> GET_SearchUserAsync(string value)
    {
        try
        {
            UserNickName userNickName = new UserNickName();
            userNickName.userNickname = value;

            string json = "";
            json = JsonUtility.ToJson(userNickName);

            Debug.Log("곡 검색 json: " + json);

            using (UnityWebRequest www = UnityWebRequest.Get(url + "/api/user/search"))
            {
                www.SetRequestHeader("token", UserData.Instance.Token);
                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("accept", "text/plain");
                www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));

                await www.SendWebRequest();

                List<User> users = new List<User>();
                if (www.error == null)
                {
                    if (www.isDone)
                    {
                        string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                        Debug.Log("결과 " + jsonResult);
                        
                        JsonData jsonData = JsonToObject(jsonResult)["user"];



                        for (int i = 0; i < jsonData.Count; i++)
                        {
                            User user = new User();
                            user.id = (string)jsonData[i]["id"];
                            user.nickname = (string)jsonData[i]["nickname"];
                            user.character = (int)jsonData[i]["character"];
                            user.followNum = (int)jsonData[i]["followNum"];
                            user.followerNum = (int)jsonData[i]["followerNum"];

                            users.Add(user);

                        }
                        
                    }
                    Debug.Log("done");
                    return new UserList(users);

                }
                else
                {
                    Debug.Log(www.error.ToString());
                    return null;
                }
            }
        }
        catch (ArgumentNullException e)
        {
            Debug.Log("search 요청 취소됨");
            return null;
        }
        catch (UnityWebRequestException e)
        {
            if (e.ResponseCode == 400)
            {
                ErrorPopup.Instance.Open(0);
                Debug.Log(e.ResponseCode + "[ GET_SearchUserAsync] 토큰 만료");
            }
            else if (e.ResponseCode == 0)
            {
                Debug.Log("서버와 연결 끊어짐");
                ErrorPopup.Instance.Open(1);
            }
            else
            {
                Debug.Log(e);
            }
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }
    protected async UniTask<StringList> POST_FollowUserAsync(string userID,string userName, bool isDelete=false)
    {
        try
        {
            UserNameId uni = new UserNameId();

            uni.userId = userID;
            uni.userNickname = userName;

            string json = "";
            json = JsonUtility.ToJson(uni);

            Debug.Log("follow "+isDelete+ " json: " + json);

            using (UnityWebRequest www = UnityWebRequest.Post(url + "/api/follow" + (isDelete? "/delete" : ""),json))
            {

                www.SetRequestHeader("token", UserData.Instance.Token);
                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("accept", "text/plain");
                www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));

                await www.SendWebRequest();
                StringList sl = new StringList();
                if (www.error == null)
                {
                    List<String> strList = new List<string>();
                    Debug.Log("done"); 
                    string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    JsonData jsonData = JsonToObject(jsonResult)["follow"];
                    Debug.Log(jsonResult);
                    foreach (JsonData j in jsonData)
                    {
                        strList.Add((string)(j[0]));
                    }
                    sl.stringList = strList;
                    return sl;

                }
                else
                {
                    Debug.Log(www.error.ToString());
                    return null;
                }
            }
        }
        catch (ArgumentNullException e)
        {
            Debug.Log("follow 요청 취소됨");
            return null;
        }
        catch (UnityWebRequestException e)
        {
            if (e.ResponseCode == 400 || e.ResponseCode == 401)
            {
                ErrorPopup.Instance.Open(0);
                Debug.Log(e.ResponseCode + "[POST_FollowUserAsync] 토큰 만료");
            }
            else if (e.ResponseCode == 450)
            {
                //이미 팔로우한 유저

            }
            else if (e.ResponseCode == 0)
            {
                Debug.Log("서버와 연결 끊어짐");
                ErrorPopup.Instance.Open(1);
            }
            else
            {
                Debug.Log(e);
            }
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }
    protected async UniTask<User> GET_UserInfoAsync(string userId)
    {
        try
        {
            UserID us = new UserID();
            us.userId = userId;

            string json = "";
            json = JsonUtility.ToJson(us);

            Debug.Log("유저 정보 get json: " + json);

            using (UnityWebRequest www = UnityWebRequest.Get(GlobalData.url + "/api/user/info"))
            {

                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("accept", "text/plain");
                www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));

                await www.SendWebRequest();

                if (www.error == null)
                {
                    if (www.isDone)
                    {
                        string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                        Debug.Log("결과 " + jsonResult);
                        JsonData jsonData = JsonToObject(jsonResult)["user"];

                        User user = new User();

                        user.character = (int)(jsonData["character"]);
                        user.id = (string)(jsonData["id"]);
                        user.nickname = (string)(jsonData["nickname"]);

                        user.followerNum = (int)(jsonData["followerNum"]);
                        user.followNum = (int)(jsonData["followNum"]);

                        if (userId == UserData.Instance.user.id)
                        {//본인 정보를 받아올 때만 선호장르와 팔로우 받기
                            user.preferredGenres = new List<string>();
                            user.follow = new List<string>();

                            foreach (JsonData genre in jsonData["preferredGenres"])
                            {
                                user.preferredGenres.Add((string)genre);
                            }
                            foreach (JsonData f in jsonData["follow"])
                            {
                                user.follow.Add((string)f[0]);
                            }
                        }


                        return user;
                    }
                }
                else
                {
                    Debug.Log(www.error.ToString());
                }
                return null;
            }
        }
        catch (UnityWebRequestException e)
        {
            if (e.ResponseCode == 400)
            {
                ErrorPopup.Instance.Open(0);
                Debug.Log(e.ResponseCode + "[ GET_UserInfoAsync] 토큰 만료");

            }
            else if (e.ResponseCode == 0)
            {
                Debug.Log("서버와 연결 끊어짐");
                ErrorPopup.Instance.Open(1);
            }
            else
            {
                Debug.Log(e);
            }
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    
    protected async UniTask POST_AddPlayCount(string musicId)
    {
        try
        {
            MusicID mi = new MusicID();
            mi.musicId = musicId;

            string json = "";
            json = JsonUtility.ToJson(mi);

            Debug.Log(musicId + "재생 수 추가 요청");
            using (UnityWebRequest www = UnityWebRequest.Post(url + "/api/music/play", json))
            {

                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("accept", "text/plain");
                www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));

                await www.SendWebRequest();

                if (www.error == null)
                {

                }
                else
                {
                    Debug.Log(www.error.ToString());

                }
            }
        }
        catch (ArgumentNullException e)
        {
            Debug.Log("재생 증가 요청 취소됨");
        }
        catch (UnityWebRequestException e)
        {
            if (e.ResponseCode == 400 || e.ResponseCode == 401)
            {
                ErrorPopup.Instance.Open(0);
                Debug.Log(e.ResponseCode + "[POST_AddPlayCount] 토큰 만료");
            }
            else if (e.ResponseCode == 0)
            {
                Debug.Log("서버와 연결 끊어짐");
                ErrorPopup.Instance.Open(1);
            }
            else
            {
                Debug.Log(e);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    protected async UniTask<StringList> POST_MakeListAsync(string _listName)
    {
        try
        {
            string json = "{\"listName\":\""+_listName+"\"}";
            Debug.Log(json);
            using (UnityWebRequest www = UnityWebRequest.Post(url + "/api/user/makeList", json))
            {
                www.SetRequestHeader("token", UserData.Instance.Token);
                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("accept", "text/plain");
                www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));

                await www.SendWebRequest();
                if (www.error == null)
                {

                    string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);

                    JsonData jsonData = JsonToObject(jsonResult)["listName"];

                    StringList sl = new StringList();
                    List<string> ln = new List<string>();
                    foreach (JsonData jsonData1 in jsonData)
                    {
                        ln.Add((string)jsonData1);
                    }
                    sl.stringList = ln;

                    return sl;

                }
                else
                {
                    Debug.Log(www.error.ToString());
                    return null;
                }
            }
        }
        catch (ArgumentNullException e)
        {
            Debug.Log(""); return null;
        }
        catch (UnityWebRequestException e)
        {
            if (e.ResponseCode == 400 || e.ResponseCode == 401)
            {
                ErrorPopup.Instance.Open(0);
                Debug.Log(e.ResponseCode + "[POST_MakeListAsync] 토큰 만료");

            }
            else if (e.ResponseCode == 0)
            {
                Debug.Log("서버와 연결 끊어짐");
                ErrorPopup.Instance.Open(1);
            }
            else
            {
                Debug.Log(e);
            }
            return null ;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }
    protected async UniTask<StringList> POST_DeleteListAsync(string _listName)
    {
        try
        {
            string json = "{\"listName\":\"" + _listName + "\"}";
            using (UnityWebRequest www = UnityWebRequest.Post(url + "/api/user/deleteList", json))
            {
                www.SetRequestHeader("token", UserData.Instance.Token);
                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("accept", "text/plain");
                www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));

                await www.SendWebRequest();
                if (www.error == null)
                {

                    string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    JsonData jsonData = JsonToObject(jsonResult)["listName"];

                    StringList sl = new StringList();
                    List<string> ln = new List<string>();
                    foreach (JsonData jsonData1 in jsonData)
                    {
                        ln.Add((string)jsonData1);
                    }
                    sl.stringList = ln;

                    return sl;

                }
                else
                {
                    Debug.Log(www.error.ToString());
                    return null;
                }
            }
        }
        catch (ArgumentNullException e)
        {
            Debug.Log(""); return null;
        }
        catch (UnityWebRequestException e)
        {
            if (e.ResponseCode == 400 || e.ResponseCode == 401)
            {
                ErrorPopup.Instance.Open(0);
                Debug.Log(e.ResponseCode + "[POST_DeleteListAsync] 토큰 만료");

            }
            else if (e.ResponseCode == 0)
            {
                Debug.Log("서버와 연결 끊어짐");
                ErrorPopup.Instance.Open(1);
            }
            else
            {
                Debug.Log(e);
            }
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }
    protected async UniTask<StringList> GET_ListNameAsync()
    {
        try
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url + "/api/user/musicListName"))
            {
                www.SetRequestHeader("token", UserData.Instance.Token);


                await www.SendWebRequest();
                if (www.error == null)
                {
                    string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);

                    JsonData jsonData = JsonToObject(jsonResult)["listName"];
                    Debug.Log("결과 " + jsonResult);

                    StringList sl = new StringList();
                    List<string> ln = new List<string>();
                    foreach (JsonData jsonData1 in jsonData)
                    {
                        ln.Add((string)jsonData1);
                    }
                    sl.stringList = ln;
                    return sl;

                }
                else
                {
                    Debug.Log(www.error.ToString());
                    return null;
                }
            }
        }
        catch (ArgumentNullException e)
        {
            Debug.Log(""); return null;
        }
        catch (UnityWebRequestException e)
        {
            if (e.ResponseCode == 400 || e.ResponseCode == 401)
            {
                ErrorPopup.Instance.Open(0);
                Debug.Log(e.ResponseCode + "[GET_ListNameAsync] 토큰 만료");

            }
            else if (e.ResponseCode == 0)
            {
                Debug.Log("서버와 연결 끊어짐");
                ErrorPopup.Instance.Open(1);
            }
            else
            {
                Debug.Log(e);
            }
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }
    //음원존 join (park scene MusicZoneUI에서 사용)
    protected async UniTask<MusicZoneOutputData> GET_JoinZone(MusicZoneNumber num)
    {
        try
        {
            string json = JsonUtility.ToJson(num);

            using (UnityWebRequest www = UnityWebRequest.Get(url + "/api/musiczone/joinZone"))
            {
                www.SetRequestHeader("token", UserData.Instance.Token);
                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("accept", "text/plain");
                www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));

                await www.SendWebRequest();

                MusicZoneOutputData mz = new MusicZoneOutputData();

                if (www.error == null)
                {
                    string jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    Debug.Log("결과 " + jsonResult);

                    JsonData jsonData = JsonToObject(jsonResult);

                    for (int i = 0; i < jsonData["title"].Count; i++)
                    {
                        mz.titleList.Add((string)jsonData["title"][i]);
                        mz.locateList.Add((string)jsonData["locate"][i]);

                    }
                    if (jsonData["time"].GetJsonType() == JsonType.Double)
                    {
                        mz.time = (float)(double)jsonData["time"];
                    }
                    else
                    {
                        mz.time = (float)(int)jsonData["time"];
                    }

                    return mz;
                }
                else
                {
                    Debug.Log(www.error.ToString());
                    return null;
                }
            }
        }
        catch (ArgumentNullException e)
        {
            Debug.Log(""); return null;
        }
        catch (UnityWebRequestException e)
        {
            if (e.ResponseCode == 400 || e.ResponseCode == 401)
            {
                ErrorPopup.Instance.Open(0);
                Debug.Log(e.ResponseCode + "[POST_DeleteListAsync] 토큰 만료");

            }
            else if (e.ResponseCode == 510)
            {
                Debug.Log("재생되는 음원 없음 : 재생 시간 초과");

            }
            else if (e.ResponseCode == 0)
            {
                Debug.Log("서버와 연결 끊어짐");
                ErrorPopup.Instance.Open(1);
            }
            else
            {
                Debug.Log(e);
            }
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }
    //음원존 create (park scene MusicZoneUI에서 사용)
    protected async UniTask<bool> POST_CreateZone(MusicZoneInputData mz)
    {
        try
        {
            string json = JsonUtility.ToJson(mz);

            using (UnityWebRequest www = UnityWebRequest.Post(url + "/api/musiczone/createZone", json))
            {
                www.SetRequestHeader("token", UserData.Instance.Token);
                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("accept", "text/plain");
                www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));

                await www.SendWebRequest();
                if (www.error == null)
                {
                    return true;
                }
                else
                {
                    Debug.Log(www.error.ToString());
                    return false;
                }
            }
        }
        catch (ArgumentNullException e)
        {
            Debug.Log(""); return false;
        }
        catch (UnityWebRequestException e)
        {
            if (e.ResponseCode == 400 || e.ResponseCode == 401)
            {
                ErrorPopup.Instance.Open(0);
                Debug.Log(e.ResponseCode + "[POST_DeleteListAsync] 토큰 만료");

            }
            else if (e.ResponseCode == 410)
            {
                Debug.Log("이미 만들어짐");

            }
            else if (e.ResponseCode == 0)
            {
                Debug.Log("서버와 연결 끊어짐");
                ErrorPopup.Instance.Open(1);
            }
            else
            {
                Debug.Log(e);
            }
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }
    JsonData JsonToObject(string json)
    {
        return JsonMapper.ToObject(json);
    }
}



