using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using agora_gaming_rtc;
using Cysharp.Threading.Tasks;

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

                if (nowChannel != null)
                {
                    Debug.Log("Channel leave first");
                    leaveChannel();
                }
            }

            // init engine
            mRtcEngine = IRtcEngine.GetEngine(appID);

            // multiChannel setting
            mRtcEngine.SetMultiChannelWant(true);
            mRtcEngine.SetChannelProfile(CHANNEL_PROFILE.CHANNEL_PROFILE_LIVE_BROADCASTING);
            // 채널 만들기
            nowChannel = mRtcEngine.CreateChannel(channelName); // 채널 이름과 토큰이 같아야하는 듯

            // enable log
            mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);


            isEngineLoaded = true;
        }
    }

    public void deleteToken()
    {
        StartCoroutine(HelperClass.deleteToken(url: "localhost:8082", channel: channelName));
    }

    public void callJoin(int mode)
    {
        //StartCoroutine(join(mode));
        join(mode);
    }

    private async UniTask join(int mode) //IEnumerator
    {
        Debug.Log("calling join (AppID = " + appID + ")");

        if (mRtcEngine == null)
            return;
            //yield break;

        if (mode == 0) // Busker
        {
            Debug.Log("call Join Publisher");

            if (nowBuskingSpot != null)
                nowBuskingSpot.callChangeUsed();

            role = "publisher";
             _token = await HelperClass.FetchToken(url: "localhost:8082", channel: channelName, role: role, userId: myUID);

            nowChannel.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
            //nowChannel.ChannelOnReJoinChannelSuccess = onRejoinChannelSuccess;
            nowChannel.ChannelOnJoinChannelSuccess = onJoinChannelSuccess;
            nowChannel.ChannelOnLeaveChannel = onBuskerLeaveChannel;
            mRtcEngine.OnWarning = (int warn, string msg) =>
            {
                Debug.LogWarningFormat("Warning code:{0} msg:{1}", warn, IRtcEngine.GetErrorDescription(warn));
            };
            mRtcEngine.OnError = HandleError;

            mRtcEngine.EnableVideo();
            mRtcEngine.EnableVideoObserver();

            nowChannel.JoinChannel(token: _token, info: null, uid: myUID, channelMediaOptions: new ChannelMediaOptions(false, false, true, true));
            nowChannel.Publish();

        }
        else if (mode == 1)// Audience
        {
            Debug.Log("call Join Audience");

            role = "audience";
            _token = await HelperClass.FetchToken(url: "localhost:8082", channel: channelName, role: role, userId: myUID);

            nowChannel.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_AUDIENCE);
            nowChannel.MuteLocalVideoStream(true);
            nowChannel.MuteLocalAudioStream(true);
            nowChannel.ChannelOnJoinChannelSuccess = onJoinChannelSuccess;
            nowChannel.ChannelOnUserJoined = onBuskerJoined;
            nowChannel.ChannelOnUserOffLine = onBuskerOffline;
            nowChannel.ChannelOnLeaveChannel = onAudienceLeaveChannel;

            mRtcEngine.OnWarning = (int warn, string msg) =>
            {
                Debug.LogWarningFormat("Warning code:{0} msg:{1}", warn, IRtcEngine.GetErrorDescription(warn));
            };
            mRtcEngine.OnError = HandleError;

            mRtcEngine.EnableVideo();
            mRtcEngine.EnableVideoObserver();

            nowChannel.JoinChannel(token: _token, info: null, uid: myUID, channelMediaOptions: new ChannelMediaOptions(true, true, false, false));
        }
        else
        {
            Debug.Log("Wrong join mode");
        }
     }

    // implement engine callbacks
    private void onJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("JoinChannelSuccessHandler: uid = " + uid);
    }

    private void onAudienceLeaveChannel(string channelId, RtcStats rtcStats)
    {
        Debug.Log("Audience(me) is leave because: " + rtcStats.duration);

        Destroy(audienceVideo.GetComponent<VideoSurface>());
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().OffVideoPanel();
        nowBuskingSpot.offTitleBar();

        unloadEngine();
    }


    private void onBuskerLeaveChannel(string channelId, RtcStats rtcStats)
    {
        Debug.Log("Busker(Me) is leave because: " + rtcStats.duration);

        nowChannel.Unpublish();

        Destroy(buskersmallVideo.GetComponent<VideoSurface>());
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().OffVideoPanel();

        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isMoveAble = true;
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isUIActable = true;

        unloadEngine();
    }
    
    private void onBuskerJoined(string channelId, uint uid, int elapsed)
    {
        Debug.Log("onBuskerInfo: uid = " + uid + " elapsed = " + elapsed + " nowChannel: " + channelId);
        // this is called in main thread

        setAudicenVideoSurface(audienceVideo, channelId, uid, elapsed);
    }

    private void onBuskerOffline(string channelId, uint uid, USER_OFFLINE_REASON reason)
    {
        Debug.Log("onLeaveBuskerInfo: uid = " + uid + " reason = " + reason + " nowChannel: " + channelId);

        /**
        Destroy(audienceVideo.GetComponent<VideoSurface>());
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().OffVideoPanel();
        nowBuskingSpot.offTitleBar();
        **/

        leaveChannel();
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

    public void leaveChannel()
    {
        if (nowChannel != null)
        {
            nowChannel.LeaveChannel();
            Debug.Log("Leaving channel: " + channelName);
        }
        else
        {
            Debug.LogWarning("Channel: " + channelName + " hasn't been created yet.");
        }
    }


    // unload agora engine
    public void unloadEngine()
    {
        if (isEngineLoaded)
        {
            Debug.Log("calling unloadEngine");

            nowChannel.ReleaseChannel();

            if (role == "publisher")
            {
                nowBuskingSpot.callChangeUsed();
            }
            else if (role == "audience")
            {
                nowBuskingSpot.offTitleBar();
            }
            else
            {
                Debug.Log("No unload engine because Wrong role");
            }

            mRtcEngine.DisableVideo();
            mRtcEngine.DisableVideoObserver();
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
        leaveChannel();
        //unloadEngine();
    }


}
