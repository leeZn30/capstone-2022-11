using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;


public class BuskerVideoPanel : MonoBehaviour
{
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

    // 버스킹 시작 버튼
    [SerializeField] private Button StartButton;

    // 토큰 버튼
    [SerializeField] private Button tokenButton;

    // player
    GameObject player;

    // 카메라, 마이크
    [SerializeField] private RawImage cameraImage;
    [SerializeField] private AudioSource MicSource;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.myPlayer;
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

    }


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

            // stream
        }
        catch
        {
            Debug.Log("No mic");
        }
    }

    public void setDevice()
    {
        //cameraConnect();
        //micConnect();

        AgoraManager.Instance.loadEngine();
        tokenButton.onClick.AddListener(callSetToken);
        StartButton.onClick.AddListener(StartBusking);
    }

    public void callSetToken()
    {
        AgoraManager.Instance.setToken("publisher");
    }

    // 버스킹 인터렉티브
    public void StartBusking()
    {
        /**
        if (isMicOn && isCameraOn)
        {
            player.GetComponent<PlayerControl>().OffInteractiveButton();
            webRTCOperate.Instance.webRTCConnect();
            webRTCOperate.Instance.setWebCamTexture(textureWebCam);
        }
        **/

        //AgoraManager.Instance.setBuskerAgora(1); // 현재 roomNum 1로 고정
        AgoraManager.Instance.join(0);

    }



}
