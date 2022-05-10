using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

[Serializable]
public class TokenObject
{
    public string token; //token
    //public string code;
}

public class channelInfo
{
    public uint uid;
    public string channelName;
    public int role;
}

public static class HelperClass
{
    public static IEnumerator FetchToken(string url, string channel, string role, uint userId, Action<string> callback = null)
    {
        UnityWebRequest request = UnityWebRequest.Get(string.Format(
          "{0}/rtc/{1}/{2}/uid/{3}/", url, channel, role, userId
        ));
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogWarning("FetchToken: url = " + url + " error:" + request.error);
            callback(null);
            yield break;
        }

        TokenObject tokenInfo = JsonUtility.FromJson<TokenObject>(request.downloadHandler.text);

        callback(tokenInfo.token);
    }

    public static IEnumerator deleteToken(string url, string channel)
    {
        UnityWebRequest request = UnityWebRequest.Get(string.Format(
          "{0}/rtc/{1}/delete/", url, channel
        ));
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogWarning("deleteToken: url = " + url + " error:" + request.error);
            yield break;
        }
    }

    public static async UniTask<string> testToken(string url, string channel, string role, uint userId)
    {
        UnityWebRequest request = UnityWebRequest.Get(string.Format(
         "{0}/rtc/{1}/{2}/uid/{3}/", url, channel, role, userId
       ));
        await request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogWarning("FetchToken: url = " + url + " error:" + request.error);
            return null;
        }

        TokenObject tokenInfo = JsonUtility.FromJson<TokenObject>(request.downloadHandler.text);

        return tokenInfo.token;
    }
}