using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using agora_gaming_rtc;
using Photon.Pun;


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

    // Start is called before the first frame update
    void Start()
    {
        ExitButton.onClick.AddListener(exitPanel);
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

    private void exitPanel()
    {
        gameObject.SetActive(false);
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isMoveAble = true;
        GameManager.instance.myPlayer.GetComponent<PlayerControl>().isUIActable = true;
    }

    public void setDevice()
    {
        AgoraManager.Instance.loadEngine();
        StartButton.onClick.RemoveAllListeners(); // 지워주고 해야함
        StartButton.onClick.AddListener(StartBusking);
    }

    // 버스킹 인터렉티브
    public void StartBusking()
    {
        if (titleInput.text != "" && titleInput.text != null)
        {
            AgoraManager.Instance.callJoin(0);
            
            if (AgoraManager.Instance.nowBuskingSpot != null)
            {
                AgoraManager.Instance.nowBuskingSpot.callsetTitle(PhotonNetwork.LocalPlayer.NickName, titleInput.text);
            }

            // Busker 화면 없애기
            gameObject.SetActive(false);
            smallVideo.transform.localPosition = new Vector3(-700, 350, 0);
            smallVideo.GetComponent<Button>().enabled = false;
            smallVideo.SetActive(true);
            AgoraManager.Instance.setBuskerVideoSurface(smallVideo.GetComponent<RawImage>());

            // 그만두기 버튼 설정
            PlayerControl player = GameManager.instance.myPlayer.GetComponent<PlayerControl>();
            player.changeInteractiveButton(1);
            player.InteractiveButton.GetComponent<Button>().onClick.AddListener(
                delegate { AgoraManager.Instance.leaveChannel(); });
        }
    }



}
