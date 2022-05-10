using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using LitJson;
using NAudio;
using NAudio.Wave;
using Cysharp.Threading.Tasks;
using System;
//using System.IO;
//using System;
/*
public class PcmHeader
{
    #region Public types & data

    public int BitDepth { get; }
    public int AudioSampleSize { get; }
    public int AudioSampleCount { get; }
    public ushort Channels { get; }
    public int SampleRate { get; }
    public int AudioStartIndex { get; }
    public int ByteRate { get; }
    public ushort BlockAlign { get; }

    #endregion

    #region Constructors & Finalizer

    private PcmHeader(int bitDepth,
        int audioSize,
        int audioStartIndex,
        ushort channels,
        int sampleRate,
        int byteRate,
        ushort blockAlign)
    {
        BitDepth = bitDepth;
        _negativeDepth = Mathf.Pow(2f, BitDepth - 1f);
        _positiveDepth = _negativeDepth - 1f;

        AudioSampleSize = bitDepth / 8;
        AudioSampleCount = Mathf.FloorToInt(audioSize / (float)AudioSampleSize);
        AudioStartIndex = audioStartIndex;

        Channels = channels;
        SampleRate = sampleRate;
        ByteRate = byteRate;
        BlockAlign = blockAlign;
    }

    #endregion

    #region Public Methods

    public static PcmHeader FromBytes(byte[] pcmBytes)
    {
        using var memoryStream = new MemoryStream(pcmBytes);
        return FromStream(memoryStream);
    }

    public static PcmHeader FromStream(Stream pcmStream)
    {
        pcmStream.Position = SizeIndex;
        using BinaryReader reader = new BinaryReader(pcmStream);

        int headerSize = reader.ReadInt32();  // 16
        ushort audioFormatCode = reader.ReadUInt16(); // 20

        string audioFormat = GetAudioFormatFromCode(audioFormatCode);
        if (audioFormatCode != 1 && audioFormatCode == 65534)
        {
            // Only uncompressed PCM wav files are supported.
            throw new ArgumentOutOfRangeException(nameof(pcmStream),pcmStream,
                                                  $"Detected format code '{audioFormatCode}' {audioFormat}, but only PCM and WaveFormatExtensible uncompressed formats are currently supported.");
        }

        ushort channelCount = reader.ReadUInt16(); // 22
        int sampleRate = reader.ReadInt32();  // 24
        int byteRate = reader.ReadInt32();  // 28
        ushort blockAlign = reader.ReadUInt16(); // 32
        ushort bitDepth = reader.ReadUInt16(); //34

        pcmStream.Position = SizeIndex + headerSize + 2 * sizeof(int); // Header end index
        int audioSize = reader.ReadInt32();                            // Audio size index

        return new PcmHeader(bitDepth, audioSize, (int)pcmStream.Position, channelCount, sampleRate, byteRate, blockAlign); // audio start index
    }

    public float NormalizeSample(float rawSample)
    {
        float sampleDepth = rawSample < 0 ? _negativeDepth : _positiveDepth;
        return rawSample / sampleDepth;
    }

    #endregion

    #region Private Methods

    private static string GetAudioFormatFromCode(ushort code)
    {
        switch (code)
        {
            case 1: return "PCM";
            case 2: return "ADPCM";
            case 3: return "IEEE";
            case 7: return "?-law";
            case 65534: return "WaveFormatExtensible";
            default: throw new ArgumentOutOfRangeException(nameof(code), code, "Unknown wav code format.");
        }
    }

    #endregion

    #region Private types & Data

    private const int SizeIndex = 16;

    private readonly float _positiveDepth;
    private readonly float _negativeDepth;

    #endregion
}
public struct PcmData
{
    #region Public types & data

    public float[] Value { get; }
    public int Length { get; }
    public int Channels { get; }
    public int SampleRate { get; }

    #endregion

    #region Constructors & Finalizer

    private PcmData(float[] value, int channels, int sampleRate)
    {
        Value = value;
        Length = value.Length;
        Channels = channels;
        SampleRate = sampleRate;
    }

    #endregion

    #region Public Methods

    public static PcmData FromBytes(byte[] bytes)
    {
        if (bytes == null)
        {
            throw new ArgumentNullException(nameof(bytes));
        }

        PcmHeader pcmHeader = PcmHeader.FromBytes(bytes);
        if (pcmHeader.BitDepth != 16 && pcmHeader.BitDepth != 32 && pcmHeader.BitDepth != 8)
        {
            throw new ArgumentOutOfRangeException(nameof(pcmHeader.BitDepth), pcmHeader.BitDepth, "Supported values are: 8, 16, 32");
        }

        float[] samples = new float[pcmHeader.AudioSampleCount];
        for (int i = 0; i < samples.Length; ++i)
        {
            int byteIndex = pcmHeader.AudioStartIndex + i * pcmHeader.AudioSampleSize;
            float rawSample;
            switch (pcmHeader.BitDepth)
            {
                case 8:
                    rawSample = bytes[byteIndex];
                    break;

                case 16:
                    rawSample = BitConverter.ToInt16(bytes, byteIndex);
                    break;

                case 32:
                    rawSample = BitConverter.ToInt32(bytes, byteIndex);
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(pcmHeader.BitDepth), pcmHeader.BitDepth, "Supported values are: 8, 16, 32");
            }

            samples[i] = pcmHeader.NormalizeSample(rawSample); // normalize sample between [-1f, 1f]
        }

        return new PcmData(samples, pcmHeader.Channels, pcmHeader.SampleRate);
    }

    #endregion
}
*/
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
    
    protected IEnumerator POST_Delete(MusicID id, string listName)
    {


        string json = JsonUtility.ToJson(id);
        using (UnityWebRequest request = UnityWebRequest.Post(url + "/user/delete"+listName, json))
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
    protected IEnumerator POST_AddMyList(MusicIDList idList, string listName="myList")
    {

        string json = JsonUtility.ToJson(idList);
        using (UnityWebRequest request = UnityWebRequest.Post(url + "/user/add"+listName, json))
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
        using (UnityWebRequest request = UnityWebRequest.Post(url + "/user/modifiedChar", json))
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
            string resultUrl = url + "/user/" + listName;
            if (_userid != null)
            {
                UserID userID = new UserID();
                userID.userId = _userid;

                json = JsonUtility.ToJson(userID);
                resultUrl = url + "/music/" + listName;
            }

            Debug.Log(listName + " 리스트: " + json);

            using (UnityWebRequest www = UnityWebRequest.Get(resultUrl))
            {
                www.SetRequestHeader("token", UserData.Instance.Token);
                if (_userid != null)
                {
                    www.SetRequestHeader("Content-Type", "application/json");
                    www.SetRequestHeader("accept", "text/plain");
                    www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
                }

                await www.SendWebRequest();

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
                            music.imageLocate = (string)jsonData[i]["imageLocate"];
                            music.userID = (string)jsonData[i]["userID"];
                            music.userNickname = (string)jsonData[i]["userNickname"];
                            music.category = (string)jsonData[i]["category"];
                            music.lyrics = (string)jsonData[i]["lyrics"];
                            music.info = (string)jsonData[i]["info"];
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
                Popup.Instance.Open();
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
                //getAudioWWW.SendWebRequest();
                
                await getAudioWWW.SendWebRequest();// Unity의 Async Operation 이라 await 가능하다.
                //while (!getAudioWWW.isDone)
                //{
                    //yield return new WaitForSecondsRealtime(0.01f);
                //}
                // var responseString = res.downloadHandler.text;

                Debug.Log("get audio 끝" + _filePath + audioType.ToString());
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
                Popup.Instance.Open();
                Debug.Log(e.ResponseCode+"[GetAudioClipAsync] 토큰 만료");
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
    /*
    protected bool flag;

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

            using (getAudioWWW = UnityWebRequestMultimedia.GetAudioClip("https://" + _filePath, audioType))
            {
                Debug.Log("get audio " + _filePath + audioType.ToString());
            //getAudioWWW.SendWebRequest();
            flag = false;
                getAudioWWW.SendWebRequest();// Unity의 Async Operation 이라 await 가능하다.
                                                   while (!getAudioWWW.isDone)
                                                   {
                if (flag == true) yield break;
                                                   yield return new WaitForSecondsRealtime(0.01f);
                                                   }
                                                   // var responseString = res.downloadHandler.text;

                Debug.Log("get audio 끝" + _filePath + audioType.ToString());
                if (getAudioWWW.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log(getAudioWWW.error);
                }
                else
                {
                    //return new AudioClipPlay(DownloadHandlerAudioClip.GetContent(getAudioWWW), play);
                    OnGetClip(DownloadHandlerAudioClip.GetContent(getAudioWWW), play);
                }
            }

    }
    */

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

            using (UnityWebRequest www = UnityWebRequest.Get(url + "/music/"+type))
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
                Popup.Instance.Open();
                Debug.Log(e.ResponseCode+" [GET_SearchMusicTitleAsync] 토큰 만료");
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
            using (UnityWebRequest www = UnityWebRequest.Get(url + "/music/" + type.ToString()))
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
                Popup.Instance.Open();
                Debug.Log(e.ResponseCode+"[GET_SpecificMusicListAsync] 토큰 만료");
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

            using (UnityWebRequest www = UnityWebRequest.Get(url + "/follow" + (ft== FollowPage.FollowSystemType.follower ? "/follower" : ""))) 
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
                Popup.Instance.Open();
                Debug.Log(e.ResponseCode + "[GET_FollowSystemUserListAsync] 토큰 만료");
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

            using (UnityWebRequest www = UnityWebRequest.Get(url + "/user/search"))
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
                Popup.Instance.Open();
                Debug.Log(e.ResponseCode + "[ GET_SearchUserAsync] 토큰 만료");
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
    protected async UniTask POST_FollowUserAsync(string userID,string userName, bool isDelete=false)
    {
        try
        {
            UserNameId uni = new UserNameId();

            uni.userId = userID;
            uni.userNickname = userName;

            string json = "";
            json = JsonUtility.ToJson(uni);

            Debug.Log("follow "+isDelete+ " json: " + json);

            using (UnityWebRequest www = UnityWebRequest.Post(url + "/follow"+(isDelete?"/delete":""),json))
            {

                www.SetRequestHeader("token", UserData.Instance.Token);
                www.SetRequestHeader("Content-Type", "application/json");
                www.SetRequestHeader("accept", "text/plain");
                www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));

                await www.SendWebRequest();

                if (www.error == null)
                {

                    Debug.Log("done");

                }
                else
                {
                    Debug.Log(www.error.ToString());

                }
            }
        }
        catch (ArgumentNullException e)
        {
            Debug.Log("follow 요청 취소됨");
        }
        catch (UnityWebRequestException e)
        {
            if (e.ResponseCode == 400 || e.ResponseCode == 401)
            {
                Popup.Instance.Open();
                Debug.Log(e.ResponseCode + "[POST_FollowUserAsync] 토큰 만료");
            }
            else if (e.ResponseCode == 450)
            {
                //이미 팔로우한 유저

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
    protected async UniTask<User> GET_UserInfoAsync(string userId)
    {
        try
        {
            UserID us = new UserID();
            us.userId = userId;

            string json = "";
            json = JsonUtility.ToJson(us);

            Debug.Log("유저 정보 get json: " + json);

            using (UnityWebRequest www = UnityWebRequest.Get(GlobalData.url + "/user/info"))
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

                        JsonData jsonData = JsonToObject(jsonResult)["user"];
                        Debug.Log("결과 " + jsonResult);
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
                Popup.Instance.Open();
                Debug.Log(e.ResponseCode + "[ GET_UserInfoAsync] 토큰 만료");

            }
            else
            {
                Debug.Log(e);
            }
            return null;
        }
        catch (Exception e)
        {
            Popup.Instance.Open();
            Debug.LogError(e);
            return null;
        }
    }
    JsonData JsonToObject(string json)
    {
        return JsonMapper.ToObject(json);
    }
}



