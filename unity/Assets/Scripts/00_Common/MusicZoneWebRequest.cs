using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using Cysharp.Threading.Tasks;
using System;
public class MusicZoneNumber
{

    public int zoneNumber;

}
[System.Serializable]
public class MusicZoneInputData
{
    public int zoneNumber;
    public List<float> timeList=new List<float>();
    public List<string> locateList=new List<string> ();
    public List<string> titleList = new List<string>();

}
public class MusicZoneOutputData
{
    public float time;
    public List<string> locateList = new List<string>();
    public List<string> titleList = new List<string>();

}
public class MusicZoneWebRequest : MonoBehaviour
{
    protected string url = GlobalData.url;
    // Start is called before the first frame update
    void Start()
    {
        
    }
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

                    for(int i=0; i<jsonData["title"].Count; i++)
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
    JsonData JsonToObject(string json)
    {
        return JsonMapper.ToObject(json);
    }
}
