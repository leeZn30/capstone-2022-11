using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

public class MusicUpload : Singleton<MusicUpload>
{
    string url = "http://localhost:8080";
    void Start()
    {
        
    }
    public void FileUpload(byte[] bytes, string fileName)
    {
        StartCoroutine(Upload(bytes, fileName));
    }
    IEnumerator Upload(byte[] bytes, string fileName)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        formData.Add(new MultipartFormFileSection(fileName, bytes));

        UnityWebRequest www = UnityWebRequest.Post(url+"/media/", formData);
        //추후 로딩 애니메이션추가 
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            //로딩애니메이션 종료
            Debug.Log("Form upload complete!");
        }
    }
}
