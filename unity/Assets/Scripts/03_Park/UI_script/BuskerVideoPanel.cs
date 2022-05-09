using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    // 버스킹 나가기 버튼
    [SerializeField] private Button ExitButton;

    // Input Field
    [SerializeField] private TMP_InputField titleInput;

    // 카메라, 마이크
    [SerializeField] private RawImage cameraImage;
    [SerializeField] private AudioSource MicSource;

    // small Video Panel
    [SerializeField] private GameObject smallVideo;

    private void OnEnable()
    {
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isMoveAble = false;
        //GameManager.instance.myPlayer.GetComponent<PlayerControl>().isUIActable = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        ExitButton.onClick.AddListener(() => { gameObject.SetActive(false); });
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


    public void setDevice()
    {
        AgoraManager.Instance.loadEngine();
        StartButton.onClick.AddListener(StartBusking);
    }

    // 버스킹 인터렉티브
    public void StartBusking()
    {
        if (titleInput.text != "")
        {
            if (AgoraManager.Instance.nowBuskingSpot != null)
            {
                AgoraManager.Instance.nowBuskingSpot.callsetTitle(titleInput.text);
            }
            AgoraManager.Instance.callJoin(0);

            // Busker 화면 없애기
            gameObject.SetActive(false);
            smallVideo.transform.localPosition = new Vector3(-700, 490, 0);
            smallVideo.GetComponent<Button>().enabled = false;
            smallVideo.SetActive(true);
            AgoraManager.Instance.setBuskerVideoSurface(smallVideo.GetComponent<RawImage>());
        }
    }



}
