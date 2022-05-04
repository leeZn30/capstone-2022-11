using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using agora_gaming_rtc;

public class AgoraManager : Singleton<AgoraManager>
{
    /**
    [Header("Agora Properties")]

    // *** ADD YOUR APP ID HERE BEFORE GETTING STARTED *** //
    [SerializeField] private string appID = "ADD YOUR APP ID HERE";
    //[SerializeField] private string channel = "unity3d";
    [SerializeField] private string token = "ADD Your APP TOKEN HERE";
    [SerializeField] private AgoraChannel nowChannel;
    private IRtcEngine mRtcEngine = null;
    private uint myUID = 0;
    **/

    [Header("GameObjects")]
    [SerializeField] private RawImage buskerVideo;
    [SerializeField] private RawImage audienceVideo;

    // instance of agora engine
    private IRtcEngine mRtcEngine;
    private Text MessageText;
    [SerializeField] private string appID;

    // a token is a channel key that works with a AppID that requires it. 
    // Generate one by your token server or get a temporary token from the developer console
    private string token = "006ed5d27a64ca7451189266ef6703397bfIAA2wv6ofnhXKDGGLECAtqRCyBNxkd3TjzRKsFNm5cpnho5IG0UAAAAAEACf/Brp/nhzYgEAAQD/eHNi";

    public BuskingSpot nowBuskingSpot;

    // load agora engine
    public void loadEngine(string appId)
    {
        // start sdk
        Debug.Log("initializeEngine");

        if (mRtcEngine != null)
        {
            Debug.Log("Engine exists. Please unload it first!");
            return;
        }

        // init engine
        mRtcEngine = IRtcEngine.GetEngine(appId);

        // enable log
        mRtcEngine.SetLogFilter(LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL);
    }

    public void join(int mode)
    {
        if (nowBuskingSpot != null)
            nowBuskingSpot.callChangeUsed();

        Debug.Log("calling join (channel = " + appID + ")");

        if (mRtcEngine == null)
            return;

        if (mode == 0) // Busker
        {
            // set callbacks (optional)
            mRtcEngine.OnJoinChannelSuccess = onJoinChannelSuccess;
            //mRtcEngine.OnUserJoined = onUserJoined;
            //mRtcEngine.OnUserOffline = onUserOffline;
            mRtcEngine.OnWarning = (int warn, string msg) =>
            {
                Debug.LogWarningFormat("Warning code:{0} msg:{1}", warn, IRtcEngine.GetErrorDescription(warn));
            };
            mRtcEngine.OnError = HandleError;

            // enable video
            mRtcEngine.EnableVideo();
            // allow camera output callback
            mRtcEngine.EnableVideoObserver();

            // join channel
            /*  This API Assumes the use of a test-mode AppID
                 mRtcEngine.JoinChannel(channel, null, 0);
            */

            /*  This API Accepts AppID with token; by default omiting info and use 0 as the local user id */
            mRtcEngine.JoinChannelByKey(channelKey: token, channelName: "MetaBusking");

            onSceneHelloVideoLoaded(0);
        }
        else // Audience
        {
            // set callbacks (optional)
            mRtcEngine.OnJoinChannelSuccess = onAudienceJoinChannelSuccess;
            mRtcEngine.OnUserJoined = onUserJoined; // Busker만 봐야함
            //mRtcEngine.OnUserOffline = onUserOffline;
            mRtcEngine.OnWarning = (int warn, string msg) =>
            {
                Debug.LogWarningFormat("Warning code:{0} msg:{1}", warn, IRtcEngine.GetErrorDescription(warn));
            };
            mRtcEngine.OnError = HandleError;

            mRtcEngine.EnableVideo();
            mRtcEngine.EnableVideoObserver();

            // join channel
            /*  This API Assumes the use of a test-mode AppID
                 mRtcEngine.JoinChannel(channel, null, 0);
            */

            /*  This API Accepts AppID with token; by default omiting info and use 0 as the local user id */
            mRtcEngine.JoinChannelByKey(channelKey: token, channelName: "MetaBusking");

            //onSceneHelloVideoLoaded(1);

        }
    }

    public void leave()
    {
        Debug.Log("calling leave");

        if (mRtcEngine == null)
            return;

        // leave channel
        mRtcEngine.LeaveChannel();
        // deregister video frame observers in native-c code
        mRtcEngine.DisableVideoObserver();
    }

    // unload agora engine
    public void unloadEngine()
    {
        Debug.Log("calling unloadEngine");

        nowBuskingSpot = null;

        // delete
        if (mRtcEngine != null)
        {
            IRtcEngine.Destroy();  // Place this call in ApplicationQuit
            mRtcEngine = null;
        }
    }

    // accessing GameObject in Scnene1
    // set video transform delegate for statically created GameObject
    public void onSceneHelloVideoLoaded(int mode)
    {
        if (mode == 0) // Busker
        {
            VideoSurface videoSurface = makeImageSurface(buskerVideo);
        }
        else // Audience
        {
            VideoSurface videoSurface = makeImageSurface(audienceVideo);
        }
    }


    // implement engine callbacks
    private void onJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("Busker JoinChannelSuccessHandler: uid = " + uid);
    }

    private void onAudienceJoinChannelSuccess(string channelName, uint uid, int elapsed)
    {
        Debug.Log("Audience JoinChannelSuccessHandler: uid = " + uid);
    }

    // When a remote user joined, this delegate will be called. Typically
    // create a GameObject to render video on it
    private void onUserJoined(uint uid, int elapsed)
    {
        Debug.Log("onUserJoined: uid = " + uid + " elapsed = " + elapsed);
        // this is called in main thread

        // create a GameObject and assign to this new user
        VideoSurface videoSurface = makeImageSurface(audienceVideo);
        if (!ReferenceEquals(videoSurface, null))
        {
            // configure videoSurface
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
    // When remote user is offline, this delegate will be called. Typically
    // delete the GameObject for this user
    private void onUserOffline(uint uid, USER_OFFLINE_REASON reason)
    {
        // remove video stream
        Debug.Log("onUserOffline: uid = " + uid + " reason = " + reason);
        // this is called in main thread
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



    /**
    void setChannel(int roomNum)
    {
        // channel을 여러개 만든다
        // channel 이름은 방 이름으로
        if (mRtcEngine != null)
        {
            nowChannel = mRtcEngine.CreateChannel(roomNum.ToString());

            // SetClientRole >> Host
            nowChannel.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);

        }
    }


    public void setBuskerAgora(int roomNum)
    {
        // channel 생성
        setChannel(roomNum);

        // Video setup
        VideoEncoderConfiguration config = new VideoEncoderConfiguration();
        config.dimensions.width = 480;
        config.dimensions.height = 480;
        config.frameRate = FRAME_RATE.FRAME_RATE_FPS_15;
        config.bitrate = 800;
        config.degradationPreference = DEGRADATION_PREFERENCE.MAINTAIN_QUALITY;
        mRtcEngine.SetVideoEncoderConfiguration(config);

        // SetUp Callbacks
        nowChannel.ChannelOnJoinChannelSuccess 
            = OnJoinChannelSuccessHandler;
        // mRtcEngine.OnUserJoined = OnAudienceJoinedHandler; // 다른 유저 들어왔는지 알 필요 없음
        mRtcEngine.OnLeaveChannel = OnBuskerLeaveChannelHandler;
        // mRtcEngine.OnUserOffline = OnAudienceOfflineHandler; // 다른 유저 나가는지 알 필요 없음

        // Video 부르기
        mRtcEngine.EnableVideo();
        mRtcEngine.EnableVideoObserver();

        // By setting our UID to "0" the Agora Engine creates a unique UID and returns it in the OnJoinChannelSuccess callback. 
        nowChannel.JoinChannel(token, "", 0, new ChannelMediaOptions(true, true));
    }
    **/


    #region Agora Callbacks
    // Busker Join Success Handler
    /**
    private void OnJoinChannelSuccessHandler(string channelName, uint uid, int elapsed)
    {
        Debug.Log("Join Channel Success");

        myUID = uid;

        CreateUserVideoSurface(uid, true);
    }

    // Audience Join Success Handler
    private void OnAudienceJoinedHandler(uint uid, int elapsed)
    {

    }

    // Busker가 떠나갈 때
    private void OnBuskerLeaveChannelHandler(RtcStats stats)
    {
        if (mRtcEngine != null)
        {
            // 채널 파괴하기
            nowChannel.LeaveChannel();
            nowChannel.ReleaseChannel();

            // mRTCEngine video 안되게
            mRtcEngine.DisableVideoObserver();
            mRtcEngine.DisableVideo();
        }
    }
    **/
    #endregion

    // Create new image plane to display users in party.
    private void CreateUserVideoSurface(uint uid, bool isLocalUser)
    {
        // BuskerVideo에 VideoSurface 달기
        if (buskerVideo == null)
        {
            Debug.LogError("CreateUserVideoSurface() - newUserVideoIsNull");
            return;
        }

        // Update our VideoSurface to reflect new users
        VideoSurface newVideoSurface = buskerVideo.GetComponent<VideoSurface>();
        if (newVideoSurface == null)
        {
            Debug.LogError("CreateUserVideoSurface() - VideoSurface component is null on newly joined user");
            return;
        }

        /**
        if (isLocalUser == false)
        {
            newVideoSurface.SetForUser(uid);
        }
        newVideoSurface.SetGameFps(30);
        **/
    }


    private void OnApplicationQuit()
    {
        //TerminateAgoraEngine();

        unloadEngine();
    }

    private void TerminateAgoraEngine()
    {
        if (mRtcEngine != null)
        {
            mRtcEngine.LeaveChannel();
            mRtcEngine = null;
            IRtcEngine.Destroy();
        }
    }


}
