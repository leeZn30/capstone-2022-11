using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using UnityWebrtc;

public class BuskerVideoPanel : MonoBehaviour
{
    // ī�޶� ����
    protected WebCamTexture textureWebCam = null;
    public GameObject objectTarget;
    private bool isCameraOn = false;

    // ����ũ ����
    public AudioSource micAudioSource;
    private bool isMicOn = false;

    // ī�޶� ����ũ üũ�� �̹���
    [SerializeField] private Image CameraCheck;
    [SerializeField] private Image MicCheck;

    // ����ŷ ���� ��ư
    [SerializeField] private Button StartButton;

    // player
    GameObject player;

    // ī�޶�, ����ũ
    [SerializeField] private RawImage cameraImage;
    [SerializeField] private AudioSource MicSource;

    //WebRTC
    [SerializeField] private GameObject webrtcSignalControls;
    public GameObject localView;

    //Busking ���
    public BuskingSpot nowBuskingSpot;


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

        // ī�޶�
        WebCamDevice[] devices = WebCamTexture.devices;

        int selectedCameraIndex = -1;
        for (int i = 0; i < devices.Length; i++)
        {
            // ��� ������ ī�޶� �α�
            Debug.Log("Available Webcam: " + devices[i].name + ((devices[i].isFrontFacing) ? "(Front)" : "(Back)"));

            // �ĸ� ī�޶����� üũ
            if (devices[i].isFrontFacing)
            {
                // �ش� ī�޶� ����
                selectedCameraIndex = i;
                break;
            }
        }

        // WebCamTexture ����
        if (selectedCameraIndex >= 0)
        {
            // ���õ� ī�޶� ���� ���ο� WebCamTexture�� ����
            textureWebCam = new WebCamTexture(devices[selectedCameraIndex].name);

            // ���ϴ� FPS�� ����
            if (textureWebCam != null)
            {
                textureWebCam.requestedFPS = 60;
            }
        }

        // objectTarget���� ī�޶� ǥ�õǵ��� ����
        if (textureWebCam != null)
        {
            // ī�޶�
            objectTarget.GetComponent<RawImage>().texture = textureWebCam;
            textureWebCam.Play();

            // ī�޶� �����ٰ� ǥ��
            isCameraOn = true;

            cameraImage = objectTarget.GetComponent<RawImage>();


        }
        else // ī�޶� �ؽ��� ����
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

            // ����ũ �����ٰ� ǥ��
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

        StartButton.onClick.AddListener(StartBusking);
    }

    // ����ŷ ���ͷ�Ƽ��
    public void StartBusking()
    {
        /**
        if (isMicOn && isCameraOn)
        {
            player.GetComponent<PlayerControl>().OffInteractiveButton();
            webrtcSignalControls.startLocal();
        }
        **/

        player.GetComponent<PlayerControl>().OffInteractiveButton();
        //webrtcSignalControls.GetComponent<GameObject>().SetActive(true);
        webrtcSignalControls.SetActive(true);
        localView.SetActive(true);
        

    }



}
