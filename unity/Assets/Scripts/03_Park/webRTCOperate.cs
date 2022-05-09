using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
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
    string mainServer = "http://172.20.10.4:8080";

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

            if (nowBuskingSpot != null) // 나중에 연결 끊나거나 하면 roomNum이랑 nowvBuskingSpot 초기화시키기
            {
                nowBuskingSpot.callChangeUsed(); // 일단 에디터에서는ㄴ 잘 바뀜 근데 이거 에디터에서 안하면 바로 안바뀔 수 있다는 점 상기
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
            Debug.Log("User : " + socket.id);
            socket.emit("joinRoom", userOption);
            socket.on("createRoom", onCreateRoom);
            socket.on("joinRoom", onjoinRoom);
            socket.on("senderOffer", onSenderOffer);
            socket.on("getReceiverAnswer", onGetReceiverAnswer);
            socket.on("getCandidate", getCandidate);


            //isJoin = true;
        }
    }

    void onSenderOffer(Dictionary<string, dynamic> userOption)
    {
        try
        {
            Debug.Log("[CLIENT]get Offer");
            string resultOffer = "" + userOption["offer"];
            RTCSessionDescription sdOffer = new RTCSessionDescription();
            sdOffer.type = RTCSdpType.Offer;
            sdOffer.sdp = resultOffer;
            receivePC.SetRemoteDescription(ref sdOffer);
            
            Debug.Log("[CLIENT]Offer Is Done");
            StartCoroutine(CreateAnswer(userOption));
            // var op = receivePC.CreateAnswer();

            // // offer.Desc의 type이 offer이고, offer.Desc.sdp가 있다.
            // var tmp = op.Desc;
            // receivePC.SetLocalDescription(ref tmp);

            // Dictionary<string, dynamic> answer = new Dictionary<string, dynamic>();
            // answer.Add("sdp", tmp.sdp);
            // Debug.Log(userOption["userId"] + " 1");
            // userOption["answer"] = answer;
            // socket.emit("getReceiverAnswer", userOption);

        }
        catch (Exception e)
        {
            Debug.Log("[CLIENT ERROR -!! (onSenderOffer)] " + e);
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
            if (!string.IsNullOrEmpty(Event.Candidate)) // Candidate 형식이 다름 > 다 Json으로 만들어줘야함
            {
                Debug.Log("[CLIENT]send Candi");
                // Json으로 변경
                try {
                    // string candidate = JsonUtility.ToJson(Event.Candidate);
                    string candidate = Event.Candidate;
                    Dictionary<string, dynamic> candidateDic = new Dictionary<string, dynamic>();
                    candidateDic.Add("candidate", candidate);
                    candidateDic.Add("sdpMid", 0);
                    candidateDic.Add("sdpMLineIndex", 0);
                    userOption["candidate"] = candidateDic;

                    // 순서가 > createoffer > oniceCandidate인거 같음
                    socket.emit("getReceiverCandidate", userOption);
                }
                catch (Exception e) {
                    Debug.Log(e);
                }
            }
        };

        receivePC.OnTrack = (RTCTrackEvent e) => {
            if (e.Track.Kind == TrackKind.Video)
            {
                otherRawImageSmall
            }
        };
        // {
        //     Debug.Log("OnTrack is execute");
        // };
        // userOption["receivePC"] = receivePC;
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
                try {
                    // string candidate = JsonUtility.ToJson(Event.Candidate);
                    string candidate = Event.Candidate;
                    Dictionary<string, dynamic> candidateDic = new Dictionary<string, dynamic>();
                    candidateDic.Add("candidate", candidate);
                    candidateDic.Add("sdpMid", 0);
                    candidateDic.Add("sdpMLineIndex", 0);
                    userOption["candidate"] = candidateDic;

                    // 순서가 > createoffer > oniceCandidate인거 같음
                    socket.emit("getSenderCandidate", userOption);
                }
                catch (Exception e) {
                    Debug.Log(e);
                }
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

    private IEnumerator CreateAnswer(Dictionary<string, dynamic> userOption)
    {
        var op = receivePC.CreateAnswer();
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
            receivePC.SetLocalDescription(ref tmp);
            Dictionary<string, dynamic> answer = new Dictionary<string, dynamic>();
            answer.Add("sdp", op.Desc.sdp);
            userOption["answer"] = answer;
            socket.emit("getReceiverAnswer", userOption);

        }
        catch (Exception e)
        {
            Debug.Log("No Sending Answer: " + e);
        }

    }

    private static void OnCreateSessionDescriptionError(RTCError error)
    {
        Debug.LogError($"Failed to create session description: {error.message}");
    }

    void onGetReceiverAnswer(Dictionary<string, dynamic> answer)
    {
        Debug.Log("[CLIENT]get Answer");

        //await sendPC.setRemoteDescription(answer);
        RTCSessionDescription sdAnswer = new RTCSessionDescription();
        sdAnswer.type =  RTCSdpType.Answer;
        try {
            string resultAnswer = "" + answer["answer"];
            sdAnswer.sdp = resultAnswer;
            sendPC.SetRemoteDescription(ref sdAnswer);
            Debug.Log("[CLIENT]Answer Is Done");
        }
        catch (Exception e) {
            Debug.Log("Error Set Des : " + e);
        }
    }

    void getCandidate(Dictionary<string, dynamic> data)
    {
        try {
            Debug.Log("[CLIENT]get Candi");
            string candi = "" + data["candidate"];
            string sdpMid = "" + data["sdpMid"];
            string sdpMLineIndexStr = ""+ data["sdpMLineIndex"];
            int? sdpMLineIndex = int.Parse(sdpMLineIndexStr);
            RTCIceCandidateInit candiInfo = new RTCIceCandidateInit();
            candiInfo.candidate = candi;
            candiInfo.sdpMid = sdpMid;
            candiInfo.sdpMLineIndex = sdpMLineIndex;

            RTCIceCandidate candidate = new RTCIceCandidate(candiInfo);

            int option = int.Parse(""+ data["option"]);
            if (option == 1)
            {
                // 일단 필요없을듯
                //buskerRawImage.texture = videoStream.Texture;
                sendPC.AddIceCandidate(candidate);
            }
            else
            {
                // receivePC.AddIceCandidate(candidateObj);
            }
                Debug.Log("End Candi !!!");
        }
        catch (Exception e) {
            Debug.Log("[CLIENT ERROR-!!(getCandidate) : " + e);
        }
    }


    // ---------JSON 만들기---------

}
