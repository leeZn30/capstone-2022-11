using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.WebRTC;
using KyleDulce.SocketIo;
using System.IO;
using System;

public class BuskerVideoPanel : MonoBehaviour
{
    // Stream
    Socket socket;
    string mainServer = "http://localhost:8080";
    public MediaStream sourceStream;

    // 카메라 관련
    protected WebCamTexture textureWebCam = null;
    public GameObject objectTarget;
    private bool isCameraOn = false;

    // 마이크 관련
    public AudioSource micAudioSource;
    private bool isMicOn = false;

    // 카메라 마이크 체크할 이미지
    [SerializeField] private Image CameraCheck;
    [SerializeField] private Image MicCheck;

    // webRTC script
    [SerializeField] public WebRTC webrtc;

    // 버스킹 시작 버튼
    [SerializeField] private Button StartButton;

    // player
    GameObject player;

    //TCP
    /**
    TcpClient tcpsocket;
    NetworkStream stream;
    StreamWriter writer;
    StreamReader reader;
    **/

    // 카메라, 마이크
    [SerializeField] private RawImage cameraImage;
    [SerializeField] private AudioSource MicSource;

    private RTCConfiguration config = new RTCConfiguration
    {
        iceServers = new[] { new RTCIceServer { urls = new[] { "stun:stun.services.mozilla.com", "stun:stun.l.google.com:19302" } } }
    };


    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.myPlayer;

    }

    void test(Dictionary<string, dynamic> t)
    {
        Debug.Log(t["hey"]);
    }

    // Update is called once per frame
    void Update()
    {

        if (isCameraOn)
            CameraCheck.color = Color.green;
        else
            CameraCheck.color = Color.white;

        if (isMicOn)
            MicCheck.color = Color.green;
        else
            MicCheck.color = Color.white;


        if (Input.GetKeyDown(KeyCode.S))
        {
            joinRoom();
        }

    }

    /**
    void socketConnect()
    {

        try
        {
            tcpsocket = new TcpClient("http://localhost", 8080);
            stream = tcpsocket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            //socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log($"소켓에러 : {e.Message}");
        }
    }
    **/


    private void cameraConnect()
    {

        // 카메라
        WebCamDevice[] devices = WebCamTexture.devices;

        int selectedCameraIndex = -1;
        for (int i = 0; i < devices.Length; i++)
        {
            // 사용 가능한 카메라 로그
            Debug.Log("Available Webcam: " + devices[i].name + ((devices[i].isFrontFacing) ? "(Front)" : "(Back)"));

            // 후면 카메라인지 체크
            if (devices[i].isFrontFacing)
            {
                // 해당 카메라 선택
                selectedCameraIndex = i;
                break;
            }
        }

        // WebCamTexture 생성
        if (selectedCameraIndex >= 0)
        {
            // 선택된 카메라에 대한 새로운 WebCamTexture를 생성
            textureWebCam = new WebCamTexture(devices[selectedCameraIndex].name);

            // 원하는 FPS를 설정
            if (textureWebCam != null)
            {
                textureWebCam.requestedFPS = 60;
            }
        }

        // objectTarget으로 카메라가 표시되도록 설정
        if (textureWebCam != null)
        {
            // 카메라
            objectTarget.GetComponent<RawImage>().texture = textureWebCam;
            textureWebCam.Play();

            // 카메라 켜졌다고 표시
            isCameraOn = true;

            cameraImage = objectTarget.GetComponent<RawImage>();


            //MediaStreamTrack 


        }
        else // 카메라 텍스쳐 없음
        {
            Debug.Log("No Camera Texture");
        }
    }

    private void micConnect()
    {
        try
        {
            string mic = Microphone.devices[0];
            micAudioSource.clip = Microphone.Start(mic, true, 10, 44100);
            while (!(Microphone.GetPosition(mic) > 0)) { } // Wait until the recording has started
            micAudioSource.Play(); // Play the audio source!

            // 마이크 켜졌다고 표시
            isMicOn = true;
        }
        catch
        {
            Debug.Log("No mic");
        }
    }

    public void setDevice()
    {
        cameraConnect();
        micConnect();

        StartButton.onClick.AddListener(StartBusking);
    }

    // 버스킹 인터렉티브
    public void StartBusking()
    {
        if (isMicOn && isCameraOn)
        {
            player.GetComponent<PlayerControl>().OffInteractiveButton();
            webRTCConnect();
        }

    }

    public void webRTCConnect()
    {
        try
        {
            socket = SocketIo.establishSocketConnection(mainServer);
            socket.open();

        }
        catch 
        {
            Debug.Log("No Connection");
        }

    }



    void joinRoom()
    {
        int roomNum = 1; // 일단 통일

        if (socket != null)
        {
            Dictionary<string, dynamic> userOption = new Dictionary<string, dynamic>();
            userOption.Add("roomNum", roomNum);
            userOption.Add("userId", socket.id);

            socket.emit("joinRoom", userOption);

            socket.on("createRoom", onCreateRoom);

            //isJoin = true;
        }
    }

    void callCreateRoom()
    {
        if (socket != null)
        {
            socket.on("createRoom", onCreateRoom);
        }
    }

    // on을 하기 위해서는 emit안에 넣어야 하는 것 같음 why..?
    void onCreateRoom(Dictionary<string, dynamic> userOption)
    {

        var sendPC = new RTCPeerConnection(ref config);
        Debug.Log("!!!!!!!!");
        Debug.Log(sendPC);

        sendPC.OnIceCandidate = candidate => sendPC.AddIceCandidate(candidate);

        var opOffer = sendPC.CreateOffer();

        if (opOffer != null)
        {
            RTCSessionDescription desc = new RTCSessionDescription();
            desc = opOffer.Desc;
            sendPC.SetLocalDescription(ref desc);

            //Dictionary<string, object> tmp = (Dictionary<string, object>) userOption;
            //Debug.Log("Tmp : " + tmp);
            //tmp["offer"] = opOffer;
           
            //socket.emit("senderOffer", userOption);
        }

        //user option = 1
        try
        {
            //var sendPc = new RTCPeerConnection(ref config);

            /**
            sendPc.OnTrack = e =>
            {
                if (e.Track is VideoStreamTrack videoTrack)
                {
                    videoTrack.OnVideoReceived += tex =>
                    {
                        cameraImage.texture = tex;

                        //??????
                        sourceStream.AddTrack(videoTrack);
                    };
                }

                if (e.Track is AudioStreamTrack audioTrack)
                {
                    MicSource.SetTrack(audioTrack);
                    MicSource.loop = true;
                    MicSource.Play();
                }
            };

            sendPc.OnIceCandidate = candidate => sendPc.AddIceCandidate(candidate);


            foreach (var track in sourceStream.GetTracks())
            {
                sendPc.AddTrack(track, sourceStream);
            }

            var offer = sendPc.CreateOffer();

            var offerDesc = offer.Desc;
            sendPc.SetLocalDescription(ref offerDesc);

            //socket.emit("senderOffer", "TESt");
            **/
        }
        catch
        {

        }
    }



}
