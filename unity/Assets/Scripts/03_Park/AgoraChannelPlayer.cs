using System.Collections.Generic;
using UnityEngine;
using agora_gaming_rtc;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class AgoraChannelPlayer : Singleton<AgoraChannelPlayer>
{
    // Channel관련
    [Header("Channel 관련")]
    public string channelName;
    [SerializeField] private string channelToken;
    [SerializeField] private bool isPublishing;
    private AgoraChannel nowChannel;
    [SerializeField] private bool isFoundBusker = false;
    [SerializeField] private uint buskerUid;

    // 필요 Object
    [SerializeField] private RawImage buskersmallVideo;
    [SerializeField] private RawImage audienceVideo;
    [SerializeField] private InfoPanel infoPanel;

    // User 정보
    [Header("User 정보")]
    [SerializeField] private uint myUID;
    public string role;

    // BuskingZone 정보
    [Header("BuskingZone 정보")]
    public BuskingSpot nowBuskingSpot;
    string buskerName;
    string title;
    string buskerId;
#if !UNITY_ANDROID
    string url = "http://metabusking.c.cs.kookmin.ac.kr"; //string url = "http://localhost:8082";
#else
    string url = "http://metabusking.c.cs.kookmin.ac.kr";
#endif

    private void Start()
    {
        myUID = UIDCreator.createUID(UserData.Instance.id);
    }

    // ======================= Join 관련 =============================
    public void callJoin(int mode, string buskerID = null, string buskerNickname = null, string title = null)
    {
        join(mode, buskerID, buskerNickname, title);
    }
    private async UniTask join(int mode, string buskerID = null, string buskerNickname = null, string t = null) //IEnumerator
    {
        if (nowChannel == null)
        {
            Debug.Log("calling Channel mode = " + mode);
            nowBuskingSpot.offTitleBar();
            if (mode == 0) // Busker
            {
                role = "publisher";

                buskerId = buskerID;
                buskerName = buskerNickname;
                title = t;

                 channelToken = await HelperClass.FetchToken(url: url, channel: channelName, role: role, userId: myUID);

   

                if (channelToken == "not Token") // 실패할 경우
                {
                    GameManager.instance.myPlayer.GetComponent<PlayerControl>().isMoveAble = true;
                    GameManager.instance.myPlayer.GetComponent<PlayerControl>().isUIActable = true;
                    return;
                }

                nowChannel = AgoraEngine.mRtcEngine.CreateChannel(channelName);
                nowChannel.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);

                nowChannel.ChannelOnJoinChannelSuccess = OnBuskerJoinChannelSuccess;
                nowChannel.ChannelOnLeaveChannel = OnBuskerLeaveChannel;

                publish();

                nowChannel.JoinChannel(channelToken, null, myUID, new ChannelMediaOptions(false, false, true, true)); // 이거 내가 한건 다 다름
                Debug.Log("Joining channel: " + channelName);
            }
            else if (mode == 1) //Audience
            {
                role = "audience";

                channelToken = await HelperClass.FetchToken(url: url, channel: channelName, role: role, userId: myUID);

                if (channelToken == "not Token")
                    return;

                nowChannel = AgoraEngine.mRtcEngine.CreateChannel(channelName);
                nowChannel.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_AUDIENCE);
                nowChannel.MuteLocalVideoStream(true);
                nowChannel.MuteLocalAudioStream(true);

                nowChannel.ChannelOnJoinChannelSuccess = OnAudienceJoinChannelSuccess;
                nowChannel.ChannelOnUserJoined = OnBuskerJoined;
                nowChannel.ChannelOnLeaveChannel = OnAudienceLeaveChannel;
                nowChannel.ChannelOnUserOffLine = OnBuskerOffline;

                nowChannel.JoinChannel(channelToken, null, myUID, new ChannelMediaOptions(true, true, false, false));

                Debug.Log("Joining channel: " + channelName);
            }
            else
            {
                Debug.Log("Wrong join mode");
            }
        }
        else
        {
            Debug.Log("nowChannel is not null!");
        }

    }
    //=========================================================

    // ============= 각종 코드 ===========
    private void publish()
    {
        if (nowChannel == null)
        {
            Debug.LogError("New channel isn't created yet.");
            return;
        }

        if (isPublishing == false)
        {
            int publishResult = nowChannel.Publish();
            if (publishResult == 0)
            {
                isPublishing = true;
            }

            Debug.Log("Publishing to channel: " + channelName + " result: " + publishResult);
        }
        else
        {
            Debug.Log("Already publishing to a channel.");
        }
    }
    private void unpublish()
    {
        if (nowChannel == null)
        {
            Debug.Log("New channel isn't created yet.");
            return;
        }

        if (isPublishing == true)
        {
            int unpublishResult = nowChannel.Unpublish();
            if (unpublishResult == 0)
            {
                isPublishing = false;
            }

            Debug.Log("Unpublish from channel: " + channelName + " result: " + unpublishResult);
        }
        else
        {
            Debug.Log("Not published to any channel");
        }
    }

    private void setAudicenVideoSurface(RawImage rawImage, string channelId, uint uid, int elapsed)
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
    public void setBuskerVideoSurface(RawImage rawImage)
    {
        VideoSurface videoSurface = makeImageSurface(rawImage);
    }

    public async UniTask leaveChannel()
    {
        if (nowChannel != null)
        {

            if (role == "publisher")
            {
                string s = await HelperClass.deleteToken(url:url, channelName);
            }

            nowBuskingSpot.offTitleBar();
            nowChannel.LeaveChannel();
            Debug.Log("Leaving channel: " + channelName);
        }
        else
        {
            Debug.LogWarning("Channel: " + channelName + " hasn't been created yet.");
        }
    }

    //============================================

    #region Handler
    public void OnBuskerJoinChannelSuccess(string channelID, uint uid, int elapsed)
    {
        Debug.Log("Join party channel success - channel: " + channelID + " uid: " + uid);

        if (nowBuskingSpot != null)
        {
            Debug.Log("busker join handler");
            nowBuskingSpot.callSetBuskingZone(buskerId, buskerName, title);
            nowBuskingSpot.onTitleBar();
        }

        // Busker 화면 없애기
        buskersmallVideo.gameObject.SetActive(true);
        setBuskerVideoSurface(buskersmallVideo);
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isUIActable = true;

        // 버튼 설정
        PlayerControl player = GameManager.instance.myPlayer.GetComponent<PlayerControl>();
        player.changeInteractiveButton(1);
        nowBuskingSpot.callInsideUserJoin(AgoraChannelPlayer.Instance.channelName);

    }
    public void OnAudienceJoinChannelSuccess(string channelID, uint uid, int elapsed)
    {
        Debug.Log("Join party channel success - channel: " + channelID + " uid: " + uid);

    }



    public void OnBuskerJoined(string channelID, uint uid, int elapsed)
    {
        if (!isFoundBusker) // Busker는 맨 처음 들어옴
        {
            Debug.Log("onBuskerInfo: uid = " + uid + " elapsed = " + elapsed + " nowChannel: " + channelID);
            // this is called in main thread

            buskerUid = uid;
            setAudicenVideoSurface(audienceVideo, channelID, uid, elapsed);

            PlayerControl player = GameManager.instance.myPlayer.GetComponent<PlayerControl>();

            // 만약 방송 준비중이었다면 지워줌
            player.OffVideoPanel();
            player.isMoveAble = true;
            player.isUIActable = true;

            player.OnVideoPanel(0);
            audienceVideo.gameObject.SetActive(true);
            player.OnInteractiveButton(2); // 버튼 활성화 아니었던 사람들
            player.changeInteractiveButton(2); // 버튼 활성화 되어있던 사람들

            isFoundBusker = true;

            if (nowBuskingSpot != null)
                nowBuskingSpot.onTitleBar();
        }

    }

    private void OnBuskerLeaveChannel(string channelID, RtcStats stats)
    {
        Debug.Log("Busker(Me) is leave");

        unpublish();

        Destroy(buskersmallVideo.GetComponent<VideoSurface>());
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().OffVideoPanel();
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().changeInteractiveButton(0);

        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isMoveAble = true;
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isUIActable = true;

        nowBuskingSpot.callSetBuskingZone();
        role = null;
        nowChannel = null;

    }

    public void OnBuskerOffline(string channelID, uint uid, USER_OFFLINE_REASON reason)
    {
        Debug.Log("onLeaveBuskerInfo: uid = " + uid + " reason = " + reason + " nowChannel: " + channelID);

        if (buskerUid != 0 && buskerUid == uid) // 버스커가 나갈때만
        {
            leaveChannel();
        }
    }

    private void OnAudienceLeaveChannel(string channelID, RtcStats stats)
    {
        Debug.Log("Audience(me) is leave");

        Destroy(audienceVideo.GetComponent<VideoSurface>());
        if (GameManager.instance.myPlayer != null)
        {
            GameManager.instance.myPlayer.GetComponent<PlayerControl>().OffVideoPanel();
            GameManager.instance.myPlayer.GetComponent<PlayerControl>().OffInteractiveButton(2); // 팔로우 버튼 삭제

        }


        buskerUid = 0;
        isFoundBusker = false;
        role = null;
        nowChannel = null;
    }
    #endregion

    private void OnApplicationQuit()
    {
        if (nowChannel != null)
        {
            leaveChannel();
            nowChannel.ReleaseChannel();
        }
    }

}
