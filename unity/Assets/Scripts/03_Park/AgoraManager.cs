using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using agora_gaming_rtc;

public class AgoraManager : Singleton<AgoraManager>
{
    [Header("Agora Properties")]
    private bool isEngineLoaded = false;
    // *** ADD YOUR APP ID HERE BEFORE GETTING STARTED *** //
    [SerializeField] private string appID = "ADD YOUR APP ID HERE";
    public string channelName = "unity3d";
    [SerializeField] private AgoraChannel nowChannel = null;
    public string _token = null;
    private IRtcEngine mRtcEngine = null;
    [SerializeField] private uint myUID;
    [SerializeField] private string role;

    [Header("GameObjects")]
    [SerializeField] private RawImage buskerVideo;
    [SerializeField] private RawImage buskersmallVideo;
    [SerializeField] private RawImage audienceVideo;
    [SerializeField]
    private RawImage bigAudienceImage;

    // instance of agora engine
    private Text MessageText;

    // BuskingZone 정보
    public BuskingSpot nowBuskingSpot;

    // TMP
    public InputField InputField;

    private void Start()
    {
        myUID = UIDCreator.createUID(UserData.Instance.id);
    }


    private void Update()
    {
    }

    // load agora engine
    public void loadEngine()
    {
        if (!isEngineLoaded)
        {
            // start sdk
            Debug.Log("initializeEngine");

            if (mRtcEngine != null)
            {
                Debug.Log("Engine exists. Please unload it first!");
                return;
            }

            // init engine
            mRtcEngine = IRtcEngine.GetEngine(appID);

            isEngineLoaded = true;

            // multiChannel setting
            mRtcEngine.SetMultiChannelWant(true);
            mRtcEngine.SetChannelProfile(CHANNEL_PROFILE.CHANNEL_PROFILE_LIVE_BROADCASTING);
            // 채널 만들기
            nowChannel = mRtcEngine.CreateChannel(channelName); // 채널 이름과 토큰이 같아야하는 듯

            // enable log
            mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);

        }
    }

    public async void setToken(string role)
    {
        StartCoroutine(HelperClass.FetchToken(url: "localhost:8082", channel: channelName, role: role, userId: myUID, callback: getToken));
    }

    private void getToken(string token)
    {
        _token = token;

        Debug.Log("Received token: " + _token);
    }

    public void deleteToken()
    {
        StartCoroutine(HelperClass.deleteToken(url: "localhost:8082", channel: channelName));
    }

    public void callJoin(int mode)
    {
        StartCoroutine(join(mode));
    }

    private IEnumerator join(int mode)
    {
        Debug.Log("calling join (AppID = " + appID + ")");

        if (mRtcEngine == null)
            yield break;

        if (mode == 0) // Busker
        {
            if (nowBuskingSpot != null)
                nowBuskingSpot.callChangeUsed();

            role = "publisher";
            setToken("publisher");
            yield return new WaitForSeconds(0.05f);

            nowChannel.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
            //nowChannel.ChannelOnReJoinChannelSuccess = onRejoinChannelSuccess;
            nowChannel.ChannelOnJoinChannelSuccess = onJoinChannelSuccess;
            nowChannel.ChannelOnLeaveChannel = onLeaveChannel;
            mRtcEngine.OnWarning = (int warn, string msg) =>
            {
                Debug.LogWarningFormat("Warning code:{0} msg:{1}", warn, IRtcEngine.GetErrorDescription(warn));
            };
            mRtcEngine.OnError = HandleError;

            mRtcEngine.EnableVideo();
            mRtcEngine.EnableVideoObserver();

            nowChannel.JoinChannel(token: _token, info: null, uid: myUID, channelMediaOptions: new ChannelMediaOptions(false, false, true, true));
            nowChannel.Publish();

            setBuskerVideoSurface(buskerVideo);
        }
        else // Audience
        {
            role = "audience";
            setToken("audience");
            yield return new WaitForSeconds(0.05f);

            nowChannel.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_AUDIENCE);
            nowChannel.MuteLocalVideoStream(true);
            nowChannel.MuteLocalAudioStream(true);
            nowChannel.ChannelOnJoinChannelSuccess = onJoinChannelSuccess;
            nowChannel.ChannelOnUserJoined = onBuskerJoined;
            nowChannel.ChannelOnUserOffLine = onBuskerOffline;

            mRtcEngine.OnWarning = (int warn, string msg) =>
            {
                Debug.LogWarningFormat("Warning code:{0} msg:{1}", warn, IRtcEngine.GetErrorDescription(warn));
            };
            mRtcEngine.OnError = HandleError;

            mRtcEngine.EnableVideo();
            mRtcEngine.EnableVideoObserver();

            nowChannel.JoinChannel(token: _token, info: null, uid: myUID, channelMediaOptions: new ChannelMediaOptions(true, true, false, false));

        }
     }

    private void onReJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {

    }

    // implement engine callbacks
    private void onJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("JoinChannelSuccessHandler: uid = " + uid);
    }

    private void onLeaveChannel(string channelId, RtcStats rtcStats)
    {
        Debug.Log("Busker(Me) is leave");

        deleteChannel();

        Destroy(buskersmallVideo.GetComponent<VideoSurface>());
        buskersmallVideo.gameObject.SetActive(false);
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isMoveAble = true;
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isUIActable = true;
    }
    
    private void onBuskerJoined(string channelId, uint uid, int elapsed)
    {
        Debug.Log("onBuskerInfo: uid = " + uid + " elapsed = " + elapsed + " nowChannel: " + channelId);
        // this is called in main thread

        setAudicenVideoSurface(audienceVideo, channelId, uid, elapsed);
    }

    private void onBuskerOffline(string channelId, uint uid, USER_OFFLINE_REASON reason)
    {
        Debug.Log("onLeaveBuskerInfo: uid = " + uid + " elapsed = " + reason + " nowChannel: " + channelId);

        Destroy(audienceVideo.GetComponent<VideoSurface>());
        audienceVideo.gameObject.SetActive(false);
        nowBuskingSpot.offTitleBar();
    }

    public void setAudicenVideoSurface(RawImage rawImage, string channelId, uint uid, int elapsed)
    {
        // create a GameObject and assign to this new user
        VideoSurface videoSurface = makeImageSurface(rawImage);
        if (!ReferenceEquals(videoSurface, null))
        {
            // configure videoSurface
            videoSurface.SetForMultiChannelUser(channelId, uid);
            videoSurface.SetForUser(uid); // 이러면 이제 되는거같은데
            videoSurface.SetEnable(true);
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
        }

    }

    public VideoSurface makeImageSurface(RawImage rawimage)
    {
        // configure videoSurface
        VideoSurface videoSurface = rawimage.gameObject.AddComponent<VideoSurface>();
        videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
        return videoSurface;
    }

    public void deleteChannel()
    {
        if (nowChannel != null)
        {
            //mRtcEngine.LeaveChannel();
            nowChannel.LeaveChannel();
            nowChannel.ReleaseChannel();
        }
    }

    // unload agora engine
    public void unloadEngine()
    {
        if (isEngineLoaded)
        {
            Debug.Log("calling unloadEngine");

            if (role == "publisher")
            {
                Destroy(buskersmallVideo.GetComponent<VideoSurface>());
                GameManager.instance.myPlayer.GetComponent<PlayerControl>().OffVideoPanel();
                nowBuskingSpot.offTitleBar();
                nowBuskingSpot.callChangeUsed();
                deleteChannel();
            }
            else
            {
                Destroy(audienceVideo.GetComponent<VideoSurface>());
                GameManager.instance.myPlayer.GetComponent<PlayerControl>().OffVideoPanel();
                nowBuskingSpot.offTitleBar();
            }

            nowBuskingSpot = null;
            channelName = null;
            role = null;

            // delete
            if (mRtcEngine != null)
            {
                nowChannel = null;
                IRtcEngine.Destroy();  // Place this call in ApplicationQuit
                mRtcEngine = null;
                isEngineLoaded = false;
            }
        }
    }

    public void setBuskerVideoSurface(RawImage rawImage)
    {
        VideoSurface videoSurface = makeImageSurface(rawImage);
    }


    #region Error Handling
    private int LastError { get; set; }
    private void HandleError(int error, string msg)
    {
        if (error == LastError)
        {
            return;
        }

        if (string.IsNullOrEmpty(msg))
        {
            msg = string.Format("Error code:{0} msg:{1}", error, IRtcEngine.GetErrorDescription(error));
        }

        switch (error)
        {
            case 101:
                msg += "\nPlease make sure your AppId is valid and it does not require a certificate for this demo.";
                break;
        }

        Debug.LogError(msg);
        if (MessageText != null)
        {
            if (MessageText.text.Length > 0)
            {
                msg = "\n" + msg;
            }
            MessageText.text += msg;
        }

        LastError = error;
    }

    #endregion


    private void OnApplicationQuit()
    {
        unloadEngine();
    }


}
