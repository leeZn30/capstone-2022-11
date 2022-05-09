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
    [SerializeField] private List<string> tokens;
    [SerializeField] private AgoraChannel nowChannel = null;
    private IRtcEngine mRtcEngine = null;
    [SerializeField] private uint myUID;

    [Header("GameObjects")]
    [SerializeField] private RawImage buskerVideo;
    [SerializeField] private RawImage audienceVideo;

    // instance of agora engine
    private Text MessageText;

    // BuskingZone 정보
    public BuskingSpot nowBuskingSpot;

    // TMP
    public TextMeshProUGUI text;
    public string _token = null;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            join(1);
        }
    }

    public void setMyUID() // userId이용해서 하기
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

            // 임시
            if (myUID == 123)
            {
                setToken("audience");
            }

            // enable log
            mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);

        }
    }

    public void setToken(string role)
    {
        StartCoroutine(HelperClass.FetchToken(url: "localhost:8080", channel: channelName, role: role, userId: myUID, callback: getToken));
    }

    private void getToken(string token)
    {
        _token = token;

        Debug.Log("Received token: " + _token);
    }

    public void deleteToken()
    {
        StartCoroutine(HelperClass.deleteToken(url: "localhost:8080", channel: channelName));
    }

    public void join(int mode)
    {
        Debug.Log("calling join (AppID = " + appID + ")");
        //text.text = nowChannel.ChannelId();

        if (mRtcEngine == null)
            return;

        if (mode == 0) // Busker
        {
            if (nowBuskingSpot != null)
                nowBuskingSpot.callChangeUsed();

            nowChannel.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
            nowChannel.ChannelOnJoinChannelSuccess = onJoinChannelSuccess;
            nowChannel.ChannelOnLeaveChannel = onLeaveChannel;
            //mRtcEngine.OnJoinChannelSuccess = onJoinChannelSuccess;
            mRtcEngine.OnWarning = (int warn, string msg) =>
            {
                Debug.LogWarningFormat("Warning code:{0} msg:{1}", warn, IRtcEngine.GetErrorDescription(warn));
            };
            mRtcEngine.OnError = HandleError;

            mRtcEngine.EnableVideo();
            mRtcEngine.EnableVideoObserver();;

            //mRtcEngine.RenewToken(_token);
            //mRtcEngine.JoinChannelByKey(_token, "testing", null, myUID);
            nowChannel.JoinChannel(token: _token, info: null, uid: myUID, channelMediaOptions: new ChannelMediaOptions(false, false, true, true));
            //nowChannel.JoinChannel(token: tokens[Int32.Parse(channelName)], info: null, uid: 0, channelMediaOptions: new ChannelMediaOptions(false, false, true, true));

            nowChannel.Publish();

            setBuskerVideoSurface();
        }
        else // Audience
        {
            nowChannel.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_AUDIENCE);
            nowChannel.MuteLocalVideoStream(true);
            nowChannel.MuteLocalAudioStream(true);
            nowChannel.ChannelOnJoinChannelSuccess = onJoinChannelSuccess;
            nowChannel.ChannelOnUserJoined = onBuskerJoined;

            //mRtcEngine.OnJoinChannelSuccess = onJoinChannelSuccess;
            //mRtcEngine.OnUserJoined = onUserJoined;
            //nowChannel.ChannelOnLeaveChannel = oneLeaveChannel;  // audience는 나간거 신경 x
            mRtcEngine.OnWarning = (int warn, string msg) =>
            {
                Debug.LogWarningFormat("Warning code:{0} msg:{1}", warn, IRtcEngine.GetErrorDescription(warn));
            };
            mRtcEngine.OnError = HandleError;

            mRtcEngine.EnableVideo();
            mRtcEngine.EnableVideoObserver();

            //mRtcEngine.JoinChannelByKey(_token, "testing", null, myUID); // 바로 못받아오고 있음
            //mRtcEngine.JoinChannelByKey(channelKey: token, channelName: "MetaBusking");
            nowChannel.JoinChannel(token: _token, info: null, uid: myUID, channelMediaOptions: new ChannelMediaOptions(true, true, false, false));

        }
     }

    // implement engine callbacks
    private void onJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("JoinChannelSuccessHandler: uid = " + uid);
    }


    private void onLeaveChannel(string channelId, RtcStats rtcStats)
    {
        nowBuskingSpot.callChangeUsed();
        deleteChannel();
        deleteToken();
    }
    
    private void onBuskerJoined(string channelId, uint uid, int elapsed)
    {
        Debug.Log("onBuskerInfo: uid = " + uid + " elapsed = " + elapsed + " nowChannel: " + channelId);
        // this is called in main thread

        // create a GameObject and assign to this new user
        VideoSurface videoSurface = makeImageSurface(audienceVideo);
        if (!ReferenceEquals(videoSurface, null))
        {
            // configure videoSurface
            videoSurface.SetForMultiChannelUser(channelId, uid);
            videoSurface.SetForUser(uid); // 이러면 이제 되는거같은데
            videoSurface.SetEnable(true);
            videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.RawImage);
        }
    }

    private void onUserJoined(uint uid, int elapsed)
    {
        //Debug.Log("onBuskerInfo: uid = " + uid + " elapsed = " + elapsed + " nowChannel: " + channelId);
        // this is called in main thread

        // create a GameObject and assign to this new user
        VideoSurface videoSurface = makeImageSurface(audienceVideo);
        if (!ReferenceEquals(videoSurface, null))
        {
            // configure videoSurface
            //videoSurface.SetForMultiChannelUser(channelId, uid);
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

            nowBuskingSpot = null;
            channelName = null;

            // delete
            if (mRtcEngine != null)
            {
                // 지금 나가는 버튼을 안만들어서 임시로
                deleteToken();

                deleteChannel();
                nowChannel = null;
                IRtcEngine.Destroy();  // Place this call in ApplicationQuit
                mRtcEngine = null;
                isEngineLoaded = false;
            }
        }
    }

    public void setBuskerVideoSurface()
    {
        VideoSurface videoSurface = makeImageSurface(buskerVideo);
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
