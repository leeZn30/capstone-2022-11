using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Unity.WebRTC;
using KyleDulce.SocketIo;
using LitJson;

public class webRTCOperate : Singleton<webRTCOperate>
{
    // BuskingSpot 관련
    public int roomNum;
    public BuskingSpot nowBuskingSpot;

    // Socket 관련
    Socket socket = null;
    string mainServer = "http://localhost:8080";

    // Webrtc 관련
    private RTCPeerConnection sendPC;
    private RTCPeerConnection receivePC;

    // Stream 관련
    [SerializeField] private WebCamTexture textureWebCam;
    //private VideoStreamTrack videoStream;
    private MediaStreamTrack videoStream;

    // BuskerPanel 관련
    [SerializeField] private RawImage buskerRawImage;
    [SerializeField] private RawImage otherRawImageSmall;

    //기타
    bool isSocketOn = false;

    private void Update()
    {
        // input 받아야만 함
        if (Input.GetKeyDown(KeyCode.S) && isSocketOn)
        {
            joinRoom();
        }
    }


    public void webRTCConnect()
    {
        try
        {
            socket = SocketIo.establishSocketConnection(mainServer);
            socket.open();
            isSocketOn = true;

            /**
            if (nowBuskingSpot != null) // 나중에 연결 끊나거나 하면 roomNum이랑 nowvBuskingSpot 초기화시키기
                nowBuskingSpot.isUsed = true; // 일단 에디터에서는ㄴ 잘 바뀜 근데 이거 에디터에서 안하면 바로 안바뀔 수 있다는 점 상기
            **/

            if (nowBuskingSpot != null)
            {
                nowBuskingSpot.callChangeUsed();
            }
        }
        catch
        {
            Debug.Log("No Connection");
        }
    }

    public void setWebCamTexture(WebCamTexture texture)
    {
        textureWebCam = texture;
    }


    void joinRoom()
    {
        WebRTC.Initialize(EncoderType.Software);

        if (socket != null)
        {
            Dictionary<string, dynamic> userOption = new Dictionary<string, dynamic>();
            userOption.Add("roomNum", roomNum);
            userOption.Add("userId", socket.id);

            socket.emit("joinRoom", userOption);
            socket.on("createRoom", onCreateRoom);
            //socket.on("joinRoom", onjoinRoom);

            //socket.on("senderOffer", onSenderOffer);


            //isJoin = true;
        }
    }

    void onSenderOffer(Dictionary<string, dynamic> userOption)
    {
        try
        {
            Debug.Log("[CLIENT]get Offer");
            var offer = userOption["offer"];

            Debug.Log("getOpper: " + offer);

            receivePC.SetRemoteDescription(offer);


            /**
            receivePC
            .createAnswer({
            offerToReceiveAudio: true,
                offerToReceiveVideo: true,
            })
            .then((answer) => {
                 console.log("[CLIENT]sender Answer");
                 receivePC.setLocalDescription(new RTCSessionDescription(answer));
                 socket.emit("getReceiverAnswer", answer, userOption);
             })
            **/
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private static RTCConfiguration GetSelectedSdpSemantics()
    {
        RTCConfiguration config = default;
        config.iceServers = new[] {
            new RTCIceServer { urls = new[] { "stun:stun.services.mozilla.com" } },
            new RTCIceServer { urls = new[] { "stun:stun.l.google.com:19302" } }
        };
        return config;
    }

    void onjoinRoom(Dictionary<string, dynamic> userOption)
    {
        Debug.Log("Join : " + userOption["userId"] + " RoomID : " + userOption["roomNum"]);

        userOption["option"] = 2;

        var config = GetSelectedSdpSemantics();
        receivePC = new RTCPeerConnection(ref config);

        receivePC.OnIceCandidate = Event => {
            if (Event.Candidate != null)
            {
                Debug.Log("[CLIENT]receive Candi");
                userOption["candidate"] = Event.Candidate;

                socket.emit("getReceiverCandidate", userOption);
            }
        };

        receivePC.OnTrack = (RTCTrackEvent e) =>
        {
            Debug.Log("OnTrack is execute");
        };

        userOption["receivePC"] = receivePC;
        socket.emit("joinRoomFromClient", userOption);

        /**
        receiveStream.OnAddTrack = e =>
        {
            if (e.Track is VideoStreamTrack track)
            {
                Texture receiveVideo = track.Texture;

                objectTarget.GetComponent<RawImage>().texture = receiveVideo;

                Debug.Log(receiveVideo);

            }

        };

        receivePC.OnTrack = (RTCTrackEvent e) =>
        {
            Debug.Log("!!!!!!!!!!!!1");

            if (e.Track.Kind == TrackKind.Video)
            {
                // Add track to MediaStream for receiver.
                // This process triggers `OnAddTrack` event of `MediaStream`.
                receiveStream.AddTrack(e.Track);
                Debug.Log("Add Track is finished");
            }
        };
        **/

    }

    // 방을 만들기만 하면 server단에서 senderoffer와 getsendercandidate가 제대로 작동해야함
    void onCreateRoom(Dictionary<string, dynamic> userOption)
    {
        Debug.Log("Create : " + userOption["userId"] + " RoomID : " + userOption["roomNum"]);

        // MediaStreamTrack
        videoStream = new VideoStreamTrack(textureWebCam);

        userOption["option"] = 1;

        var config = GetSelectedSdpSemantics();
        sendPC = new RTCPeerConnection(ref config);

        sendPC.OnIceCandidate = Event =>
        {
            if (!string.IsNullOrEmpty(Event.Candidate)) // Candidate 형식이 다름 > 다 Json으로 만들어줘야함
            {
                Debug.Log("[CLIENT]send Candi");
                
                // Json으로 변경
                JsonData candidate = JsonMapper.ToJson((RTCIceCandidate)Event);
                userOption["candidate"] = candidate;

                // 순서가 > createoffer > oniceCandidate인거 같음
                socket.emit("getSenderCandidate", userOption);
            }
        };

        // MediaStream 넣기
        sendPC.AddTrack(videoStream);

        // unity와 서버의 rtcpeerconnection 구조가 다른듯. 뭐가 필요한지만 알면 될거같은데
        //JsonData sendPCJson = JsonMapper.ToJson(sendPC); // Json으로 넣는 것 만으로도 offer가 보내지지 않음
        //userOption["senderPC"] = sendPC; // peerconnection 객체를 넣는 순간 offer가 보내지지 않음

        StartCoroutine(CreateOffer(userOption));

        // IceCandidateAdd
        //sendPC.OnIceCandidate = candidate => sendPC.AddIceCandidate(candidate);

    }

    private IEnumerator CreateOffer(Dictionary<string, dynamic> userOption)
    {
        var op = sendPC.CreateOffer();
        yield return op;

        if (op.IsError)
        {
            OnCreateSessionDescriptionError(op.Error);
            yield break;
        }

        try
        {
            // offer.Desc의 type이 offer이고, offer.Desc.sdp가 있다.
            var tmp = op.Desc;

            sendPC.SetLocalDescription(ref tmp);

            Dictionary<string, dynamic> offer = new Dictionary<string, dynamic>();
            offer.Add("sdp", op.Desc.sdp);
            userOption["offer"] = offer;

            socket.emit("senderOffer", userOption);

        }
        catch (Exception e)
        {
            Debug.Log("No Sending Offer: " + e);
        }

    }

    private static void OnCreateSessionDescriptionError(RTCError error)
    {
        Debug.LogError($"Failed to create session description: {error.message}");
    }

    void getReceiverAnswer(Dictionary<string, dynamic> answer)
    {
        Debug.Log("[CLIENT]get Answer");

        //await sendPC.setRemoteDescription(answer);
        Debug.Log(answer["answer"]);
        sendPC.SetRemoteDescription(answer["answer"]);
    }

    void getCandidate(Dictionary<string, dynamic> data)
    {
        Debug.Log("[CLIENT]get Candi " + data);

        var candidate = data["candidate"];
        var option = data["option"];

        var iceCandidate = new RTCIceCandidate(candidate);

        if (option == 1)
        {
            // 일단 필요없을듯
            //buskerRawImage.texture = videoStream.Texture;
        }
        else
        {
            receivePC.AddIceCandidate(iceCandidate);
        }

    }


    // ---------JSON 만들기---------

}
