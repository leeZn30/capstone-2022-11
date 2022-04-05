using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System;
using KyleDulce.SocketIo;

public class BuskingSpot : MonoBehaviourPun, IPunObservable
{
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isUsed);
        }
        else
        {
            this.isUsed = (bool)stream.ReceiveNext();
        }
    }

    // 카메라 관련
    protected WebCamTexture textureWebCam = null;
    [SerializeField] private GameObject objectTarget;

    // 마이크 관련
    [SerializeField] private AudioSource micAudioSource;

    public bool isUsed = false;

    Socket socket;

    private void Update()
    {
        if (isUsed)
        {
            this.GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else
        {
            this.GetComponent<SpriteRenderer>().color = new Color32(202, 162, 48, 250);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject player = GameManager.instance.myPlayer;
        if (collision.gameObject == player && player.GetComponent<PhotonView>().IsMine)
        {
            if (isUsed && !player.GetComponent<PlayerControl>().isVideoPanelShown)
                collision.transform.GetComponent<PlayerControl>().OnVideoPanel(0);
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject player = GameManager.instance.myPlayer;
        if (collision.gameObject == player && player.GetComponent<PhotonView>().IsMine)
        {
            if (player.GetComponent<PlayerControl>().isVideoPanelShown)
                collision.transform.GetComponent<PlayerControl>().OffVideoPanel();
        }
    }


    // 버스킹 인터렉티브
    public void StartBusking()
    {
        isUsed = true;

        GameObject player = GameManager.instance.myPlayer;

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
            player.GetComponent<PlayerControl>().OnVideoPanel(1);
            textureWebCam.Play();

            // 소리
            try
            {
                string mic = Microphone.devices[0];
                micAudioSource.clip = Microphone.Start(mic, true, 10, 44100);
                while (!(Microphone.GetPosition(mic) > 0)) { } // Wait until the recording has started
                micAudioSource.Play(); // Play the audio source!
                player.GetComponent<PlayerControl>().OffInteractiveButton();
                webRTCConnect();
            }
            catch
            {
                webRTCConnect();
                Debug.Log("No mic");
            }

        }
        else // 카메라 텍스쳐 없음
        {
            webRTCConnect();
            Debug.Log("No Camera Texture");
        }

    }

    void webRTCConnect()
    {
        try
        {
            socket = SocketIo.establishSocketConnection("http://localhost:8080");
            socket.connect();

            Debug.Log(socket);
            Debug.Log("Connect Success!");
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }

    }

}
