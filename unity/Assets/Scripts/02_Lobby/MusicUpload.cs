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
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
    }
}
